using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSword : MonoBehaviour
{
    private BoxCollider swordCollider;
    [SerializeField] private GameObject swordMeshCollider;
    [SerializeField] private float attackInterval = 0.5f;
    private bool isAttacking = false;
    private Vector3 swordStartRotation = new Vector3(0, -90, 0);
    private Vector3 swordEndRotation = new Vector3(0, 90, 0);

    private void Start()
    {
        swordCollider = GetComponent<BoxCollider>();
        ToggleWeaponVisiblity(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        MonsterBase monster = other.GetComponent<MonsterBase>();
        if (monster != null)
        {
            monster.TakeHit();
        }
    }

    public void Attack()
    {
        if (!isAttacking)
        {
         StartCoroutine(AttackAndWait());
        }
    }

    private IEnumerator AttackAndWait()
    {
        isAttacking = true;
        ToggleWeaponVisiblity(true);
        transform.localEulerAngles = swordEndRotation;
        transform.DOLocalRotate(swordStartRotation, attackInterval);
        //Attack animation;
        yield return new WaitForSeconds(attackInterval);
        ToggleWeaponVisiblity(false);
        isAttacking = false;
    }

    private void ToggleWeaponVisiblity(bool flag)
    {
        swordCollider.enabled = flag;
        swordMeshCollider.gameObject.SetActive(flag);
    }
}