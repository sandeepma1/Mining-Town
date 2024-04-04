using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MiningTown.IO
{
    public static class SourceDatabase
    {
        private static Dictionary<int, Source> sources = new Dictionary<int, Source>();
        private const string fileName = "Source";

        public static void LoadDatabase()
        {
            List<string> linesList = fileName.ToCsvLines();
            for (int i = 0; i < linesList.Count; i++)
            {
                string[] chars = Regex.Split(linesList[i], ",");
                if (!sources.ContainsKey(chars[0].ToInt()))
                {
                    sources.Add(chars[0].ToInt(),
                        new Source(chars[0].ToInt(), chars[1], chars[3].ToInt(), chars[4],
                        chars[5].ToInt(), chars[6].ToInt(), chars[7]));
                }
            }
            //print("SourceDatabase loaded " + sources.Count);
        }

        public static string GetSlugBySourceId(int sourceId)
        {
            return sources[sourceId].slug;
        }

        public static Source GetSourceInfoById(int itemId)
        {
            return sources[itemId];
        }

        public static Dictionary<int, Source> GetAllSources()
        {
            return sources;
        }

        public static string GetSourceNameById(int itemId)
        {
            return sources[itemId].name;
        }

        public static int GetSourceInfolength()
        {
            return sources.Count;
        }

    }
}

[System.Serializable]
public class Source
{
    //id	name	category	categoryId	description	buildCost	deployTime	slug
    //0      1       2           3           4                           5        6
    public int id;
    public string name;
    public int categoryId;
    public string description;
    public int buildCost;
    public int deployTime;
    public string slug;

    public Source(int id, string name, int categoryId, string description, int buildCost, int deployTime, string slug)
    {
        this.id = id;
        this.name = name;
        this.categoryId = categoryId;
        this.description = description;
        this.buildCost = buildCost;
        this.deployTime = deployTime;
        this.slug = slug;
    }
}