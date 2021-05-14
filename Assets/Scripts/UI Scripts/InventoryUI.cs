using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public Transform equipmentParent;

    public GameObject inventoryUI;
    public GameObject equipmentUI;

    PlayerManager playerManager;

    public Inventory playerInventory;
    public EquipmentManager playerEquipment;

    Player_Controller playerController;
    Player_Stats playerStats;

    public InventorySlot[] inventorySlots;
    public EquipmentSlot[] equipmentSlots;

    public Text stats;



    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.instance;
        if (playerManager.player2 != null)
        { 
            playerInventory = playerManager.player2.GetComponent<Inventory>();
            playerEquipment = playerManager.player2.GetComponent<EquipmentManager>();
            
            //Update the Inventory UI every time that the inventory is modified
            playerInventory.onItemChangedCallback += UpdateInventoryUI;
            playerEquipment.onEquipmentChanged += UpdateEquipmentUI;
        }
        
        playerInventory = playerManager.player1.GetComponent<Inventory>();
        playerEquipment = playerManager.player1.GetComponent<EquipmentManager>();

        playerController = playerManager.player1.GetComponent<Player_Controller>();
        playerStats = playerManager.player1.GetComponent<Player_Stats>();


        //Update the Inventory UI every time that the inventory is modified
        playerInventory.onItemChangedCallback += UpdateInventoryUI;
        playerEquipment.onEquipmentChanged += UpdateEquipmentUI;

        //Update the UI every time that the player switches characters
        playerManager.onCharacterChangeCallback += UpdateInventoryUI;
        playerManager.onCharacterChangeCallback += UpdateEquipmentUI;

        inventorySlots = itemsParent.GetComponentsInChildren<InventorySlot>();
        equipmentSlots = equipmentParent.GetComponentsInChildren<EquipmentSlot>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].playerController = playerController;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            equipmentUI.SetActive(!equipmentUI.activeSelf);
        }

        stats.text = 
            playerStats.maxHP.statName + ": " + playerStats.maxHP.GetValue() + "\n" +
            "current HP: " + playerStats.curHP + "\n" +
            playerStats.damage.statName + ": " + playerStats.damage.GetValue() + "\n" +
            playerStats.armor.statName + ": " + playerStats.armor.GetValue() + "\n" +
            playerStats.moveSpeed.statName + ": " + playerStats.moveSpeed.GetValue() + "\n" +
            playerStats.attackSpeed.statName + ": " + playerStats.attackSpeed.GetValue() + "\n";
    }

    void UpdateInventoryUI()
    {
        playerInventory = playerManager.activePerson.GetComponent<Inventory>();
        playerController = playerManager.activePerson.GetComponent<Player_Controller>();
        playerStats = playerManager.activePerson.GetComponent<Player_Stats>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].playerController = playerController;
            if (i < playerInventory.items.Count)
            {
                inventorySlots[i].AddItem(playerInventory.items[i]);

            }
            else
            {
                inventorySlots[i].ClearSlot();
            }
        }

    }

    void UpdateEquipmentUI()
    {
        playerEquipment = playerManager.activePerson.GetComponent<EquipmentManager>();
        playerController = playerManager.activePerson.GetComponent<Player_Controller>();
        playerStats = playerManager.activePerson.GetComponent<Player_Stats>();

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].playerController = playerController;
            if (playerEquipment.curEquipment[i] != null)
            {
                if (playerEquipment.curEquipment[i].equipSlot == equipmentSlots[i].piece)
                    equipmentSlots[i].AddEquipment(playerEquipment.curEquipment[i]);
            }
            else
            {
                equipmentSlots[i].ClearSlot();
            }
        }
    }

    void UpdateEquipmentUI(Equipment newItem, Equipment oldItem)
    {
        playerEquipment = playerManager.activePerson.GetComponent<EquipmentManager>();
        playerController = playerManager.activePerson.GetComponent<Player_Controller>();
        playerStats = playerManager.activePerson.GetComponent<Player_Stats>();

        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].playerController = playerController;

            if(newItem != null)
            {
                if (equipmentSlots[i].piece == newItem.equipSlot)
                {
                    equipmentSlots[i].AddEquipment(newItem);
                }
                    
            }
            else if (newItem == null && playerEquipment.curEquipment[i] == null)
                equipmentSlots[i].ClearSlot();  
        }
    }


}
