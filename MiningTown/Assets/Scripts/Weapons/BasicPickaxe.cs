using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BasicPickaxe : WeaponBase
{
    private Vector3 startRotation = new Vector3(0, 0, 0);
    private Vector3 endRotation = new Vector3(75, 0, 0);

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        BreakableBase breakable = other.GetComponent<BreakableBase>();
        if (breakable != null)
        {
            breakable.TakeHit();
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
        transform.localEulerAngles = startRotation;
        transform.DOLocalRotate(endRotation, attackInterval);
        //Attack animation;
        yield return new WaitForSeconds(attackInterval);
        ToggleWeaponVisiblity(false);
        isAttacking = false;
    }
}
