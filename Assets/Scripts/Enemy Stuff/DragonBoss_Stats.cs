using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonBoss_Stats : Character_Stats
{
    PlayerManager playerManager;
    Vector3 startPos;

    private void Start()
    {
        startPos = gameObject.transform.position;
        playerManager = PlayerManager.instance;

        foreach (Ability ability in abilities)
        {
            ability.SetCam(Camera.main);
            ability.cooldownTimer = 0;
        }
        curHP = maxHP.GetValue();

        maxHP.statName = "maxHP";
        damage.statName = "damage";
        armor.statName = "armor";
        moveSpeed.statName = "moveSpeed";
        attackSpeed.statName = "attackspeed";
    }

    public void ResetStats()
    {
        foreach (Ability ability in abilities)
        {
            ability.cooldownTimer = 0;
        }

        for (int i = 0; i < buffs.Count; i++)
        {
            buffs[i].durationTimer = 0;
        }

        List<float> damageMods = damage.GetMods();
        List<float> armorMods = armor.GetMods();

        if (damageMods != null)
        {
            for (int y = 0; y < damageMods.Count; y++)
            {
                damage.RemoveModifier(damageMods[y]);
            }
        }

        if (armorMods != null)
        {
            for (int x = 0; x < armorMods.Count; x++)
            {
                damage.RemoveModifier(armorMods[x]);
            }
        }
        
        GetComponent<CharacterAnimator>().characterAnim.SetBool("basicAttack", false);
        GetComponent<CharacterAnimator>().characterAnim.ResetTrigger(abilities[0].animatorTrigger);
        GetComponent<CharacterAnimator>().characterAnim.ResetTrigger(abilities[1].animatorTrigger);
        GetComponent<CharacterAnimator>().characterAnim.ResetTrigger(abilities[2].animatorTrigger);
        GetComponent<CharacterAnimator>().characterAnim.SetTrigger("reset");
        GetComponent<NavMeshAgent>().SetDestination(startPos);
        GetComponent<NavMeshAgent>().Warp(startPos);
        curHP = maxHP.GetValue();

        //transform.position = startPos;
    }

    public override void Die()
    {
        base.Die();
        playerManager.WinCondition(gameObject);
    }
    public override void PlaySoundOnHit()
    {
        base.PlaySoundOnHit();
    }
}
