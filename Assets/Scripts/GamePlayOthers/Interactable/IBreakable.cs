using UnityEngine;

public interface IBreakable
{
    Transform GetTransform();
    void Hit(int damage);
}