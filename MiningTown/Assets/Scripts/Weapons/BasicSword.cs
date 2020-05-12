using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BasicSword : WeaponBase
{
    private Vector3 swordStartRotation = new Vector3(0, -90, 0);
    private Vector3 swordEndRotation = new Vector3(0, 90, 0);

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        MonsterBase monster = other.GetComponent<MonsterBase>();
        if (monster != null)
        {
            monster.TakeHit();
        }
    }

    public override void Attack()
    {
        base.Attack();
        if (!isAttacking)
        {
            StartCoroutine(AttackAndWait());
        }
    }

    private IEnumerator AttackAndWait()
    {
        isAttacking = true;
        ToggleWeaponVisiblity(true);
        //Attack animation;
        transform.localEulerAngles = swordEndRotation;
        transform.DOLocalRotate(swordStartRotation, attackInterval);
        //Attack animation;
        yield return new WaitForSeconds(attackInterval);
        ToggleWeaponVisiblity(false);
        isAttacking = false;
    }
}