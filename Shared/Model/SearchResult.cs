using System;
using System.Collections.Generic;


namespace Shared.Model;
    /*
     * A data class representing the result of a search.
     * Hits is the total number of documents containing at least one word from the query.
     * DocumentHits is the documents and the number of words from the query contained in the document - see
     * the class DocumentHit
     * Ignored contains words from the query not present in the document base.
     * TimeUsed is the timespan used to perform the search.
     */
    public class SearchResult
    {
        

        public String[] Query { get; set; }


        /// <summary>
        /// The total number of documents containing at least one word from the query
        /// </summary>
        public int NoOfHits { get; set; }
        
        /// <summary>
        /// The most important details about the documents hit by the query
        /// </summary>
        public List<DocumentHit> DocumentHits { get; set; }

        /// <summary>
        /// Words from the query that is ignored because they are not in any document
        /// </summary>
        public List<string> Ignored { get; set; }

        /// <summary>
        /// The care time used for the search
        /// </summary>
        public TimeSpan TimeUsed { get; set; }
    }

