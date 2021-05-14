using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Controller : MonoBehaviour
{
    Enemy enemyInteractor;

    PlayerManager playerManager;
    public float lookRadius = 10f;

    public EnemyAbilityBehaviour[] abilityBehaviors = new EnemyAbilityBehaviour[3];

    float basicAttackRange;

    Transform target;
    NavMeshAgent agent;
    CharacterCombat combat;
    Character_Stats stats;

    Enemy_Detection detector;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.instance;
        agent = GetComponent<NavMeshAgent>();
        combat = GetComponent<CharacterCombat>();
        stats = GetComponent<Character_Stats>();
        enemyInteractor = GetComponent<Enemy>();

        detector = GetComponentInChildren<Enemy_Detection>();
        detector.GetComponent<SphereCollider>().radius = lookRadius-1f;
        detector.GetComponent<SphereCollider>().isTrigger = true;

        agent.stoppingDistance = enemyInteractor.radius;

        agent.updateRotation = false;

        basicAttackRange = combat.attackRange;

        if (basicAttackRange < agent.stoppingDistance)
            basicAttackRange = agent.stoppingDistance;

        agent.speed = stats.moveSpeed.GetValue();
    }

    // Update is called once per frame
    void Update()
    {
        if (stats.dead) return;
        if (playerManager.gameOver) return;
        agent.speed = stats.moveSpeed.GetValue();

        if (combat.CastTime > 0 || combat.rooted)
        {
            target = null;
            agent.velocity = Vector3.zero;
        }
        if (combat.GetAttackCooldown() > 0)
        {
            agent.velocity = Vector3.zero;
        }

        if (detector.target != null)
        {
            target = detector.target;
        }
        else return;

        FaceTarget(target.position);

        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);
        }

        for (int i = 0; i < stats.abilities.Count; i++)
        {
            AbilityCasting(i);
        }
               
        BasicAttacking(distance);
    }


    void BasicAttacking(float distance)
    {   
        if (distance <= basicAttackRange)
        {
            //attack
            Character_Stats targetStats = target.GetComponent<Character_Stats>();
            if (targetStats != null && combat.CastTime < 0)
            {
                combat.Attack(targetStats);
            }
        }
        else
            GetComponent<CharacterAnimator>().characterAnim.SetBool("basicAttack", false);
        
    }

    void AbilityCasting(int num)
    {
        switch (abilityBehaviors[num])
        {
            case EnemyAbilityBehaviour.Less_Than_75_Percent_HP:
                if(stats.curHP <= stats.maxHP.GetValue() * 0.75)
                {
                    CastAbility(stats.abilities[num]);
                }
                break;
            case EnemyAbilityBehaviour.Less_Than_50_Percent_HP:
                if (stats.curHP <= stats.maxHP.GetValue() * 0.50)
                {
                    CastAbility(stats.abilities[num]);
                }
                break;
            case EnemyAbilityBehaviour.Less_Than_25_Percent_HP:
                if (stats.curHP <= stats.maxHP.GetValue() * 0.25)
                {
                    CastAbility(stats.abilities[num]);
                }
                break;
            case EnemyAbilityBehaviour.Any_Time_Off_Cooldown:
                CastAbility(stats.abilities[num]);
                break;
            case EnemyAbilityBehaviour.Player_Enemies_Nearby:
                Collider[] alliesNear = Physics.OverlapSphere(transform.position, stats.abilities[num].GetRange());
                int allAlliesNear = 0;
                foreach(Collider ally in alliesNear)
                {
                    if(ally.tag == "Enemy")
                    {
                        allAlliesNear++;
                    }
                }
                if(allAlliesNear >= 2)
                {
                    CastAbility(stats.abilities[num]);
                }
                break;

            case EnemyAbilityBehaviour.Player_Enemy_In_Danger:
                Collider[] checkAllyHealth = Physics.OverlapSphere(transform.position, stats.abilities[num].GetRange());
                foreach (Collider ally in checkAllyHealth)
                {
                    if (ally.tag == "Enemy")
                    {
                        Character_Stats allyStats = ally.GetComponent<Character_Stats>();
                        if(allyStats.curHP <= allyStats.maxHP.GetValue() * 0.25)
                        {
                            CastAbility(stats.abilities[num]);
                        }
                    }
                }
                break;
        }
    }

    void CastAbility(Ability ability)
    {
        if (ability.cooldownTimer > 0)
            return;
        float distance;
        switch (ability.GetTargetType())
        {
            case TargetType.Self:
                ability.Use(gameObject);
                break;

            case TargetType.PlayerEnemy:
                Collider[] alliesNear = Physics.OverlapSphere(transform.position, ability.GetRange());
                foreach (Collider ally in alliesNear)
                {
                    if (ally.tag == "Enemy" && ally.gameObject != gameObject)
                    {
                        ability.SetTarget(ally.transform.position, ally.GetComponent<CharacterCombat>());
                        ability.Use(gameObject);
                    }
                }
                break;

            default:
                distance = Vector3.Distance(target.position, transform.position);
                if (distance <= ability.GetRange())
                {
                    CharacterCombat targetCombat = target.GetComponent<CharacterCombat>();
                    if (targetCombat != null && combat.CastTime < 0)
                    {
                        ability.SetTarget(targetCombat.transform.position, targetCombat);
                        ability.Use(gameObject);
                    }
                }
                break;
        }
    }


    //make sure to face target when attacking
    void FaceTarget(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 2;
        Gizmos.DrawRay(transform.position, direction);
    }

   
}

public enum EnemyAbilityBehaviour
{
    Any_Time_Off_Cooldown,
    Less_Than_75_Percent_HP, 
    Less_Than_50_Percent_HP, 
    Less_Than_25_Percent_HP, 
    Player_Enemies_Nearby,
    Player_Enemy_In_Danger,
}