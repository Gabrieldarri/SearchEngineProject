using Shared.Model;

namespace SearchAPI.Logic;

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
        var a = dbA.GetAllWords();
        var b = dbB.GetAllWords();
        foreach (var kv in b)
            if (!a.ContainsKey(kv.Key))
                a.Add(kv.Key, kv.Value);
        return a;
    }

    public BEDocument GetDocDetails(int docId)
    {
        return dbA.GetDocDetails(docId) ?? dbB.GetDocDetails(docId);
    }

    public List<(int docId, int hits)> GetDocuments(List<int> wordIds)
    {
        var combined = new List<(int docId, int hits)>();
        combined.AddRange(dbA.GetDocuments(wordIds));
        combined.AddRange(dbB.GetDocuments(wordIds));
        combined.Sort((x, y) => y.hits.CompareTo(x.hits));
        return combined;
    }

    public List<string> GetMissing(int docId, List<int> wordIds)
    {
        var r = dbA.GetMissing(docId, wordIds);
        return (r != null && r.Count > 0) ? r : dbB.GetMissing(docId, wordIds);
    }

    public List<string> GetHits(int docId, List<int> wordIds)
    {
        var r = dbA.GetHits(docId, wordIds);
        return (r != null && r.Count > 0) ? r : dbB.GetHits(docId, wordIds);
    }
}
