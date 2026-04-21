using System.Collections.Generic;
using Shared.Model;

namespace Indexer;

// ReceptionDatabase forwards InsertAllWords/InsertWord to both databases
// and splits documents/occurrences between dbA and dbB by document id parity.
public class ReceptionDatabase : IDatabase
{
    private readonly IDatabase dbA;
    private readonly IDatabase dbB;

    public ReceptionDatabase(IDatabase a, IDatabase b)
    {
        dbA = a;
        dbB = b;
    }

    public void InsertAllOcc(int docId, ISet<int> wordIds)
    {
        // route occurrences to the DB owning the document (by parity)
        if ((docId % 2) == 0)
            dbB.InsertAllOcc(docId, wordIds);
        else
            dbA.InsertAllOcc(docId, wordIds);
    }

    public void InsertAllWords(Dictionary<string, int> words)
    {
        // ensure both DBs have the same word-id mapping
        dbA.InsertAllWords(words);
        dbB.InsertAllWords(words);
    }

    public void InsertDocument(BEDocument doc)
    {
        if ((doc.Id % 2) == 0)
            dbB.InsertDocument(doc);
        else
            dbA.InsertDocument(doc);
    }

    public void InsertWord(int id, string value)
    {
        dbA.InsertWord(id, value);
        dbB.InsertWord(id, value);
    }

    public Dictionary<string, int> GetAllWords()
    {
        // both DBs should have identical word lists because InsertAllWords writes to both.
        return dbA.GetAllWords();
    }

    public int DocumentCounts => dbA.DocumentCounts + dbB.DocumentCounts;
}
