using UnityEditor;
using UnityEngine;

public class BlobButtonMenuItem : MonoBehaviour
{
    private static string uiBlobButtonName = "UiBlobButton";
    [MenuItem("GameObject/UI/UiBlobButton", false, 10)]
    static void CreateUiBlobButton(MenuCommand menuCommand)
    {
        GameObject go = Instantiate((GameObject)Resources.Load(uiBlobButtonName));
        go.name = uiBlobButtonName;
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}