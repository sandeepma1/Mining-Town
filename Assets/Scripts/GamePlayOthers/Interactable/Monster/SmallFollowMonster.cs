
public class SmallFollowMonster : MonsterBase
{
    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (isPlayerDetected)
        {
            navMeshAgent.SetDestination(playerTransform.position);
        }
        else
        {
            navMeshAgent.SetDestination(transform.position);
        }
    }
}