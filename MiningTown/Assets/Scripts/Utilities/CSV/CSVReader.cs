using UnityEngine;
using System.Linq;

public static class CSVReader
{
    public static void DebugOutputGrid(string[,] grid)
    {
        string textOutput = "";
        for (int y = 0; y < grid.GetUpperBound(1); y++)
        {
            for (int x = 0; x < grid.GetUpperBound(0); x++)
            {
                textOutput += grid[x, y];
            }
            textOutput += "\n";
        }
        Debug.Log(textOutput);
    }

    public static string[,] SplitCsvGrid(string csvText)
    {
        string[] lines = csvText.Split("\n\r\t"[0]);

        // finds the max width of row
        int width = 0;
        int height = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Replace("\r", "");
            string[] row = SplitCsvLine(lines[i]);
            width = Mathf.Max(width, row.Length);
            if (!string.IsNullOrEmpty(lines[i]))
            {
                height++;
            }
        }

        // creates new 2D string grid to output to
        string[,] outputGrid = new string[width, height];
        for (int y = 0; y < height; y++)
        {
            string[] row = SplitCsvLine(lines[y]);
            for (int x = 0; x < row.Length; x++)
            {
                outputGrid[x, y] = row[x];

                // This line was to replace "" with " in my output. 
                // Include or edit it as you wish.
                outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
            }
        }
        return outputGrid;
    }

    // splits a CSV row 
    static public string[] SplitCsvLine(string line)
    {
        return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
        @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
        System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value).ToArray();
    }
}