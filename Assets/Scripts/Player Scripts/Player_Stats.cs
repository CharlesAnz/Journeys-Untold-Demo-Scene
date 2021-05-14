using UnityEngine;
using UnityEngine.AI;

public class Player_Stats : Character_Stats
{
    EquipmentManager equipManager;
    PlayerManager playerManager;

    public Transform resetPos;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.instance;

        equipManager = GetComponent<EquipmentManager>();
        equipManager.onEquipmentChanged += OnEquipmentChanged;

        if (resetPos == null) resetPos = transform;

        curHP = maxHP.GetValue();
    }

    void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        if (newItem != null)
        {
            armor.AddModifier(newItem.armorMod);
            damage.AddModifier(newItem.damageMod);
        }

        if (oldItem != null)
        {
            armor.RemoveModifier(oldItem.armorMod);
            damage.RemoveModifier(oldItem.damageMod);
        }

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

        GetComponent<Player_Controller>().RemoveFocus();
        GetComponent<Player_Movement>().MovetoPoint(resetPos.position);
        GetComponent<CharacterAnimator>().characterAnim.SetBool("basicAttack", false);
        GetComponent<CharacterAnimator>().characterAnim.ResetTrigger(abilities[0].animatorTrigger);
        GetComponent<CharacterAnimator>().characterAnim.ResetTrigger(abilities[1].animatorTrigger);
        GetComponent<CharacterAnimator>().characterAnim.ResetTrigger(abilities[2].animatorTrigger);
        GetComponent<CharacterAnimator>().characterAnim.SetTrigger("reset");

        curHP = maxHP.GetValue();

        GetComponent<NavMeshAgent>().Warp(GetComponent<Player_Stats>().resetPos.position);
        //transform.position = resetPos.position;

        dead = false;
    }

    public override void Die()
    {
        if (dead) return;
        base.Die();
        GetComponent<CharacterAnimator>().characterAnim.SetTrigger("death");
        playerManager.LoseCondition();
    }
}
