using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Player_Movement : MonoBehaviour
{
    Transform target;

    CharacterCombat combat;

    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        combat = GetComponent<CharacterCombat>();

        agent.updateRotation = false;
    }

    private void Update()
    {
        if (combat.GetMyStats().dead) return;

        agent.speed = combat.GetMyStats().moveSpeed.GetValue();

        if (combat.CastTime > 0 || combat.rooted)
        {
            target = null;
        }
        if (combat.GetAttackCooldown() > 0)
        {
            agent.velocity = Vector3.zero;
        }

        if (target != null)
        {
            if (combat.cc_Effects.Count == 0)
            {
                agent.SetDestination(target.position);
                //agent.angularSpeed = 100f;
            }

            else
            {
                foreach (CC_Effect effect in combat.cc_Effects)
                {
                    if (effect.affect != StatusEffects.Root)
                    {
                        agent.SetDestination(target.position);
                        //agent.angularSpeed = 120f;
                    }
                }
            }

            FaceTarget();
        }

    }

    private void LateUpdate()
    {
        if (agent.velocity.sqrMagnitude > Mathf.Epsilon && combat.CastTime < 0)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }
    }

    public void MovetoPoint(Vector3 newPoint)
    {
        agent.stoppingDistance = 2.0f;
        agent.SetDestination(newPoint);
        if(combat.GetAttackCooldown() > 0)
            combat.SetAttackCooldown(0.0f);
    }

    public void FollowTarget(Interactable newTarget)
    {
        agent.stoppingDistance = newTarget.radius;

        if (combat.attackRange > 0 && newTarget.TryGetComponent(out Enemy enemyInteractable))
            agent.stoppingDistance = combat.attackRange - 0.2f;


        agent.updateRotation = false;
        target = newTarget.interactionTransform;
        if (combat.GetAttackCooldown() > 0)
            combat.SetAttackCooldown(0.0f);
    }

    public void FollowTarget(GameObject newTarget, float stopDistance)
    {
        agent.stoppingDistance = stopDistance;
        target = newTarget.transform;
    }

    public void StopFollowTarget()
    {
        agent.stoppingDistance = 0f;
        target = null;
        //agent.velocity = Vector3.zero;
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
