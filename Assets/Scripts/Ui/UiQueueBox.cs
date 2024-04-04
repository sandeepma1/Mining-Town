using UnityEngine;
using UnityEngine.UI;

public class UiQueueBox : MonoBehaviour
{
    [SerializeField] private GameObject tapeImage;
    [SerializeField] private Image itemImage;
    [SerializeField] private GameObject lidImage;

    public void AddItem(string itemSlug)
    {
        tapeImage.gameObject.SetActive(true);
        lidImage.gameObject.SetActive(true);
        itemImage.sprite = AtlasBank.Instance.GetSpriteByName(itemSlug, AtlasType.UiItems);
    }

    public void RemoveItem()
    {
        tapeImage.gameObject.SetActive(false);
        lidImage.gameObject.SetActive(false);
    }
}