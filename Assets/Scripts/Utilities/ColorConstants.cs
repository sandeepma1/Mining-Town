using MiningTown.Utilities;
using UnityEngine;


[CreateAssetMenu(fileName = "ColorConstants", menuName = "MiningTown/ColorConstants")]
public class ColorConstants : ScriptableSingleton<ColorConstants>
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("MiningTown/ColorConstants")]
    public static void ShowInEditor()
    {
        UnityEditor.Selection.activeObject = Instance;
    }
#endif

    [Header("In use")]
    #region Farm Home
    [SerializeField] private Color fruitTreesDead = new Color(0.75f, 1, 0);
    public static Color FruitTreesDead { get { return Instance.fruitTreesDead; } }

    [SerializeField] private Color greenTile = new Color(0, 1, 0, 0.5f);
    public static Color GreenTile { get { return Instance.greenTile; } }

    [SerializeField] private Color redTile = new Color(1, 0, 0, 0.5f);
    public static Color RedTile { get { return Instance.redTile; } }
    #endregion


    [Header("UI components")]
    [Space(20)]
    [Header("Navigation Bottom Tabs")]
    [SerializeField] private Color selectedColor = new Color(0.9339623f, 0.639063f, 0.08370414f, 1);
    public static Color SelectedColor { get { return Instance.selectedColor; } }

    [SerializeField] private Color deselectedColor = new Color(1, 1, 1, 1);
    public static Color DeselectedColor { get { return Instance.deselectedColor; } }

    [Header("Main Ui Colors")]
    #region Menu UI        
    [SerializeField] private Color primaryUiColor = new Color(1, 0.9f, 0.5f);
    public static Color PrimaryUiColor { get { return Instance.primaryUiColor; } }

    public static Color SecondaryUiColor { get { return Instance.secondaryUiColor; } }
    [SerializeField] private Color secondaryUiColor = new Color(1, 1, 1);

    public static Color ThirdUiColor { get { return Instance.thirdUiColor; } }
    [SerializeField] private Color thirdUiColor = new Color(0.83f, 0, 0, 1f);

    public static Color SelectedTabColor { get { return Instance.selectedTabColor; } }
    [SerializeField] private Color selectedTabColor = new Color(1, 0.937255f, 0.8117648f, 1);

    public static Color UnselectedTabColor { get { return Instance.unselectedTabColor; } }
    [SerializeField] private Color unselectedTabColor = new Color(0.7830189f, 0.706463f, 0.5724902f, 1);

    public static Color FishingMenuGreenColor { get { return Instance.fishingMenuGreenColor; } }
    [SerializeField] private Color fishingMenuGreenColor = new Color(0.1730941f, 0.9622642f, 0.04992881f, 1);

    public static Color FishingMenuOrangeColor { get { return Instance.fishingMenuOrangeColor; } }
    [SerializeField] private Color fishingMenuOrangeColor = new Color(1f, 0.50049f, 0f, 1);


    [Header("Not used")]
    [Space(20)]
    [SerializeField]
    private Color fpsColor = new Color(0, 0, 0, 1f);
    public static Color FpsColor { get { return Instance.fpsColor; } }
    #endregion

    #region UiBuildingMenu
    [SerializeField]
    private Color insufficientItemAmount = new Color(0, 0, 0, 1f);
    public static Color InsufficientItemAmount { get { return Instance.insufficientItemAmount; } }

    [SerializeField]
    private Color normalSecondaryText = new Color(0, 0, 0, 1f);
    public static Color NormalSecondaryText { get { return Instance.normalSecondaryText; } }
    #endregion
}
