using UnityEngine;
public interface IInteractable
{
    //InteractableType GetInteractableType();
    Transform GetTransform();

    float GetDamageValue();
}

public enum InteractableType
{
    Monster,
    Breakable
}