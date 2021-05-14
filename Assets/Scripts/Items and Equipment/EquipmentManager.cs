using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public Equipment[] curEquipment;
    Inventory inventory;

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    private void Start()
    {
        int slots = System.Enum.GetNames(typeof(EquipmentPiece)).Length;
        curEquipment = new Equipment[slots];
        inventory = gameObject.GetComponent<Inventory>();
    }

    public void Equip(Equipment newItem)
    {
        int slotIndex = (int)newItem.equipSlot;

        Equipment oldItem = null;

        if (curEquipment[slotIndex] != null)
        {
            oldItem = curEquipment[slotIndex];
            inventory.Add(oldItem);
        }

        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        curEquipment[slotIndex] = newItem;
    }

    public void UnEquip(int slotIndex)
    {
        if (curEquipment[slotIndex] != null)
        {
            Equipment oldItem = curEquipment[slotIndex];
            inventory.Add(oldItem);

            curEquipment[slotIndex] = null;

            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }
        }
    }

    public void UnEquipAll()
    {
        for (int i = 0; i < curEquipment.Length; i++)
        {
            UnEquip(i);
        }
    }
}
