using System;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayerHudCanvas : MonoBehaviour
{
    public static Action<IInteractable> OnShowPlayerBubble;
    [SerializeField] private RectTransform bubbleImageRect;
    [SerializeField] private Image bubbleImage;
    [SerializeField] private UiPlayerHealthBar uiPlayerHealthBar;
    private RectTransform uiPlayerHealthBarRect;
    private Transform playerHeadPosition;
    private Canvas mainCanvas;
    private IneractableType lastIneractableType = IneractableType.None;
    private Vector2 viewportPoint;

    private void Awake()
    {
        uiPlayerHealthBarRect = uiPlayerHealthBar.GetComponent<RectTransform>();
        mainCanvas = GetComponent<Canvas>(); ;
        PlayerHealth.OnPlayerHealthBarPosition += OnPlayerHealthBarPosition;
        PlayerCurrencyManager.OnUpdateHealthText += UpdateHealthText;
        OnShowPlayerBubble += ShowPlayerBubble;
        SceneLoader.OnSceneChanged += OnSceneChanged;
    }

    private void OnDestroy()
    {
        PlayerCurrencyManager.OnUpdateHealthText -= UpdateHealthText;
        PlayerHealth.OnPlayerHealthBarPosition -= OnPlayerHealthBarPosition;
        OnShowPlayerBubble -= ShowPlayerBubble;
    }

    private void LateUpdate()
    {
        if (playerHeadPosition != null)
        {
            viewportPoint = playerHeadPosition.WorldSpaceToUiSpace(mainCanvas);
            bubbleImageRect.anchoredPosition = viewportPoint;
            uiPlayerHealthBarRect.anchoredPosition = viewportPoint;
        }
    }

    private void UpdateHealthText()
    {
        uiPlayerHealthBar.OnHealthChanged();
    }

    private void OnPlayerHealthBarPosition(Transform playerHealthBarPosition)
    {
        playerHeadPosition = playerHealthBarPosition;
    }

    private void ShowPlayerBubble(IInteractable interactable)
    {
        if (interactable == null)
        {
            bubbleImageRect.gameObject.SetActive(false);
            return;
        }
        else
        {
            bubbleImageRect.gameObject.SetActive(true);
        }

        if (lastIneractableType == interactable.GetIneractableType())
        {
            return;
        }
        switch (interactable.GetIneractableType())
        {
            case IneractableType.None:
                bubbleImageRect.gameObject.SetActive(false);
                break;
            case IneractableType.FarmItems:
                break;
            case IneractableType.Water:
                bubbleImage.sprite = AtlasBank.Instance.GetSpriteByName("fishbubble", AtlasType.UiItems);
                break;
            case IneractableType.RaisedBed:
                bubbleImage.sprite = AtlasBank.Instance.GetSpriteByName("cropbubble", AtlasType.UiItems);
                break;
            case IneractableType.ProdBuilding:
                bubbleImage.sprite = AtlasBank.Instance.GetSpriteByName("prodbuildingbubble", AtlasType.UiItems);
                break;
            case IneractableType.FruitTree:
                bubbleImage.sprite = AtlasBank.Instance.GetSpriteByName("fruittreebubble", AtlasType.UiItems);
                break;
            case IneractableType.Livestock:
                bubbleImage.sprite = AtlasBank.Instance.GetSpriteByName("livestockbubble", AtlasType.UiItems);
                break;
            default:
                break;
        }
        lastIneractableType = interactable.GetIneractableType();
    }

    private void OnSceneChanged(Scenes scenes)
    {
        switch (scenes)
        {
            case Scenes.Loading:
                break;
            case Scenes.FarmHome:
            case Scenes.Town:
                PlayerCurrencyManager.FillHealthToMax();
                uiPlayerHealthBar.Init();
                uiPlayerHealthBar.OnHealthChanged();
                break;
            case Scenes.Mines:
            case Scenes.Forest:
                uiPlayerHealthBar.Init();
                uiPlayerHealthBar.OnHealthChanged();
                break;
            default:
                break;
        }
    }
}