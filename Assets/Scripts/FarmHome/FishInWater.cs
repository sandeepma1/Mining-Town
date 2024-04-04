using UnityEngine;

public class FishInWater : MonoBehaviour, IInteractable
{
    private BoxCollider boxCollider;
    [SerializeField] private Transform floaterPosition;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public Vector3 GetColliderSize()
    {
        return boxCollider.size;
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
        UiFishingCanvas.OnFishingToggle?.Invoke(true, null);
    }
}