using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [SerializeField] private GameObject weaponMesh;
    [SerializeField] protected BoxCollider weaponCollider;
    protected bool isAttacking = false;


    protected virtual void Start()
    {
        ToggleWeaponCollider(false);
        ToggleWeaponVisiblity(false);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {

    }

    public virtual void Attack()
    {

    }

    public void ToggleWeaponVisiblity(bool flag)
    {
        weaponMesh.gameObject.SetActive(flag);
    }

    protected void ToggleWeaponCollider(bool flag)
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = flag;
        }
    }
}