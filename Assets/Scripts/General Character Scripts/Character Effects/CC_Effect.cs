using UnityEngine;

[System.Serializable]
public class CC_Effect
{
    public StatusEffects affect;

    public float duration;

    [HideInInspector]
    public float durationTimer;

    public CC_Effect(StatusEffects affect, float duration, float durationTimer)
    {
        this.affect = affect;
        this.duration = duration;
        this.durationTimer = durationTimer;
    }
}

public enum StatusEffects { Stun, Root, Silence, Disarm }
