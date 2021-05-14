using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Despawner", menuName = "Inventory/DespawnObject")]
public class DespawnObject : Item
{
    public string objectToDespawnName;

    public override void Use(GameObject interactor)
    {
        base.Use(interactor);

        GameObject.Find(objectToDespawnName).SetActive(false);
    }

}
