using System.Collections.Generic;
using UnityEngine;

//This is for holding the stats of any character and is used mostly for combat purposes
public class Character_Stats : MonoBehaviour
{
    [SerializeField]
    public float curHP; //{ get; private set; }

    public Stat maxHP;
    public Stat damage;
    public Stat armor;
    public Stat moveSpeed;
    public Stat attackSpeed;

    public bool dead = false;

    public List<Ability> abilities = new List<Ability>();

    public List<GameObject> abilityParticleEffects = new List<GameObject>();

    public List<BufforDebuff> buffs = new List<BufforDebuff>();

    private void Awake()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            abilities[i].SetCam(Camera.main);
            abilities[i].cooldownTimer = 0;

            if(abilityParticleEffects.Count > 0)
            {
                if (abilityParticleEffects[i] != null)
                    abilities[i].SetActivatedParticleEffect(abilityParticleEffects[i]);
            }
                
        }
    }

    private void Start()
    {
        foreach (Ability ability in abilities)
        {
            ability.SetCam(Camera.main);
            ability.cooldownTimer = 0;
        }
        maxHP.statName = "maxHP";
        damage.statName = "damage";
        armor.statName = "armor";
        moveSpeed.statName = "moveSpeed";
        attackSpeed.statName = "attackspeed";

        curHP = maxHP.GetValue();
    }

    //Method for taking damage, damage is subtracted by the amount of armor and damage min is 0
    public void TakeDam(float damage)
    {
        if (armor.GetValue() > 0)
        {
            damage -= armor.GetValue();
        }

        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        curHP -= damage;
        Debug.Log(gameObject + " takes " + damage + " damage");
        PlaySoundOnHit();
        if (curHP <= 0)
        {
            Die();
        }
    }

    //Method for taking damage not affected by armor
    public void TakePureDam(float damage)
    {
        if (damage < 0)
            damage = Mathf.Abs(damage);

        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        curHP -= damage;
        Debug.Log(gameObject + " takes " + damage + " bleed damage");

        if (curHP <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        amount = Mathf.Clamp(amount, 0, int.MaxValue);

        curHP += amount;
        Debug.Log(gameObject + " heals for " + amount + " of health");

        if (curHP >= maxHP.GetValue())
        {
            curHP = maxHP.GetValue();
        }
    }

    public virtual void Die()
    {
        GetComponent<CharacterAnimator>().characterAnim.SetBool("basicAttack", false);
        GetComponent<CharacterAnimator>().characterAnim.SetTrigger("death");
        dead = true;
    }

    public virtual void PlaySoundOnHit()
    {

    }
}
