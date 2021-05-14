using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability/Aoe")]
public class Aoe_Ability : Ability
{
    public float areaSize;

    [SerializeField]
    private AOEType aoeType;

    [HideInInspector]
    private Vector3 origin;

    public bool selfOrigin = false;

    [SerializeField]
    private List<CharacterCombat> targets = new List<CharacterCombat>();

    public override void Use(GameObject user)
    {
        base.Use(user);

        if (selfOrigin)
        {
            origin = user.transform.position; 
        }

        else if (user.tag == "Player")
        {
            origin = FindTargetWithMouse(100);
        }

        if (Vector3.Distance(origin, abilityUser.transform.position) > range)
        {
            Debug.Log("Out of Range");
            return;
        }

        if (!Conditions(user)) return;

        displacePos = origin;

        if(projectile != null)
        {
            if (delay > 0)
                user.GetComponent<CharacterCombat>().SpawnProjectile(origin, this);
            else
                SpawnProjectile(origin);

            return;
        }

        if (origin != null)
        {
            if (aoeType == AOEType.Cube || aoeType == AOEType.Sphere)
                CheckArea();
            else if (aoeType == AOEType.Cone)
                CheckCone();
            else if (aoeType == AOEType.Line)
                CheckLine();


            foreach (var target in targets)
            {
                Debug.Log("Hit: " + target.name);
                if (delay > 0)
                    user.GetComponent<CharacterCombat>().UseAbility(target, this);
                else
                {
                    OnAbilityUse.Invoke(target);
                }
            }
        }
        targets.Clear();
    }

    private void CheckCone()
    {
        Collider[] colliderArray = Physics.OverlapSphere(origin, areaSize);

        if (colliderArray != null)
        {
            foreach (var collider in colliderArray)
            {

                Vector3 directionTowardT = collider.transform.position - origin;
                float angleFromConeCenter = Vector3.Angle(directionTowardT, abilityUser.transform.TransformDirection(Vector3.forward));


                CharacterCombat colliderCombat = collider.gameObject.GetComponent<CharacterCombat>();

                if (colliderCombat != null && angleFromConeCenter <= 45)
                {
                    switch (targetType)
                    {
                        case TargetType.PlayerAlly:
                            if (colliderCombat.tag == "Ally" || colliderCombat.tag == "Player")
                            {
                                targets.Add(colliderCombat);
                                Debug.Log("hit: " + colliderCombat.name);
                            }

                            break;

                        case TargetType.PlayerEnemy:
                            if (colliderCombat.tag == "Enemy")
                                targets.Add(colliderCombat);

                            break;

                        case TargetType.Any:
                            targets.Add(colliderCombat);
                            break;

                        default:
                            targets.Add(colliderCombat);
                            break;
                    }
                }
            }
        }
    }

    private void CheckLine()
    {
        Ray aoeLineRay = new Ray(abilityUser.transform.position, abilityUser.transform.TransformDirection(Vector3.forward) * 2);

        RaycastHit[] collidersHit = Physics.SphereCastAll(aoeLineRay, 0.3f, areaSize);

        if (collidersHit != null)
        {
            foreach (var rayHit in collidersHit)
            {
                CharacterCombat colliderCombat = rayHit.collider.gameObject.GetComponent<CharacterCombat>();
                if (colliderCombat != null)
                {
                    Debug.Log("Hit:" + colliderCombat.name);
                    switch (targetType)
                    {
                        case TargetType.PlayerAlly:
                            if (colliderCombat.tag == "Ally" || colliderCombat.tag == "Player") targets.Add(colliderCombat);
                            break;

                        case TargetType.PlayerEnemy:
                            if (colliderCombat.tag == "Enemy")
                            {
                                Debug.Log("Hit:" + colliderCombat.name);
                                targets.Add(colliderCombat);
                            } 
                            break;

                        case TargetType.Any:
                            targets.Add(colliderCombat);
                            break;
                    }
                }
            }
        }
    }

    private void CheckArea()
    {
        Collider[] collidersNear = null;

        if (aoeType == AOEType.Cube)
            collidersNear = Physics.OverlapBox(origin, new Vector3(areaSize / 2, areaSize / 2, areaSize / 2));
  

        else if (aoeType == AOEType.Sphere)
            collidersNear = Physics.OverlapSphere(origin, areaSize);
        
            

        if (collidersNear != null)
        {
            foreach (var collider in collidersNear)
            {
                CharacterCombat colliderCombat = collider.gameObject.GetComponent<CharacterCombat>();

                if (colliderCombat != null)
                {
                    switch (targetType)
                    {
                        case TargetType.PlayerAlly:
                            if (colliderCombat.tag == "Ally" || colliderCombat.tag == "Player") targets.Add(colliderCombat);
                            break;

                        case TargetType.PlayerEnemy:
                            if (colliderCombat.tag == "Enemy")
                            { 
                                targets.Add(colliderCombat);
                            }
                            break;

                        case TargetType.Any:
                            targets.Add(colliderCombat);
                            break;
                    }
                }
            }

            
        }
    }

    public override void SetTarget(Vector3 pos, CharacterCombat character)
    {
        origin = pos;
    }

    private void SpawnParticleEffect()
    {
        Vector3 newpos = new Vector3(origin.x, origin.y + 0.2f, origin.z);
        Instantiate(spawnParticleEffect, newpos, spawnParticleEffect.transform.rotation);
    }

    public override IEnumerator UseAbility(CharacterCombat target)
    {
        yield return new WaitForSeconds(delay);

        if (spawnParticleEffect != null)
        {
            SpawnParticleEffect();
        }

        OnAbilityUse.Invoke(target);
    }
}

public enum AOEType { Sphere, Cube, Cone, Line }


