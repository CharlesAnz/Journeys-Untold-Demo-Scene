using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Potion")]
public class Potion : Item
{
    [SerializeField]
    private int value;

    [SerializeField]
    private bool doesHealing;

    public List<BufforDebuff> buffList = new List<BufforDebuff>();

    public override void Use(GameObject interactor)
    {
        base.Use(interactor);

        user = interactor;
        if (doesHealing) Heal(interactor.GetComponent<CharacterCombat>());
        if (buffList.Count > 0) addBuff(interactor.GetComponent<CharacterCombat>());

        RemoveFromInventory();
    }

    private void Heal(CharacterCombat target)
    {
        user.GetComponent<CharacterCombat>().AbilityHeal(target.GetMyStats(), value);
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
                        if (target.healingBuffIndicator != null) target.healingBuffIndicator.SetActive(true);
                        statsAffected.Heal(buff.amount);
                    }
                        
                    break;
                default:
                    break;
            }

            statsAffected.buffs.Add(buff);
        }
    }
}
