using System.Collections.Generic;
using UnityEngine;

public class Enemy : Interactable
{
    public Character_Stats myStats;
    public List<CharacterCombat> interactorCombats = new List<CharacterCombat>();
     
    private void Start()
    {
        myStats = GetComponent<Character_Stats>();
    }

    public override void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent(out CharacterCombat attackerCombat))
        {
            attackerCombat.Attack(myStats);
        }
    }

    void CheckDistance(float distance, CharacterCombat attackerCombat)
    {
        distance -= 0.5f;
        if (attackerCombat.attackRange > 0)
        {
            if (distance <= attackerCombat.attackRange)
            {
                Interact(attackerCombat.gameObject);
            }
            else if (distance > attackerCombat.attackRange)
                attackerCombat.GetComponent<CharacterAnimator>().characterAnim.SetBool("basicAttack", false);
        }

        else
        {
            if (distance <= radius)
            {
                Interact(attackerCombat.gameObject);
            }
            else if (distance > radius)
                attackerCombat.GetComponent<CharacterAnimator>().characterAnim.SetBool("basicAttack", false);
        }
    }

    private void Update()
    {
        //If this interactable is the focus of a character and not interacted with yet
        //check if character is close enough to interact, if so, set hasInteracted to true
        
        for (int i = 0; i < interactorCombats.Count; i++)
        {
            if (isFocus)
            {
                float distance = Vector3.Distance(interactorCombats[i].transform.position, interactionTransform.position);
                CheckDistance(distance, interactorCombats[i]);
            }

            if (myStats.dead)
            {
                interactorCombats[i].GetComponent<Player_Controller>().RemoveFocus();
                return;
            }
        }
        

        if (myStats.dead && interactorCombats.Count == 0)
        {
            this.enabled = false;
        }
    }


    public override void OnDefocused(GameObject interactor)
    {
        base.OnDefocused(interactor);

        for (int i = 0; i < interactorCombats.Count; i++)
        {
            if (interactor.TryGetComponent(out CharacterCombat attackerCombat))
            {
                attackerCombat.GetComponent<CharacterAnimator>().characterAnim.SetBool("basicAttack", false);
                interactorCombats.Remove(attackerCombat);
            }
        }
    }

    public override void OnFocused(Transform interactorTransform)
    {
        base.OnFocused(interactorTransform);

        if(interactorTransform.TryGetComponent(out CharacterCombat attackerCombat))
        {
            if(!interactorCombats.Contains(attackerCombat))
                interactorCombats.Add(attackerCombat);
        }
            

    }

}
