using System;

public static class PlayerCurrencyManager
{
    public static Action OnUpdateXpText;
    public static Action OnUpdateEnergyText;
    public static Action OnUpdateCoinsText;
    public static Action OnUpdateGemsText;
    public static Action OnUpdateHealthText;
    public static Action OnPlayerDied;
    public static Action OnPlayerHitted;

    public static int Xp
    {
        get
        {
            return SaveLoadManager.saveData.playerStats.playerXp;
        }
        set
        {
            SaveLoadManager.saveData.playerStats.playerXp = value;
            OnUpdateXpText?.Invoke();
        }
    }
    public static int Energy
    {
        get
        {
            return SaveLoadManager.saveData.playerStats.currentEnergy;
        }
        private set
        {
            SaveLoadManager.saveData.playerStats.currentEnergy = value;
            OnUpdateEnergyText?.Invoke();
        }
    }
    public static int Coins
    {
        get
        {
            return SaveLoadManager.saveData.playerStats.coins;
        }
        private set
        {
            SaveLoadManager.saveData.playerStats.coins = value;
            OnUpdateCoinsText?.Invoke();
        }
    }
    public static int Gems
    {
        get
        {
            return SaveLoadManager.saveData.playerStats.gems;
        }
        private set
        {
            SaveLoadManager.saveData.playerStats.gems = value;
            OnUpdateGemsText?.Invoke();
        }
    }
    public static int Health
    {
        get
        {
            return SaveLoadManager.saveData.playerStats.currentHealth;
        }
        private set
        {
            SaveLoadManager.saveData.playerStats.currentHealth = value;
            OnUpdateHealthText?.Invoke();
        }
    }

    public static bool ConsumeItem(Item currentSelectedItem)
    {
        if (IsEnergyMax() && IsHealthMax())
        {
            //TODO: Floating text effect saying ("Energy is max");
            return false;
        }
        AddEnergy(currentSelectedItem.energyRestore);
        AddHealth(currentSelectedItem.healthRestore);
        return true;
    }

    #region Add Currency
    public static void AddCoins(int count)
    {
        Coins += count;
    }
    public static void AddGems(int count)
    {
        Gems += count;
    }
    public static void AddXp(int count)
    {
        Xp += count;
    }
    public static void AddEnergy(int count)
    {
        Energy += count;
        if (Energy > SaveLoadManager.saveData.playerStats.maxEnergy)
        {
            Energy = SaveLoadManager.saveData.playerStats.maxEnergy;
        }
    }
    public static void AddHealth(int count)
    {
        Health += count;
        if (Health > SaveLoadManager.saveData.playerStats.maxHealth)
        {
            Health = SaveLoadManager.saveData.playerStats.maxHealth;
        }
    }
    public static void FillHealthToMax()
    {
        Health = SaveLoadManager.saveData.playerStats.maxHealth;
    }
    #endregion


    #region Check if have Currency
    public static int GetCoinCount()
    {
        return Coins;
    }
    public static int GetGemCount()
    {
        return Gems;
    }
    public static int GetEnergyCount()
    {
        return Energy;
    }
    public static bool HaveEnergy(int count)
    {
        if (count > Energy)
        {
            ShowPopupToGetEnergy();
            return false;
        }
        else
        {
            return true;
        }
    }
    public static bool IsEnergyMax()
    {
        return Energy >= SaveLoadManager.saveData.playerStats.maxEnergy;
    }
    public static bool HaveHealth(int count)
    {
        if (count > Health)
        {
            //TODO: Show player died
            //UiPlayerDiedCanvas.ShowPlayerDiedCanvas?.Invoke();
            return false;
        }
        else
        {
            return true;
        }
    }
    public static bool IsHealthMax()
    {
        return Health >= SaveLoadManager.saveData.playerStats.maxHealth;
    }
    public static bool HaveCoins(int count)
    {
        if (count > Coins)
        {
            ShowPopupToGetCoins();
            return false;
        }
        else
        {
            return true;
        }
    }
    public static bool HaveGems(int count)
    {
        if (count > Gems)
        {
            ShowPopupToGetGems();
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion


    #region Remove/Deduct Currency
    public static bool ReduceCoins(int count)
    {
        if (HaveCoins(count))
        {
            Coins -= count;
            return true;
        }
        ShowPopupToGetCoins();
        return false;
    }
    public static bool ReduceGems(int count)
    {
        if (HaveGems(count))
        {
            Gems -= count;
            return true;
        }
        ShowPopupToGetGems();
        return false;
    }
    public static bool ReduceEnergy(int count)
    {
        if (HaveEnergy(count))
        {
            Energy -= count;
            return true;
        }
        ShowPopupToGetEnergy();
        return false;
    }
    public static void ReduceHealth(int count)
    {
        if (!SaveLoadManager.saveData.playerStats.isInGodMode)
        {
            Health -= count;
            OnPlayerHitted?.Invoke();
            if (Health <= 0)
            {
                OnPlayerDied?.Invoke();
            }
        }
    }
    #endregion


    #region Popup to get currency
    public static void ShowPopupToGetCoins()
    {
        UiCommonPopupMenu.Instance.InitOkDialog(GameVariables.msg_lowOnCoins, OnOK);
    }
    public static void ShowPopupToGetGems()
    {
        UiCommonPopupMenu.Instance.InitOkDialog(GameVariables.msg_lowOnGems, OnOK);
    }
    public static void ShowPopupToGetEnergy()
    {
        UiCommonPopupMenu.Instance.InitOkDialog(GameVariables.msg_lowOnEnergy, OnOK);
    }
    private static void OnOK()
    {
        //Nothing 
    }
    #endregion
}