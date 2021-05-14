using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Slider slider;
    PlayerManager playerManager;
    private Text nameTxt;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        nameTxt = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        playerManager = PlayerManager.instance;

    }

    public void SetMaxHP(int hp)
    {
        slider.maxValue = hp;
    }

    public void SetCurHP(int hp)
    {
        slider.value = hp;
    }

    public void SetNameTxt(string newName)
    {
        nameTxt.text = newName;
    }


    public void UpdateUI(string newName, int maxHP, int curHP)
    {
        nameTxt.text = newName;
        slider.maxValue = maxHP;
        slider.value = curHP;
    }
}
