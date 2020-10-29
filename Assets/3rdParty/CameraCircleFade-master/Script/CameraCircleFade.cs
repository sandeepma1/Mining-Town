using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[ExecuteInEditMode]
public class CameraCircleFade : MonoBehaviour
{
    [SerializeField] private Button fadeIn;
    [SerializeField] private Button fadeOut;
    private Material _mtl;
    private float maxCircle = 1.2f;

    Material mtl
    {
        get
        {
            if (_mtl) return _mtl;
            else
            {
                _mtl = new Material(Shader.Find("Hidden/Circle"));
                return _mtl;
            }
        }
    }

    private void Start()
    {
        fadeIn.onClick.AddListener(FaceIn);
        fadeOut.onClick.AddListener(FaceOut);
    }

    private void OnDestroy()
    {
        fadeIn.onClick.RemoveListener(FaceIn);
        fadeOut.onClick.RemoveListener(FaceOut);
    }

    private void FaceIn()
    {
        mtl.DOFloat(maxCircle, "_Radius", 0.5f);
    }

    private void FaceOut()
    {
        mtl.DOFloat(0, "_Radius", 0.5f);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, mtl);
    }
}