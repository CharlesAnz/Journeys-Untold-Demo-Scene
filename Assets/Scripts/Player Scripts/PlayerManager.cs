using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    //Singleton class for controling player characters

    #region Singleton
    public static PlayerManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public delegate void OnCharacterChange();
    public OnCharacterChange onCharacterChangeCallback;
    public GameObject player2;
    public GameObject player1;

    public UI_HealthBar playerHealthBar;
    public UI_HealthBar targetHealthBar;

    public Player_Controller activePerson;

    private CharacterCombat activePersonCombat;

    public bool gameOver = false;

    public string titleScreen = "Start Screen";

    public GameObject UICanvas;

    void Start()
    {
        activePerson = player1.GetComponent<Player_Controller>();
        player1.GetComponent<Player_Controller>().activeCharacter = true;
        
        foreach (Ability ability in player1.GetComponent<Player_Stats>().abilities)
        {
            ability.cooldownTimer = 0;
        }

        if (player2 != null)
        {
            player2.GetComponent<Player_Controller>().activeCharacter = false;
            foreach (Ability ability in player2.GetComponent<Player_Stats>().abilities)
            {
                ability.cooldownTimer = 0;
            }
        }
       
        activePersonCombat = player1.GetComponent<CharacterCombat>();
        
        if(playerHealthBar != null)
        {
            playerHealthBar.UpdateUI(activePerson.gameObject.name,
                (int)activePersonCombat.GetMyStats().maxHP.GetValue(),
                (int)activePersonCombat.GetMyStats().curHP
                );
        }

        if(targetHealthBar != null)
        {
            targetHealthBar.UpdateUI(" ", 1, 0);
            targetHealthBar.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (gameOver) return;
        
        if (playerHealthBar != null)
            playerHealthBar.SetCurHP((int)activePerson.GetComponent<CharacterCombat>().GetMyStats().curHP);

        if (activePerson.focus != null)
        {
            if (activePerson.focus.GetType().Equals(typeof(Enemy)))
            {
                CharacterCombat focusedEnemy = activePerson.focus.GetComponent<CharacterCombat>();
                targetHealthBar.gameObject.SetActive(true);

                targetHealthBar.UpdateUI(focusedEnemy.gameObject.name,
                    (int)focusedEnemy.GetMyStats().maxHP.GetValue(),
                    (int)focusedEnemy.GetMyStats().curHP);
            }
        }
        else
        {
            targetHealthBar.UpdateUI(" ", 1, 0);
            targetHealthBar.gameObject.SetActive(false);
        }


        //press 1 on keyboard to switch to character 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player1.GetComponent<Player_Controller>().activeCharacter = true;
            if (player2 != null) player2.GetComponent<Player_Controller>().activeCharacter = false;
            activePerson = player1.GetComponent<Player_Controller>();

            playerHealthBar.UpdateUI(activePerson.gameObject.name,
                (int)activePersonCombat.GetMyStats().maxHP.GetValue(),
                (int)activePersonCombat.GetMyStats().curHP
                );

            if (onCharacterChangeCallback != null)
            {
                onCharacterChangeCallback.Invoke();

            }

        }

        //press 2 on keyboard to switch to character 2

        if (player2 != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                player1.GetComponent<Player_Controller>().activeCharacter = false;
                player2.GetComponent<Player_Controller>().activeCharacter = true;
                activePerson = player2.GetComponent<Player_Controller>();

                playerHealthBar.UpdateUI(activePerson.gameObject.name,
                (int)activePersonCombat.GetMyStats().maxHP.GetValue(),
                (int)activePersonCombat.GetMyStats().curHP
                );

                if (onCharacterChangeCallback != null)
                {
                    onCharacterChangeCallback.Invoke();
                }
            }
        }

        AbilityUI abilityUI = UICanvas.GetComponent<AbilityUI>();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            abilityUI.SlotAbilityCasted(KeyCode.Q);
            //activePerson.GetComponent<Player_Stats>().abilities[0].Use(activePerson.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            abilityUI.SlotAbilityCasted(KeyCode.W);
            //activePerson.GetComponent<Player_Stats>().abilities[1].Use(activePerson.gameObject);
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            abilityUI.SlotAbilityCasted(KeyCode.E);
            //activePerson.GetComponent<Player_Stats>().abilities[2].Use(activePerson.gameObject);
        }
            
    }

    public void LoseCondition()
    {
        StartCoroutine(RestartGame());
    }

    public void WinCondition(GameObject enemy)
    {
        gameOver = true;

        enemy.SetActive(false);
        activePerson.gameObject.SetActive(false);

        UICanvas.SetActive(false);
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(7);

        ////Call these when you need to reset the characters to restart the fight
        activePerson.GetComponent<Player_Stats>().ResetStats();
    }
}
