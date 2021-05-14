using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius;
    public Transform interactionTransform;

    protected bool isFocus = false;
    protected Transform interactorCharacter;

    protected bool hasInteracted = false;

    //This is what is called when a character interacts with an interactable. 
    //This is used as a base and meant to be built upon for more specialised interactables
    public virtual void Interact(GameObject interactor)
    {
        //Debug.Log(interactor + " is Interacting with " + this.name);
        //method is meant to be overwritten
    }

    private void Update()
    {
        //If this interactable is the focus of a character and not interacted with yet
        //check if character is close enough to interact, if so, set hasInteracted to true
        if (isFocus && !hasInteracted)
        {
            float distance = Vector3.Distance(interactorCharacter.position, interactionTransform.position);
            if (distance <= radius)
            {
                Interact(interactorCharacter.gameObject);
                hasInteracted = true;
            }
        }
    }

    //Once this interactable has been focused, finds out who is focusing it
    public virtual void OnFocused(Transform interactorTransform)
    {
        isFocus = true;
        interactorCharacter = interactorTransform;
        hasInteracted = false;
    }

    //If interactable is no longer being focused, resets variables
    public virtual void OnDefocused(GameObject defocuser)
    {
        isFocus = false;
        interactorCharacter = null;
        hasInteracted = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (interactionTransform == null)
            interactionTransform = transform;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(interactionTransform.position, radius);
    }
}
