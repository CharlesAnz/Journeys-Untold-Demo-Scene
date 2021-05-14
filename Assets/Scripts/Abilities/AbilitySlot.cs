using UnityEngine;
using UnityEngine.UI;

public class AbilitySlot : MonoBehaviour
{
    public Image icon;
    public Text description;

    public Player_Controller playerController;

    public Button abilitybutton;
    Image cooldownUI;

    private bool onCooldown;

    public KeyCode activationKey;

    Ability ability;

    private void Update()
    {
        if(onCooldown)
        {
            cooldownUI.fillAmount += 1 / ability.GetCooldown() * Time.deltaTime;

            if(ability.cooldownTimer <= 0)
            {
                cooldownUI.fillAmount = 1;
                onCooldown = false;
            }
        }
    }
    //Adds item to inventory slot
    public void AddAbility(Ability newAbility)
    {
        ability = newAbility;

        icon.sprite = ability.icon;
        icon.enabled = true;

        icon.GetComponentInChildren<Text>().text = activationKey.ToString();

        description.text = newAbility.description;

        abilitybutton = GetComponentInChildren<Button>();

        abilitybutton.onClick.AddListener(UseAbility);

        cooldownUI = abilitybutton.GetComponent<Image>();

        cooldownUI.fillAmount = 1;
    }

    //clears inventory slot
    public void ClearSlot()
    {
        ability = null;

        icon.sprite = null;
        icon.enabled = false;
        description.text = "";
    }

    //uses item in inventory slot
    public void UseAbility()
    {
        if (ability != null)
        {
            if (ability.cooldownTimer < 0)
            {
                onCooldown = true;
                cooldownUI.fillAmount = 0;
            }
            ability.Use(playerController.gameObject);
            
        }
    }
}
