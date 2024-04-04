public class Decoration : MovableUnitBase
{
    private DecorationData decorationData;

    public void InitDecoration(DecorationData decorationData, bool isNewelyPlaced)
    {
        this.decorationData = decorationData;
        transform.position = decorationData.pos;
        if (isNewelyPlaced)
        {
            isInBuildMenu = true;
            EnableEditMode();
        }
    }


    #region Save Decoration Building position
    protected override void AddThisNewBuilding()
    {
        base.AddThisNewBuilding();
        SaveLoadManager.AddDecoration(decorationData);
    }

    protected override void SavePositionData()
    {
        base.SavePositionData();
        decorationData.pos = transform.position;
    }
    #endregion
}