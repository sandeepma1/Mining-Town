using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiningTown.IO;
using System;

public class UiBuildCanvas : MonoBehaviour
{
    public static Action<ProdBuildingData> OnAddNewProdBuilding;
    public static Action<LivestockData> OnAddNewLivestockBuilding;
    public static Action<RaisedBedData> OnAddNewRaisedBed;
    public static Action<FruitTreeData> OnAddFruitTree;
    public static Action<DecorationData> OnAddNewDecoration;
    public static Action<bool> OnIsInBuildMode;
    [Header("Child Class")]
    [Space(10)]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button closeButtonBackground;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private UiBuildItem uiBuildItemPrefab;
    [SerializeField] private UiTabButton[] tabButtons;
    [SerializeField] private ScrollRect mainScrollView;
    [SerializeField] private RectTransform[] contents;
    private Dictionary<int, Source> sources = new Dictionary<int, Source>();
    private Dictionary<int, DecorationInfo> decorations = new Dictionary<int, DecorationInfo>();
    private int lastClickedTabId;
    private int reduceCoinsOnBuild;
    private int reduceGemsOnBuild;

    private void Start()
    {
        closeButton.onClick.AddListener(CloseCanvas);
        closeButtonBackground.onClick.AddListener(CloseCanvas);
        sources = SourceDatabase.GetAllSources();
        decorations = DecorationDatabase.GetAllDecorations();
        InitBuildMenuItems();
        InitTabButtons();
        //Trick to solve one bug
        OnTabButtonsClicked(1);
        OnTabButtonsClicked(0);
        ToggleCanvas(false);
        UiAllScreenButtonsCanvas.OnBuildButtonClick += ShowCanvas;
        UiBuildingEditModeCanvas.OnBuildOkButtonClick += OnBuildOkButtonClick;
        UiBuildingEditModeCanvas.OnBuildCancelButtonClick += OnBuildCancelButtonClick;
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(CloseCanvas);
        closeButtonBackground.onClick.RemoveListener(CloseCanvas);
        UiAllScreenButtonsCanvas.OnBuildButtonClick -= ShowCanvas;
        UiBuildingEditModeCanvas.OnBuildOkButtonClick -= OnBuildOkButtonClick;
        UiBuildingEditModeCanvas.OnBuildCancelButtonClick -= OnBuildCancelButtonClick;
    }

    private void OnBuildOkButtonClick()
    {
        print("Build placed, reduce currency");
        PlayerCurrencyManager.ReduceCoins(reduceCoinsOnBuild);
        PlayerCurrencyManager.ReduceGems(reduceGemsOnBuild);
        reduceCoinsOnBuild = 0;
        reduceGemsOnBuild = 0;
        OnIsInBuildMode?.Invoke(false);
        StartCoroutine(ShowBuildModeDelay());
    }

    private IEnumerator ShowBuildModeDelay()
    {
        yield return new WaitForEndOfFrame();
        UiAllScreenButtonsCanvas.OnToggleAllButtons?.Invoke(false);
        ShowCanvas();
    }

    private void OnBuildCancelButtonClick()
    {
        print("Cancel, done place anyting");
        reduceCoinsOnBuild = 0;
        reduceGemsOnBuild = 0;
        OnIsInBuildMode?.Invoke(false);
    }

    private void InitBuildMenuItems()
    {
        //Init sources
        foreach (KeyValuePair<int, Source> item in sources)
        {
            UiBuildItem uiBuildItem = Instantiate(uiBuildItemPrefab, contents[item.Value.categoryId]);
            uiBuildItem.InitSource(item.Value);
            uiBuildItem.OnSourceButtonClick += OnSourceButtonClick;
        }

        //Init Decoration items
        foreach (KeyValuePair<int, DecorationInfo> item in decorations)
        {
            UiBuildItem uiBuildItem = Instantiate(uiBuildItemPrefab, contents[3]);
            uiBuildItem.InitDecoration(item.Value);
            uiBuildItem.OnDecorationButtonClick += OnDecorationButtonClick;
        }
    }


