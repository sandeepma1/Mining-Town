using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UiResourceSpawn : MonoBehaviour
{
    [SerializeField] private Image glowImage;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private RectTransform rectTransform;
    private Vector2 glowSizeMax = new Vector2(200, 200);
    private const float animDuration = 1f;

    public void InitItem(string slug, Transform worldObject, Canvas mainCanvas, Vector2 endPos, Camera camera, int count)
    {
        itemImage.sprite = AtlasBank.Instance.GetSpriteByName(slug, AtlasType.UiItems);
        this.countText.text = "+" + count;
        glowImage.GetComponent<RectTransform>().DOSizeDelta(glowSizeMax, animDuration);
        glowImage.DOFade(0, animDuration);

        rectTransform.anchoredPosition = worldObject.WorldSpaceToUiSpace(mainCanvas);
        rectTransform.DOAnchorPos(endPos, animDuration).SetEase(Ease.InExpo).OnComplete(() => Destroy(gameObject));
    }
}