using UnityEngine;

//Put this on any item in the game that can be picked up and put in an inventory
public class Item_Pickup : Interactable
{
    public Item item;

    //Interactable 
    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);

        Pickup(interactor);
    }

    //picks up item and puts it into the inventory of the character that picked it up
    void Pickup(GameObject interactor)
    {
        Debug.Log("Picking up " + item.name);
        bool wasPickedUp = interactor.GetComponent<Inventory>().Add(item);
        //add to inventory

        if (wasPickedUp)
            Destroy(gameObject);

    }


}
