using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityProjectile : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<CharacterCombat> OnHit;

    [HideInInspector]
    public TargetType targetType;

    public GameObject spawnParticleEffect;

    public bool pierce;

    public float duration;

    float aliveTimer;

    [HideInInspector]
    public GameObject user;
    // Start is called before the first frame update
    void Start()
    {
        aliveTimer = duration;
    }

    private void Update()
    {
        aliveTimer -= Time.deltaTime;

        if (aliveTimer <= 0) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterCombat targetCharacter = other.GetComponent<CharacterCombat>();

        if (targetCharacter != null)
        {
            switch (targetType)
            {
                case TargetType.PlayerAlly:
                    //ability affects targeted ally (of the player character) or the player themself
                    if (targetCharacter.tag == "Ally" || targetCharacter.tag == "Player")
                    {
                        OnHit.Invoke(targetCharacter);
                        if (!pierce) Destroy(gameObject);
                    }
                    break;

                case TargetType.PlayerEnemy:
                    //ability affects targeted enemy
                    if (targetCharacter.gameObject.tag == "Enemy")
                    {
                        OnHit.Invoke(targetCharacter);
                        if (!pierce) Destroy(gameObject);
                    }
                    break;

                case TargetType.Any:
                    //ability affects targeted character
                    OnHit.Invoke(targetCharacter);
                    if (!pierce) Destroy(gameObject);
                    break;

                case TargetType.AnyExcludingSelf:
                    if (targetCharacter.gameObject != user)
                    {
                        OnHit.Invoke(targetCharacter);
                        if (!pierce) Destroy(gameObject);
                    }
                    break;
            }
        }
    }

    public void SpawnParticleEffect(CharacterCombat combat)
    {
        Vector3 newpos = new Vector3(combat.transform.position.x, combat.transform.position.y + 0.2f, combat.transform.position.z);
        Instantiate(spawnParticleEffect, newpos, spawnParticleEffect.transform.rotation);
    }
}
