using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentPiece equipSlot;

    public List<Stat> statMods = new List<Stat>();

    public int armorMod;
    public int damageMod;

    ///equips piece of equipment
    public override void Use(GameObject interactor)
    {
        base.Use(interactor);
        EquipmentManager manager = interactor.GetComponent<EquipmentManager>();

        manager.Equip(this);
        RemoveFromInventory();
    }


}

//Slots that equipment can be placed in
public enum EquipmentPiece { Head, Chest, Legs, RHand, LHand, Feet }
