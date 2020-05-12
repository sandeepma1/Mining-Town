using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [SerializeField] private GameObject weaponMesh;
    [SerializeField] protected float attackInterval = 0.5f;
    protected bool isAttacking = false;
    protected BoxCollider weaponCollider;

    protected virtual void Start()
    {
        weaponCollider = GetComponent<BoxCollider>();
        ToggleWeaponVisiblity(false);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {

    }

    public virtual void Attack()
    {

    }

    protected void ToggleWeaponVisiblity(bool flag)
    {
        weaponCollider.enabled = flag;
        weaponMesh.gameObject.SetActive(flag);
    }
}