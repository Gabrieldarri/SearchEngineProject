using System;
using System.Collections.Generic;
using Shared.Model;
using Npgsql;
using Shared;
namespace Indexer;

public class DatabasePostgres : IDatabase
{
    private readonly NpgsqlConnection _connection;

    public DatabasePostgres()
    {
        var csb = new NpgsqlConnectionStringBuilder(Paths.POSTGRES_DATABASE);
        // ensure port/host are parsed correctly
        if (string.IsNullOrEmpty(csb.Host)) csb.Host = "127.0.0.1";
        if (csb.Port == 0) csb.Port = 5432;

        _connection = new NpgsqlConnection(csb.ConnectionString);

        // Lazy-open: attempt open here but don't throw to caller (optionally fall back)
        try
        {
            _connection.Open();
        }
        catch (NpgsqlException ex)
        {
            // Log and rethrow or set a flag so callers can fall back
            throw new InvalidOperationException("Could not open Postgres connection. Ensure server is running and connection string is correct.", ex);
        }

        Execute("DROP TABLE IF EXISTS Occ");

        Execute("DROP TABLE IF EXISTS document");
        Execute("CREATE TABLE document(id INTEGER PRIMARY KEY, url TEXT, idxTime TIMESTAMP, creationTime TIMESTAMP)");

        Execute("DROP TABLE IF EXISTS word");
        Execute("CREATE TABLE word(id INTEGER PRIMARY KEY, name TEXT)");

        Execute("CREATE TABLE Occ(wordId INTEGER, docId INTEGER, "
                + "FOREIGN KEY (wordId) REFERENCES word(id), "
                + "FOREIGN KEY (docId) REFERENCES document(id))");
        Execute("CREATE INDEX word_index ON Occ (wordId)");
    }

    private void Execute(string sql)
    {
        var cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    public void InsertAllWords(Dictionary<string, int> res)
    {
        using (var transaction = _connection.BeginTransaction())
        {
            var command = _connection.CreateCommand();
            command.CommandText =
                @"INSERT INTO word(id, name) VALUES(@id,@name)";

            var paramName = command.CreateParameter();
            paramName.ParameterName = "name";
            command.Parameters.Add(paramName);

            var paramId = command.CreateParameter();
            paramId.ParameterName = "id";
            command.Parameters.Add(paramId);

            // Insert all entries in the res

            foreach (var p in res)
            {
                paramName.Value = p.Key;
                paramId.Value = p.Value;
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
    }

    public void InsertAllOcc(int docId, ISet<int> wordIds)
    {
        using (var transaction = _connection.BeginTransaction())
        {
            var command = _connection.CreateCommand();
            command.CommandText =
                @"INSERT INTO occ(wordId, docId) VALUES(@wordId,@docId)";

            var paramwordId = command.CreateParameter();
            paramwordId.ParameterName = "wordId";

            command.Parameters.Add(paramwordId);

            var paramDocId = command.CreateParameter();
            paramDocId.ParameterName = "docId";
            paramDocId.Value = docId;

            command.Parameters.Add(paramDocId);

            foreach (var p in wordIds)
            {
                paramwordId.Value = p;

                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
    }

    public void InsertWord(int id, string value)
    {
        var insertCmd = new NpgsqlCommand("INSERT INTO word(id, name) VALUES(@id,@name)");
        insertCmd.Connection = _connection;

        var pName = new NpgsqlParameter("name", value);
        insertCmd.Parameters.Add(pName);

        var pCount = new NpgsqlParameter("id", id);
        insertCmd.Parameters.Add(pCount);

        insertCmd.ExecuteNonQuery();
    }

    public void InsertDocument(BEDocument doc)
    {
        var insertCmd =
            new NpgsqlCommand(
                "INSERT INTO document(id, url, idxTime, creationTime) VALUES(@id,@url, @idxTime, @creationTime)");
        insertCmd.Connection = _connection;

        var pId = new NpgsqlParameter("id", doc.Id);
        insertCmd.Parameters.Add(pId);

        var pUrl = new NpgsqlParameter("url", doc.Url);
        insertCmd.Parameters.Add(pUrl);

        var pIdxTime = new NpgsqlParameter("idxTime", doc.IdxTime);
        insertCmd.Parameters.Add(pIdxTime);

        var pCreationTime = new NpgsqlParameter("creationTime", doc.CreationTime);
        insertCmd.Parameters.Add(pCreationTime);

        insertCmd.ExecuteNonQuery();

    }

    public Dictionary<string, int> GetAllWords()
    {
        Dictionary<string, int> res = new Dictionary<string, int>();

        var selectCmd = _connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM word";

        using (var reader = selectCmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var w = reader.GetString(1);

                res.Add(w, id);
            }
        }

        return res;
    }

    public int DocumentCounts
    {
        get
        {
            var selectCmd = _connection.CreateCommand();
            selectCmd.CommandText = "SELECT count(*) FROM document";

            using (var reader = selectCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    var count = reader.GetInt32(0);
                    return count;
                }
            }

            return -1;
        }
    }
}