using UnityEngine;

public interface IInteractable
{
    void InteractOnClick();
    Transform GetTransform();
    Vector3 GetColliderSize();
    IneractableType GetIneractableType();
}

public enum IneractableType
{
    None,
    FarmItems,
    Water,
    RaisedBed,
    ProdBuilding,
    FruitTree,
    Livestock
}