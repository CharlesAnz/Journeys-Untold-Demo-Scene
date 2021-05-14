using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability/Targeted")]
public class Targeted_Ability : Ability
{
    public CharacterCombat targetHit;

    public override void Use(GameObject user)
    {
        base.Use(user);

        Vector3 point = Vector3.zero;

        if (user.tag == "Player")
        {
            point = FindTargetWithMouse(100);
        }

        CharacterCombat targetCharacter = targetHit;

        if (targetCharacter == null)
        {
            Debug.Log("Did not hit target");      
            return; 
        }

        if (Vector3.Distance(targetCharacter.transform.position, abilityUser.transform.position) > range)
        {
            Debug.Log("Out of Range");
            return;
        }

        if (!Conditions(user)) return;

        if(spawnParticleEffect != null)
        {
            OnAbilityUse.AddListener(SpawnParticleEffect);
        }

        if (targetType == TargetType.Self)
        {
            //ability does thing to itself
            OnAbilityUse.Invoke(user.GetComponent<CharacterCombat>());
            return;
        }


        if (projectile != null)
        {
            if (delay > 0)
            {
                user.GetComponent<CharacterCombat>().SpawnProjectile(targetCharacter.transform.position, this);
            }
            else
            {
                SpawnProjectile(targetCharacter.transform.position);
            }
            return;
        }

        if (targetCharacter != null)
        {
            switch (targetType)
            {
                case TargetType.PlayerAlly:
                    //ability affects targeted ally
                    if (targetCharacter.tag == "Ally" || targetCharacter.tag == "Player")
                    {
                        Debug.Log("We hit: " + targetCharacter.name + " " + point);
                        if (delay > 0)
                            user.GetComponent<CharacterCombat>().UseAbility(targetCharacter, this);
                        else
                        {
                            OnAbilityUse.Invoke(targetCharacter);
                        }
                    }
                    break;

                case TargetType.PlayerEnemy:
                    //ability affects targeted enemy
                    if (targetCharacter.gameObject.tag == "Enemy")
                    {
                        Debug.Log("We hit: " + targetCharacter.name + " at " + point);
                        if (delay > 0)
                            user.GetComponent<CharacterCombat>().UseAbility(targetCharacter, this);
                        else
                        {
                            OnAbilityUse.Invoke(targetCharacter);
                        }
                    }
                    break;

                case TargetType.Any:
                    //ability affects targeted character
                    Debug.Log("We hit: " + targetCharacter.name + " at " + point);
                    if (delay > 0)
                        user.GetComponent<CharacterCombat>().UseAbility(targetCharacter, this);
                    else
                    {
                        OnAbilityUse.Invoke(targetCharacter);
                    }
                        
                    break;

                case TargetType.AnyExcludingSelf:
                    if (targetCharacter.gameObject != abilityUser)
                    {
                        Debug.Log("We hit: " + targetCharacter.name + " at " + point);
                        if (delay > 0)
                            user.GetComponent<CharacterCombat>().UseAbility(targetCharacter, this);
                        else
                        {
                            OnAbilityUse.Invoke(targetCharacter);
                        }
                    }

                    break;
            }
        }

    }

    private void SpawnParticleEffect(CharacterCombat target)
    {
        Vector3 newpos = new Vector3(target.transform.position.x, target.transform.position.y - 0.2f, target.transform.position.z);
        Instantiate(spawnParticleEffect, newpos, spawnParticleEffect.transform.rotation);
    }

    protected override Vector3 FindTargetWithMouse(float maxCastDistance)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxCastDistance))
        {
            targetHit = hit.collider.GetComponent<CharacterCombat>();
            //if(hit.collider.TryGetComponent(out targetHit))
            if(targetHit != null)
            {
                Vector3 direction = (targetHit.transform.position - abilityUser.transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                abilityUser.transform.rotation = lookRotation; 
            }
            
            return hit.point;
        }

        else return Vector3.zero;
    }

    public override void SetTarget(Vector3 pos, CharacterCombat character)
    {
        targetHit = character;
    }

}

