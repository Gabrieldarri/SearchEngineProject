using System.Collections.Generic;
using Shared.Model;

namespace SearchAPI.Logic;

// CompositeDatabase queries two databases and merges results so the API can search across both.
public class CompositeDatabase : IDatabase
{
    private readonly IDatabase dbA;
    private readonly IDatabase dbB;

    public CompositeDatabase(IDatabase a, IDatabase b)
    {
        dbA = a;
        dbB = b;
    }

    public Dictionary<string, int> GetAllWords()
    {
        // merge dictionaries; prefer dbA entries when duplicate
        var a = dbA.GetAllWords();
        var b = dbB.GetAllWords();
        foreach (var kv in b)
        {
            if (!a.ContainsKey(kv.Key))
                a.Add(kv.Key, kv.Value);
        }
        return a;
    }

    public BEDocument GetDocDetails(int docId)
    {
        var d = dbA.GetDocDetails(docId);
        if (d != null) return d;
        return dbB.GetDocDetails(docId);
    }

    public List<(int docId, int hits)> GetDocuments(List<int> wordIds)
    {
        var listA = dbA.GetDocuments(wordIds);
        var listB = dbB.GetDocuments(wordIds);
        var combined = new List<(int docId, int hits)>();
        combined.AddRange(listA);
        combined.AddRange(listB);
        // order by hits desc
        combined.Sort((x, y) => y.hits.CompareTo(x.hits));
        return combined;
    }

    public List<string> GetMissing(int docId, List<int> wordIds)
    {
        var r = dbA.GetMissing(docId, wordIds);
        if (r != null && r.Count > 0) return r;
        return dbB.GetMissing(docId, wordIds);
    }

    public List<string> GetHits(int docId, List<int> wordIds)
    {
        var r = dbA.GetHits(docId, wordIds);
        if (r != null && r.Count > 0) return r;
        return dbB.GetHits(docId, wordIds);
    }
}
