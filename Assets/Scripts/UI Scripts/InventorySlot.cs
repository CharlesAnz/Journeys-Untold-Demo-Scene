using UnityEngine;
using UnityEngine.UI;

//This is a script for an individual Inventory slot on the UI
public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button removeButton;

    public Player_Controller playerController;

    Item item;

    //Adds item to inventory slot
    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.color = item.color;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    //clears inventory slot
    public void ClearSlot()
    {
        item = null;

        icon.sprite = null;
        icon.color = Color.white;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    //removes item from character's inventory
    public void OnRemoveButton()
    {
        playerController.GetComponent<Inventory>().Remove(item);

        ClearSlot();
    }

    //uses item in inventory slot
    public void UseItem()
    {
        if (item != null)
        {
            item.Use(playerController.gameObject);
        }
    }
}
