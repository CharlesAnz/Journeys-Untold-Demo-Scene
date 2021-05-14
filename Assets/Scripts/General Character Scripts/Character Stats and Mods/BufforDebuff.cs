using UnityEngine;

[System.Serializable]
public class BufforDebuff
{
    public float amount;

    public StatBuffs affects;

    public float duration;

    [HideInInspector]
    public float durationTimer;

    public bool ramping;

    public BufforDebuff(float amount, StatBuffs affects, float duration, bool ramping, float durationTimer)
    {
        this.amount = amount;
        this.affects = affects;
        this.duration = duration;
        this.ramping = ramping;
        this.durationTimer = durationTimer;
    }

}
public enum StatBuffs { Damage, Armor, MoveSpeed, AttackSpeed, Health }