    #region Tab/Navigation buttons stuff
    private void InitTabButtons()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].Init(i);
            tabButtons[i].OnTabButtonClicked += OnTabButtonsClicked;
        }
    }

    private void OnTabButtonsClicked(int tabId)
    {
        if (lastClickedTabId == tabId)
        {
            return;
        }
        //Change Tab colors
        tabButtons[lastClickedTabId].ToggleButtonPressed(ColorConstants.UnselectedTabColor, false);
        tabButtons[tabId].ToggleButtonPressed(ColorConstants.SelectedTabColor, true);

        //Change contents
        contents[lastClickedTabId].gameObject.SetActive(false);
        contents[tabId].gameObject.SetActive(true);
        mainScrollView.content = contents[tabId];

        lastClickedTabId = tabId;
    }
    #endregion


    #region  Build the actual building/trees/source/decoraion
    private void OnDecorationButtonClick(DecorationInfo decorationInfo)
    {
        if (decorationInfo.buildCostGems == 0) //this is coins
        {
            if (PlayerCurrencyManager.HaveCoins(decorationInfo.buildCostCoins))
            {
                reduceCoinsOnBuild = decorationInfo.buildCostCoins;
                DecorationData decorationData = new DecorationData(decorationInfo.id, GetCenterPositionOfGround());
                OnAddNewDecoration?.Invoke(decorationData);
                HideBuildMenuAndEnableEditMode();
            }
        }
        else //this is gems
        {
            if (PlayerCurrencyManager.HaveCoins(decorationInfo.buildCostGems))
            {
                reduceGemsOnBuild = decorationInfo.buildCostGems;
                DecorationData decorationData = new DecorationData(decorationInfo.id, GetCenterPositionOfGround());
                OnAddNewDecoration?.Invoke(decorationData);
                HideBuildMenuAndEnableEditMode();
            }
        }
    }

    private void OnSourceButtonClick(Source source)
    {
        //Source will always ask coins
        if (!PlayerCurrencyManager.HaveCoins(source.buildCost))
        {
            return;
        }
        reduceCoinsOnBuild = source.buildCost;

        switch (source.categoryId)
        {
            case 0: //Mine space
                ProdBuildingData prodBuildingData = new ProdBuildingData(GetCenterPositionOfGround(), source.id, 2);
                OnAddNewProdBuilding?.Invoke(prodBuildingData);
                break;
            case 1: //Farm space
                if (source.id == 1) //This will always be Raised Bed
                {
                    RaisedBedData raisedBedData = new RaisedBedData(GetCenterPositionOfGround());
                    OnAddNewRaisedBed?.Invoke(raisedBedData);
                }
                else//This will be Chicken Coop, Cow shed or any animal building
                {
                    LivestockData livestockData = new LivestockData(GetCenterPositionOfGround(), source.id, 2);
                    OnAddNewLivestockBuilding?.Invoke(livestockData);
                }
                break;
            case 2: //Orchard Space
                FruitTreeData fruitTreeData = new FruitTreeData(GetCenterPositionOfGround(), source.id);
                OnAddFruitTree?.Invoke(fruitTreeData);
                break;
            default:
                break;
        }
        HideBuildMenuAndEnableEditMode();
    }

    private Vector3 GetCenterPositionOfGround()
    {
        Vector3 pos = PlayerMovement.Instance.GetPlayerPosition();
        return new Vector3(pos.x, pos.y, pos.z - 5);
    }
    #endregion


    #region Show Hide Canvas
    private void HideBuildMenuAndEnableEditMode()
    {
        ToggleCanvas(false);
        OnIsInBuildMode?.Invoke(true);
        UiAllScreenButtonsCanvas.OnEditModeButtonClick?.Invoke();
    }

    private void ShowCanvas()
    {
        ToggleCanvas(true);
    }

    private void CloseCanvas()
    {
        ToggleCanvas(false);
        UiAllScreenButtonsCanvas.OnToggleAllButtons?.Invoke(true);
    }

    private void ToggleCanvas(bool isVisible)
    {
        mainPanel.SetActive(isVisible);
        closeButtonBackground.gameObject.SetActive(isVisible);
    }
    #endregion
}