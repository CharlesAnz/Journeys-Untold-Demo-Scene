using System.Collections.Generic;
using UnityEngine;

public class Chest_Spawner : Interactable
{
    public GameObject chestTop;
    public List<GameObject> itemsToSpawn = new List<GameObject>();

    public override void Interact(GameObject interactor)
    {
        chestTop.transform.localRotation = Quaternion.Euler(-45, 0, 0);

        if (itemsToSpawn.Count == 0) return;

        foreach(GameObject item in itemsToSpawn)
        {
            Random.InitState(Random.Range(1,100));
            float z = UnityEngine.Random.Range(-2.0f, 2.0f);

            //GameObject newItem = Instantiate(item, Vector3.zero, item.transform.rotation, this.transform);
            GameObject newItem = Instantiate(item, this.transform, false);

            newItem.transform.localScale = new Vector3(item.transform.localScale.x * 0.5f, item.transform.localScale.y * 0.5f, item.transform.localScale.z * 0.5f);

            newItem.transform.position = new Vector3(newItem.transform.position.x - 1, 
                newItem.transform.position.y + 0.2f, 
                newItem.transform.position.z + z);
        }

        itemsToSpawn.Clear();
    }
}
