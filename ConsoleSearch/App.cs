using System;
using System.Threading.Tasks;
using Shared;

namespace ConsoleSearch;
    public class App
    {

        public async Task Run()
        {
            SearchLogicProxy mSearchLogic = new SearchLogicProxy();
            Console.WriteLine("Console Search");
            
            while (true)
            {
                Console.WriteLine("enter search terms - q for quit");
                string input = Console.ReadLine();
                if (input.Equals("q")) break;

                var query = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
               

                var result = await mSearchLogic.Search(query, 10);

                if (result.Ignored.Count > 0) {
                    Console.WriteLine($"Ignored: {string.Join(',', result.Ignored)}");
                }
                
                int idx = 1;
                foreach (var doc in result.DocumentHits) {
                    Console.WriteLine($"{idx} : {doc.Document.Url} -- contains {doc.Hits.Count} search terms");
                    Console.WriteLine("Index time: " + doc.Document.IdxTime);
                    Console.WriteLine($"Missing: {ArrayAsString(doc.Missing.ToArray())}");
                    idx++;
                }
                Console.WriteLine($"Documents: {result.NoOfHits}. Time: {result.TimeUsed.TotalMilliseconds}");
            }
        }
        
       

        string ArrayAsString(string[] s) => s.Length == 0?"[]":$"[{String.Join(',', s)}]";
    }

