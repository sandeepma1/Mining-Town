using System.Collections;
using UnityEngine;

public class TrapLandSpikes : TrapsBase
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private float disableDuration = 1f;

    protected override void DamageGivenToPlayer()
    {
        StartCoroutine(DisableSpikesAndEnable());
    }

    private IEnumerator DisableSpikesAndEnable()
    {
        boxCollider.enabled = false;
        yield return new WaitForSeconds(disableDuration);
        boxCollider.enabled = true;
    }
}