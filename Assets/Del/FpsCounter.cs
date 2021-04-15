using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] private Text _fpsText;
    [SerializeField] private float _hudRefreshRate = 1f;

    private float _timer;

    private void Update()
    {
        _fpsText.text = (1f / Time.unscaledDeltaTime).ToString("F0");
        //_timer = Time.unscaledTime + _hudRefreshRate;
    }
}