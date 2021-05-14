using UnityEngine;
using UnityEngine.UI;

//This is a script for an individual Inventory slot on the UI
public class EquipmentSlot : MonoBehaviour
{
    public Image icon;
    public Button removeButton;

    public EquipmentPiece piece;

    public Player_Controller playerController;

    Equipment item;

    public Sprite defaultImage;

    private void Start()
    {
        if(defaultImage == null)
            defaultImage = icon.sprite;
    }

    //Adds item to inventory slot
    public void AddEquipment(Equipment newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        removeButton.interactable = true;
    }

    //clears inventory slot
    public void ClearSlot()
    {
        item = null;

        icon.sprite = defaultImage;
        removeButton.interactable = false;
    }

    //removes item from character's inventory
    public void OnRemoveButton()
    {
        playerController.GetComponent<EquipmentManager>().UnEquip((int)item.equipSlot);
        ClearSlot();
    }
}

