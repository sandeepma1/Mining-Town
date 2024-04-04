using System.Collections.Generic;
using UnityEngine;
using MiningTown.IO;

public class WaterBody : MonoBehaviour, IInteractable
{
    [SerializeField] private int locationId;
    private List<FishObject> fishes = new List<FishObject>();
    [SerializeField] private Transform floaterPosition;

    private void Start()
    {
        fishes = FishDatabase.GetFishesByLocationId(locationId);
    }


    #region IInteractable
    public Vector3 GetColliderSize()
    {
        return Vector3.one;
    }

    public IneractableType GetIneractableType()
    {
        return IneractableType.Water;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void InteractOnClick()
    {
        StartFishing();
    }

    private void StartFishing()
    {
        if (!PlayerCurrencyManager.HaveEnergy(GameVariables.energy_fishing))
        {
            return;
        }
        int randomFishIndex = UnityEngine.Random.Range(0, fishes.Count);
        UiFishingCanvas.OnFishingToggle?.Invoke(true, fishes[randomFishIndex]);
    }
    #endregion
}