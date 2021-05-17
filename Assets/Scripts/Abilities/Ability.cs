using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Ability : ScriptableObject
{
    public string description = "ability does thing";
    protected GameObject abilityUser = null;
    public Sprite icon = null;

    public List<BufforDebuff> buffList = new List<BufforDebuff>();

    public List<CC_Effect> cc_Effects = new List<CC_Effect>();

    [SerializeField]
    protected GameObject projectile;

   [SerializeField]
    protected float delay;

    [SerializeField]
    protected float range;

    [SerializeField]
    protected CC_Displacement displacement;

    [SerializeField]
    protected bool doesDamage, doesHealing;

    [SerializeField]
    protected float abilityValue;

    [SerializeField]
    private float cooldown;

    [SerializeField]
    protected TargetType targetType;

    protected Vector3 displacePos;

    protected Vector3 projectileSpawnPos;

    public string animatorTrigger;

    public GameObject spawnParticleEffect;

    protected GameObject particleEffectActivate;

    public float castTime;

    [HideInInspector]
    public float cooldownTimer = 0;

    protected Camera cam;

    protected UnityEvent<CharacterCombat> OnAbilityUse = new UnityEvent<CharacterCombat>();

    public virtual void Use(GameObject interactor)
    {
        abilityUser = interactor;

        displacePos = interactor.transform.position;
    }

    private void SetupListeners()
    {
        OnAbilityUse.RemoveAllListeners();

        if (doesDamage) OnAbilityUse.AddListener(Damage);
        if (doesHealing) OnAbilityUse.AddListener(Heal);
        if (buffList != null)
        {
            if(buffList.Count > 0)
                OnAbilityUse.AddListener(addBuff);
        }
        if (cc_Effects != null)
        {
            if (cc_Effects.Count > 0)
                OnAbilityUse.AddListener(addCC_Effect); 
        }
        if (displacement.distance != 0) OnAbilityUse.AddListener(addDisplacement);

        if (particleEffectActivate != null)
        {
            OnAbilityUse.AddListener(ActivateParticleEffect);
        }


        Debug.Log("Listener Setup completed");
    }

    //Checks if the cooldown is below, if not then nothing happens
    protected bool Conditions(GameObject interactor)
    {
        CharacterCombat combat = interactor.GetComponent<CharacterCombat>();
        CharacterAnimator anim = abilityUser.GetComponent<CharacterAnimator>();

        if (cooldownTimer >= 0)
        {
            Debug.Log("Ability on cooldown");
            return false;
        }
        if (combat.CastTime >= 0)
        {
            Debug.Log("Casting another Ability");
            return false;
        }
        if (combat.silenced)
        {
            Debug.Log(interactor.name + " is using is silenced");
            return false;
        }

        if (anim != null)
        {
            anim.characterAnim.SetTrigger(animatorTrigger);
        }

        cooldownTimer = cooldown;

        combat.CastTime = castTime;
        combat.SetAttackCooldown(castTime); //+ (1f / combat.attackSpeed));


        SetupListeners();
        Debug.Log("Condition Setup completed");
        return true;
    }

    protected virtual Vector3 FindTargetWithMouse(float maxCastDistance)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxCastDistance))
        {
            Vector3 direction = (hit.point - abilityUser.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            abilityUser.transform.rotation = lookRotation;

            return hit.point;
        }

        else return Vector3.zero;
    }

    private void Damage(CharacterCombat target)
    {
        abilityUser.GetComponent<CharacterCombat>().AbilityHit(target.GetMyStats(), abilityValue);
    }

    private void Heal(CharacterCombat target)
    {
        abilityUser.GetComponent<CharacterCombat>().AbilityHeal(target.GetMyStats(), abilityValue);
    }

    private void addBuff(CharacterCombat target)
    {
        Character_Stats statsAffected = target.GetMyStats();

        foreach (BufforDebuff buff in buffList)
        {
            buff.durationTimer = buff.duration;
            switch (buff.affects)
            {
                case StatBuffs.Damage:
                    statsAffected.damage.AddModifier(buff.amount);
                    if (target.damageBuffIndicator != null) target.damageBuffIndicator.SetActive(true);
                    break;
                case StatBuffs.Armor:
                    statsAffected.armor.AddModifier(buff.amount);
                    if (target.armorBuffIndicator != null) target.armorBuffIndicator.SetActive(true);
                    break;
                case StatBuffs.MoveSpeed:
                    statsAffected.moveSpeed.AddModifier(buff.amount);
                    break;
                case StatBuffs.AttackSpeed:
                    statsAffected.attackSpeed.AddModifier(buff.amount);
                    break;
                case StatBuffs.Health:
                    if (buff.amount < 0)
                        statsAffected.TakePureDam(buff.amount);
                    else
                    {
                        statsAffected.Heal(buff.amount);
                        if (target.healingBuffIndicator != null) target.healingBuffIndicator.SetActive(true);
                    }
                        
                    break;
                default:
                    break;
            }
            BufforDebuff newBuff = new BufforDebuff(
                buff.amount,
                buff.affects,
                buff.duration,
                buff.ramping,
                buff.durationTimer
                );
            statsAffected.buffs.Add(newBuff);
        }
    }

    private void addDisplacement(CharacterCombat target)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();

        Debug.Log(target.gameObject.name + " is being displaced");

        if (rb != null)
        {
            Vector3 direction = Vector3.zero;

            if (displacement.pushOrPull == DisplacementEffect.Push)
                direction = target.transform.position - displacePos;

            else if (displacement.pushOrPull == DisplacementEffect.Pull)
                direction = displacePos - target.transform.position;

            direction.y = 0;

            Debug.Log("Displacement velocity is:" + direction.normalized * displacement.distance);

            target.CastTime = 2f;
            target.SetAttackCooldown(2f);
            target.GetComponent<CharacterAnimator>().characterAnim.SetBool("basicAttack", false);
            //target.GetComponent<NavMeshAgent>().destination = target.transform.position + rb.velocity;
            target.GetComponent<NavMeshAgent>().Move(direction.normalized * displacement.distance);


            target.GetComponent<Player_Controller>()?.RemoveFocus();
        }
    }

    private void addCC_Effect(CharacterCombat target)
    {
        foreach (CC_Effect effect in cc_Effects)
        {
            effect.durationTimer = effect.duration;

            CC_Effect newEffect = new CC_Effect(
                effect.affect,
                effect.duration,
                effect.durationTimer
                );

            target.cc_Effects.Add(newEffect);
        }
    }

    private void ActivateParticleEffect(CharacterCombat target)
    {
        particleEffectActivate.SetActive(true);
    }

    protected void SpawnProjectile(Vector3 hitPosition)
    {
        GameObject spawnedProjectile;
        Vector3 newProjectileSpawnPos = Vector3.zero;

        if (projectileSpawnPos == Vector3.zero)
        {
            newProjectileSpawnPos = new Vector3(abilityUser.transform.localPosition.x, abilityUser.transform.localPosition.y + 1, abilityUser.transform.localPosition.z + 1);
            spawnedProjectile = Instantiate(projectile, newProjectileSpawnPos, projectile.transform.rotation);
        }
        else
        {
            spawnedProjectile = Instantiate(projectile, projectileSpawnPos, projectile.transform.rotation);
        }

       
        Debug.Log("Projectile Spawned ");

        displacePos = spawnedProjectile.transform.position;

        AbilityProjectile projectileScript = spawnedProjectile.GetComponent<AbilityProjectile>();

        projectileScript.OnHit = OnAbilityUse;
        if (spawnParticleEffect != null)
        {
            projectileScript.spawnParticleEffect = spawnParticleEffect;
            projectileScript.OnHit.AddListener(projectileScript.SpawnParticleEffect);
        }
        projectileScript.targetType = targetType;
        projectileScript.user = abilityUser;
        Vector3 direction;

        if (projectileSpawnPos == Vector3.zero)
        {
            direction = (hitPosition - newProjectileSpawnPos).normalized;
        }
        else
        {
            direction = (hitPosition - projectileSpawnPos).normalized;
        }
        
        Debug.Log("Projectile Direction: " +  direction.normalized);

        spawnedProjectile.GetComponent<Rigidbody>().velocity = direction * (Time.deltaTime + 10);
        Debug.Log("Projectile Velocity: " + spawnedProjectile.GetComponent<Rigidbody>().velocity);
    }

    public virtual void SetTarget(Vector3 pos, CharacterCombat character) { }

    public void SetProjectileSpawnPos(Vector3 pos)
    {
        projectileSpawnPos = pos;
    }

    public float GetCooldown()
    {
        return cooldown;
    }

    public float GetRange()
    {
        return range;
    }

    public TargetType GetTargetType()
    {
        return targetType;
    }

    public void SetCam(Camera newCam) { cam = newCam; }

    public void SetActivatedParticleEffect(GameObject effect) { particleEffectActivate = effect; }

    public virtual IEnumerator UseAbility(CharacterCombat target)
    {
        yield return new WaitForSeconds(delay);

        OnAbilityUse.Invoke(target);
    }

    public IEnumerator ProjectileSpawn(Vector3 pos)
    {
        yield return new WaitForSeconds(delay);

        SpawnProjectile(pos);
    }

}

public enum TargetType { Self, PlayerAlly, PlayerEnemy, Any, AnyExcludingSelf, SelfAndPlayerEnemy }






