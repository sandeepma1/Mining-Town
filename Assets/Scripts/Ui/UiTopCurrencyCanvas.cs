using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class UiTopCurrencyCanvas : Singleton<UiTopCurrencyCanvas>
{
    public static Action OnAddEnergyButtonClicked;
    public static Action OnAddCoinsButtonClicked;
    public static Action OnAddGemsButtonClicked;
    [SerializeField] private Button addEnergyButton;
    [SerializeField] private Button addCoinsButton;
    [SerializeField] private Button addGemsButton;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI energyTimerText;
    // [SerializeField] private RectTransform energyBarBGRect;
    // [SerializeField] private RectTransform energyBarRect;
    private float energyBarWidth;
    //[SerializeField] private TextMeshProUGUI energyMinusText;
    [SerializeField] private TextMeshProUGUI coinsText;
    //[SerializeField] private TextMeshProUGUI coinsMinusText;
    [SerializeField] private TextMeshProUGUI gemsText;
    //[SerializeField] private TextMeshProUGUI gemsMinusText;
    private TimeSpan timeSpan;
    private int lessMaxEnergy;

    private IEnumerator Start()
    {
        PlayerCurrencyManager.OnUpdateCoinsText += UpdateCoinsText;
        PlayerCurrencyManager.OnUpdateGemsText += UpdateGemsText;
        PlayerCurrencyManager.OnUpdateXpText += UpdateXpText;
        PlayerCurrencyManager.OnUpdateEnergyText += UpdateEnergyText;

        addEnergyButton.onClick.AddListener(OnAddEnergyButtonClick);
        addCoinsButton.onClick.AddListener(OnAddCoinsButtonClick);
        addGemsButton.onClick.AddListener(OnAddGemsButtonClick);
        UpdateAllCurrency();
        InitEnergyFill();
        //Get streched rect after end of frame
        yield return new WaitForEndOfFrame();
        // energyBarWidth = energyBarBGRect.rect.width - 5;
        UpdateEnergyText();
    }

    private void OnDestroy()
    {
        PlayerCurrencyManager.OnUpdateCoinsText -= UpdateCoinsText;
        PlayerCurrencyManager.OnUpdateGemsText -= UpdateGemsText;
        PlayerCurrencyManager.OnUpdateXpText -= UpdateXpText;
        PlayerCurrencyManager.OnUpdateEnergyText -= UpdateEnergyText;

        addEnergyButton.onClick.RemoveListener(OnAddEnergyButtonClick);
        addCoinsButton.onClick.RemoveListener(OnAddCoinsButtonClick);
        addGemsButton.onClick.RemoveListener(OnAddGemsButtonClick);
    }

    private void Update()
    {
        if (IsEnergyMax())
        {
            return;
        }
        timeSpan = SaveLoadManager.saveData.playerStats.nextEnergyDateTime.Subtract(DateTime.UtcNow);
        if (timeSpan.TotalSeconds <= 0)
        {
            if (!IsEnergyMax())
            {
                PlayerCurrencyManager.AddEnergy(1);
                SaveLoadManager.saveData.playerStats.nextEnergyDateTime = DateTime.UtcNow.AddSeconds(GameVariables.energyNewAddSeconds);
                timeSpan = SaveLoadManager.saveData.playerStats.nextEnergyDateTime.Subtract(DateTime.UtcNow);
            }
            else
            {
                energyTimerText.text = "";
            }
        }
        else
        {
            energyTimerText.text = timeSpan.ToFormattedDuration();
        }
    }

    private void InitEnergyFill()
    {
        if (IsEnergyMax())
        {
            return;
        }
        SaveLoadManager.saveData.playerStats.nextEnergyDateTime =
             JsonUtility.FromJson<JsonDateTime>(SaveLoadManager.saveData.playerStats.nextEnergyDateTimeJson);

        lessMaxEnergy = SaveLoadManager.saveData.playerStats.maxEnergy - SaveLoadManager.saveData.playerStats.currentEnergy;
        for (int i = 0; i < lessMaxEnergy; i++)
        {
            timeSpan = SaveLoadManager.saveData.playerStats.nextEnergyDateTime.Subtract(DateTime.UtcNow);
            if (timeSpan.TotalSeconds <= 0)
            {
                if (!IsEnergyMax())
                {
                    PlayerCurrencyManager.AddEnergy(1);
                    DateTime tempDateTime = new DateTime(SaveLoadManager.saveData.playerStats.nextEnergyDateTime.AddSeconds(GameVariables.energyNewAddSeconds).Ticks);
                    SaveLoadManager.saveData.playerStats.nextEnergyDateTime = tempDateTime;
                }
                else
                {
                    break;
                }
            }
            else
            {
                energyTimerText.text = timeSpan.ToFormattedDuration();
            }
        }
    }

    private bool IsEnergyMax()
    {
        if (SaveLoadManager.saveData.playerStats.currentEnergy >= SaveLoadManager.saveData.playerStats.maxEnergy)
        {
            energyTimerText.text = "";
            //SaveLoadManager.saveData.playerStats.nextEnergyDateTime = DateTime.UtcNow;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnAddEnergyButtonClick()
    {
        OnAddEnergyButtonClicked?.Invoke();
        PlayerCurrencyManager.AddEnergy(20);
    }
    private void OnAddCoinsButtonClick()
    {
        OnAddCoinsButtonClicked?.Invoke();
        PlayerCurrencyManager.AddCoins(1000);
    }
    private void OnAddGemsButtonClick()
    {
        OnAddGemsButtonClicked?.Invoke();
        PlayerCurrencyManager.AddGems(100);
    }


    private void UpdateAllCurrency()
    {
        UpdateXpText();
        UpdateEnergyText();
        UpdateCoinsText();
        UpdateGemsText();
    }
    public void UpdateXpText()
    {
        // xpText.text = SaveLoadManager.saveData.playerStats.playerXp.ToString();
    }
    public void UpdateEnergyText()
    {
        float energy = (float)SaveLoadManager.saveData.playerStats.currentEnergy /
                       SaveLoadManager.saveData.playerStats.maxEnergy;
        // energyBarRect.sizeDelta = new Vector2(energyBarWidth * energy, energyBarRect.sizeDelta.y);
        energyText.text = SaveLoadManager.saveData.playerStats.currentEnergy.ToString();
    }
    public void UpdateCoinsText()
    {
        coinsText.text = SaveLoadManager.saveData.playerStats.coins.ToString();
    }
    public void UpdateGemsText()
    {
        gemsText.text = SaveLoadManager.saveData.playerStats.gems.ToString();
    }
}