using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class ExtensionMethods
{
    public static Vector2 WorldSpaceToUiSpace(this Transform worldObject, Canvas parentCanvas)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldObject.position);
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        return movePos;
    }

    /// <summary>
    /// This has to be called from Coroutine because sizeDelta value would be wrong
    /// </summary>
    /// <param name="gridLayoutGroup"></param>
    /// <returns></returns>
    public static float FitGridCell(this GridLayoutGroup gridLayoutGroup)
    {
        RectTransform gridLayoutGroupRect = gridLayoutGroup.GetComponent<RectTransform>();
        int totalSpacesX = +gridLayoutGroup.padding.left + gridLayoutGroup.padding.right;
        totalSpacesX += (gridLayoutGroup.constraintCount - 1) * (int)gridLayoutGroup.spacing.x;
        float correctWidth = gridLayoutGroupRect.rect.width - totalSpacesX;
        float cellSize = correctWidth / gridLayoutGroup.constraintCount;
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        return cellSize;
    }

    public static void RandomShuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
    {
        return list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
    }

    public static List<string> ToCsvLines(this string fileName)
    {
        TextAsset itemCSV = Resources.Load("GameCsv/" + fileName) as TextAsset;
        List<string> linesList = Regex.Split(itemCSV.text, Environment.NewLine).ToList<string>();
        linesList.RemoveAt(0); // Remove first item as CSV has column names
        linesList.RemoveAt(linesList.Count - 1); // Warning: Remove last item as CSV one blank line at the end
        return linesList;
    }

    public static float ToFloat(this string text)
    {
        float result = 0.0f;
        float.TryParse(text, out result);
        return result;
    }

    public static int ToInt(this string text)
    {
        int num;
        if (int.TryParse(text, out num))
        {
            return num;
        }
        else
        {
            return 0;
        }
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static bool IsWithin(this int value, int minimum, int maximum)
    {
        return value > minimum && value < maximum;
    }

    public static string MinsToFormattedDuration(this int minutes)
    {
        string timeRemaining = "";
        if (minutes >= 1 && minutes < 60)
        {
            timeRemaining = minutes + "m ";
        }
        else if (minutes >= 60 && minutes < 1440)
        {
            timeRemaining = (minutes / 60) + "h " + (minutes % 60) + "m";
        }
        else if (minutes >= 1440)
        {
            int days = minutes / 60;
            timeRemaining = (days / 24) + "d " + (days % 24) + "h";
        }
        return timeRemaining;
    }

    public static string SecondsToFormattedDuration(this float seconds)
    {
        //print(string.Format("{0:0}", (int)a));
        string timeRemaining = "";
        int secs = (int)seconds;
        if (secs <= 60)
        {
            timeRemaining = secs + "s ";
        }
        else if (secs > 60)
        {
            timeRemaining = (secs / 60) + "m " + (secs % 60) + "s";
        }
        else
        {
            timeRemaining = "xx";
        }
        return timeRemaining;
    }

    public static string ToFormattedDuration(this TimeSpan timeSpan)
    {
        string timeRemaining = "";
        if (timeSpan <= new TimeSpan(360, 0, 0, 0))
        { //> 1year
            timeRemaining = timeSpan.Days.ToString() + "d " + timeSpan.Hours.ToString() + "h";
        }
        if (timeSpan <= new TimeSpan(1, 0, 0, 0))
        { //> 1day
            timeRemaining = timeSpan.Hours.ToString() + "h " + timeSpan.Minutes.ToString() + "m";
        }
        if (timeSpan <= new TimeSpan(0, 1, 0, 0))
        { //> 1hr
            timeRemaining = timeSpan.Minutes.ToString() + "m " + timeSpan.Seconds.ToString() + "s";
        }
        if (timeSpan <= new TimeSpan(0, 0, 1, 0))
        { // 1min
            timeRemaining = timeSpan.Seconds.ToString() + "s";
        }
        if (timeSpan <= new TimeSpan(0, 0, 0, 0))
        {
            timeRemaining = "";
        }
        return timeRemaining;
    }

    public static void DeepClone<T>(this T objResult, T source)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, source);
            ms.Position = 0;
            objResult = (T)bf.Deserialize(ms);
        }
    }

}
