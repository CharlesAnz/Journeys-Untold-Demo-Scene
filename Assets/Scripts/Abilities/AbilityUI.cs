using UnityEngine;

public class AbilityUI : MonoBehaviour
{
    public Transform abilityParent;
    public GameObject abilityUI;
    PlayerManager playerManager;

    public Player_Stats characterStats;
    Player_Controller playerController;

    public AbilitySlot[] slots;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.instance;

        characterStats = playerManager.player1.GetComponent<Player_Stats>();
        playerController = playerManager.player1.GetComponent<Player_Controller>();


        //Update the Inventory UI every time that the play switches characters
        playerManager.onCharacterChangeCallback += UpdateUI;

        slots = abilityParent.GetComponentsInChildren<AbilitySlot>();

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].playerController = playerController;

            if (i < characterStats.abilities.Count)
            {
                slots[i].AddAbility(characterStats.abilities[i]);

            }
            else
            {
                slots[i].ClearSlot();
            }
        }


    }

    void UpdateUI()
    {
        characterStats = playerManager.activePerson.GetComponent<Player_Stats>();
        playerController = playerManager.activePerson.GetComponent<Player_Controller>();

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].playerController = playerController;
            if (i < characterStats.abilities.Count)
            {
                slots[i].AddAbility(characterStats.abilities[i]);

            }
            else
            {
                slots[i].ClearSlot();
            }

        }
    }

    public void SlotAbilityCasted(KeyCode key)
    {
        foreach(AbilitySlot slot in slots)
        {
            if(slot.activationKey == key)
            {
                slot.UseAbility();
            }
        }
    }
}
