using System;
using System.Collections.Generic;
using Shared.Model;

namespace SearchAPI.Logic;
    public class SearchLogic
    {
        private IDatabase mDatabase;

        // all words from the database in a cache. The key is the
        // word itself and the value is the id.
        private Dictionary<string, int>? mWords;

        public SearchLogic(IDatabase database) {
            mDatabase = database;
        }

        /* Perform search of documents containing words from query. The result will
         * contain details about amost maxAmount of documents.
         */
        public SearchResult Search(String[] query, int maxAmount)
        {
            List<string> ignored;

            DateTime start = DateTime.Now;

            // Convert words to wordids
            var wordIds = GetWordIds(query, out ignored);

            if (wordIds.Count == 0) // no words present in index
                 return new SearchResult { Query = query, 
                                           NoOfHits = 0,
                                           DocumentHits = new List<DocumentHit>(), 
                                           Ignored = ignored, 
                                           TimeUsed = DateTime.Now - start};
            
            // perform the search - get all docIds
            var docIds =  mDatabase.GetDocuments(wordIds);

            // get ids for the first maxAmount             
            var top = new List<int>();
            foreach (var p in docIds.GetRange(0, Math.Min(maxAmount, docIds.Count)))
                top.Add(p.docId);

            // compose the result.
            // all the documentHit
            List<DocumentHit> docresult = new List<DocumentHit>();
            int idx = 0;
            foreach (var docId in top)
            {
                BEDocument doc = mDatabase.GetDocDetails(docId);
                var missing = mDatabase.GetMissing(doc.Id, wordIds);
                var hits = mDatabase.GetHits(doc.Id, wordIds);
                missing.AddRange(ignored);
                var docHit = new DocumentHit { Document = doc, Hits = hits, Missing = missing };
                docresult.Add(docHit);
            }

            return new SearchResult{ Query = query, 
                                     NoOfHits = docIds.Count,
                                     DocumentHits = docresult, 
                                     Ignored = ignored, 
                                     TimeUsed = DateTime.Now - start };
        }
        
        /// <summary>
        /// Get id's for words in [query]. [outIgnored] contains those word from query that is
        /// not present in any document.
        /// </summary>
        private List<int> GetWordIds(string[] query, out List<string> outIgnored)
        {
            if (mWords == null)
                mWords = mDatabase.GetAllWords();
            var res = new List<int>();
            var ignored = new List<string>();

            foreach (var aWord in query)
            {
                if (mWords.ContainsKey(aWord))
                    res.Add(mWords[aWord]);
                else
                    ignored.Add(aWord);
            }
            outIgnored = ignored;
            return res;
        }
    }
