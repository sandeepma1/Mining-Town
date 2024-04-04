using UnityEngine;

public interface IChopable
{
    Transform GetTransform();
    void Hit(int damage);
}