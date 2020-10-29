using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class DatabaseBase
{
    public static List<string> GetAllLinesFromCSV(string fileName)
    {
        TextAsset itemCSV = Resources.Load("GameCsv/" + fileName) as TextAsset;
        List<string> linesList = Regex.Split(itemCSV.text, "\r\n").ToList<string>();
        linesList.RemoveAt(0); // Remove first item as CSV has column names
        linesList.RemoveAt(linesList.Count - 1); // Warning: Remove last item as CSV one blank line at the end
        return linesList;
    }
}