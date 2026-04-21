using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shared;

namespace Indexer;
    public class App
    {
        public void Run()
        {
            IDatabase db = GetDatabase();
            Crawler crawler = new Crawler(db);

            var root = new DirectoryInfo(Config.FOLDER);

            DateTime start = DateTime.Now;

            crawler.IndexFilesIn(root, new List<string> { ".txt"});        

            TimeSpan used = DateTime.Now - start;
            Console.WriteLine("DONE! used " + used.TotalMilliseconds);

            var all = db.GetAllWords();

            Console.WriteLine($"Indexed {db.DocumentCounts} documents");
            Console.WriteLine($"Number of different words: {all.Count}");
            int count = 10;
            Console.WriteLine($"The first {count} is:");
            foreach (var p in all.Take(count)) {
                Console.WriteLine("<" + p.Key + ", " + p.Value + ">");
            }
        }

        private IDatabase GetDatabase()
        {
            Console.Write("Use SQLite (1), Postgres (2) or Reception split (3)?");
            string input = Console.ReadLine();
            if (input.Equals("1"))
                return new DatabaseSqlite();
            else if (input.Equals("2"))
                return new DatabasePostgres();
            else if (input.Equals("3"))
            {
                // Try to use Postgres as the second DB; if unavailable, fall back
                // to a second sqlite file so reception still works when only sqlite is used.
                IDatabase a = new DatabaseSqlite();
                IDatabase b;
                try
                {
                    b = new DatabasePostgres();
                }
                catch
                {
                    // fallback: create a second sqlite database file
                    var altPath = Shared.Paths.SQLITE_DATABASE + ".part2.db";
                    b = new DatabaseSqlite(altPath);
                }
                return new ReceptionDatabase(a, b);
            }
            Console.WriteLine("Wrong input - try again...");
            return GetDatabase();
        }
    }