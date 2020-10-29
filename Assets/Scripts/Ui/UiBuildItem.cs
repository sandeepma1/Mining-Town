using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UiBuildItem : MonoBehaviour
{
    public Action<Source> OnSourceButtonClick;
    public Action<DecorationInfo> OnDecorationButtonClick;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Button button;
    [SerializeField] private Image mainImage;
    [SerializeField] private TextMeshProUGUI currencyCount;
    private Source source;
    private DecorationInfo decorationInfo;

    public void InitSource(Source source)
    {
        this.source = source;
        button.onClick.AddListener(OnSourceButtonPressed);
        headerText.text = source.name;
        mainImage.sprite = AtlasBank.Instance.GetSpriteByName(source.slug, AtlasType.UiBuildMenu);

        //This will always ask for coins 
        currencyCount.text = GameVariables.tmp_coinIcon + source.buildCost;
    }

    public void InitDecoration(DecorationInfo decorationInfo)
    {
        this.decorationInfo = decorationInfo;
        button.onClick.AddListener(OnDecorationButtonPressed);
        headerText.text = decorationInfo.name;

        mainImage.sprite = AtlasBank.Instance.GetSpriteByName(decorationInfo.slug, AtlasType.UiBuildMenu);
        if (decorationInfo.buildCostGems == 0)//This will always ask for coins 
        {
            currencyCount.text = GameVariables.tmp_coinIcon + decorationInfo.buildCostCoins;
        }
        else//This will always ask for Gems 
        {
            currencyCount.text = GameVariables.tmp_gemIcon + decorationInfo.buildCostGems.ToString();
        }

    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnSourceButtonPressed);
        button.onClick.RemoveListener(OnDecorationButtonPressed);
    }

    private void OnSourceButtonPressed()
    {
        OnSourceButtonClick?.Invoke(source);
    }

    private void OnDecorationButtonPressed()
    {
        OnDecorationButtonClick?.Invoke(decorationInfo);
    }
}