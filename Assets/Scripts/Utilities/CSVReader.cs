using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    public static string[,] ReadCsv(string csvText)
    {
        csvText.Trim();
        string[] lines = Regex.Split(csvText, LINE_SPLIT_RE);
        Array.Reverse(lines);
        int totalColumns = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = Regex.Split(lines[i], SPLIT_RE);
            totalColumns = Mathf.Max(totalColumns, row.Length);
        }
        string[,] outputGrid = new string[totalColumns, lines.Length];
        for (int y = 0; y < lines.Length; y++)
        {
            string[] row = Regex.Split(lines[y], SPLIT_RE);
            for (int x = 0; x < row.Length; x++)
            {
                outputGrid[x, y] = row[x];
            }
        }
        return outputGrid;
    }

}