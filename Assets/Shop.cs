using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public static Shop shopIns;
    public TMP_Text manaText;
    public bool shopIsUp = false;

    public int currentMana;
    public PlayerScript ps;
    public GameObject theShop;

    public Button B1;
    public Button B2;
    public Button B3;
    public Button B4;
    public Button B6;

    private void Start()
    {
        shopIns = this;
    }

    private void Update()
    {
        manaText.text = currentMana + " Mana";

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            shopIsUp = !shopIsUp;
            CheckForShop();
        }
    }

    void CheckForShop()
    {
        if(shopIsUp)
        {
            theShop.SetActive(true);
        }
        else
        {
            theShop.SetActive(false);
        }
    }

    public void AddMana()
    {
        currentMana += 100;
    }

    public void Unlock3AA()
    {
        if(currentMana >= 30)
        {
            B1.interactable = false;
            currentMana -= 30;
            ps.availableAttacks++;
        }
    }

    public void Unlock4AA()
    {
        if (currentMana >= 40)
        {
            B2.interactable = false;
            currentMana -= 40;
            ps.availableAttacks++;
        }
    }

    public void UnlockKick()
    {
        if (currentMana >= 20)
        {
            B3.interactable = false;
            currentMana -= 20;
            ps.canKick = true;
        }
    }

    public void UnlockBlock()
    {
        if (currentMana >= 80)
        {
            B4.interactable = false;
            currentMana -= 80;
            ps.canBlock = true;
        }
    }

    public void UnlockRoll()
    {
        if (currentMana >= 70)
        {
            B6.interactable = false;
            currentMana -= 70;
            ps.canRoll = true;
        }
    }
}
