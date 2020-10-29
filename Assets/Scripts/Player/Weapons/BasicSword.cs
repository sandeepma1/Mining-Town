using System.Collections;
using UnityEngine;

public class BasicSword : WeaponBase
{
    [SerializeField] private ParticleSystem trailParticles;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        MonsterBase monster = other.GetComponent<MonsterBase>();
        if (monster != null)
        {
            monster.TakeHit(2);
        }
    }

    public override void Attack()
    {
        base.Attack();
        if (!isAttacking)
        {
            StartCoroutine(AttackAndWait());
            //StartCoroutine(PlayParticeAndWait());
        }
    }

    private IEnumerator AttackAndWait()
    {
        isAttacking = true;
        ToggleWeaponCollider(isAttacking);
        trailParticles.Play();
        yield return new WaitForSeconds(GameVariables.swordHitRate);
        trailParticles.Stop();
        isAttacking = false;
        ToggleWeaponCollider(isAttacking);
    }

    //private IEnumerator PlayParticeAndWait()
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    trailParticles.Play();

    //    yield return new WaitForSeconds(GameVariables.swordHitRate - 0.2f);

    //    trailParticles.Stop();
    //}
}