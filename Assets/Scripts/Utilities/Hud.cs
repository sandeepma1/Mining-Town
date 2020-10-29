using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Hud : Singleton<Hud>
{
    public static Action<string> SetHudText;
    [SerializeField] private RectTransform safeFrame;
    [SerializeField] private bool showDebug;
    private static bool ShowDebug { get { return Instance.showDebug; } set { Instance.showDebug = value; } }
    [SerializeField] private bool showFps;
    private static bool ShowFps { get { return Instance.showFps; } set { Instance.showFps = value; } }
    private static Text debugText;
    private static Text fpsText;
    private const float fpsCalculateFrequency = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        debugText = transform.GetChild(0).GetComponent<Text>();
        fpsText = transform.GetChild(1).GetComponent<Text>();
        SetHudText += PrintDebugText;
        if (ShowFps)
        {
            StartCoroutine(FPS());
        }
        Rect safeRect = Screen.safeArea;
        safeFrame.rect.Set(safeRect.x, safeRect.y, safeRect.width, safeRect.height);
    }

    private void OnDestroy()
    {
        SetHudText += PrintDebugText;
    }

    private static void PrintDebugText(string text)
    {
        if (ShowDebug)
        {
            debugText.text = text;
        }
    }

    private IEnumerator FPS()
    {
        for (; ; )
        {
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(fpsCalculateFrequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;
            fpsText.text = Mathf.RoundToInt(frameCount / timeSpan).ToString() + " fps";
        }
    }
}