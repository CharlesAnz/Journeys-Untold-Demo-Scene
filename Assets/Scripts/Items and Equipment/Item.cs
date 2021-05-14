using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    new public string name = "New Item";
    protected GameObject user = null;
    public Sprite icon = null;
    public Color color = new Color(255,255,255);

    public bool isDefault = false;

    public virtual void Use(GameObject interactor)
    {
        Debug.Log(interactor.name + " is using " + name);
        user = interactor;
    }

    //removes item from users inventory
    public void RemoveFromInventory()
    {
        user.GetComponent<Inventory>().Remove(this);
    }
}
