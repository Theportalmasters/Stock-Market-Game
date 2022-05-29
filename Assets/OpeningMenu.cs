using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpeningMenu : MonoBehaviour
{
    private GameObject currentMenu;
    public Toggle moneyToggle;
    public TMP_InputField moneyField;
    public Toggle timeToggle;
    public TMP_InputField timeField;

    public void Start()
    {
        currentMenu = transform.GetChild(1).gameObject;
        for (int i = 2; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void SwitchToMenu(GameObject newMenu)
    {
        currentMenu.SetActive(false);
        currentMenu = newMenu;
        currentMenu.SetActive(true);
    }

    public void ToggleMoneyVictory(bool value)
    {
        moneyField.interactable = value;
        if (timeToggle.isOn == value)
        {
            timeToggle.isOn = !value;
        }
    }

    public void ToggleTimeVictory(bool value)
    {
        timeField.interactable = value;
        if (moneyToggle.isOn == value)
        {
            moneyToggle.isOn = !value;
        }
    }

    public void Quit() { Application.Quit(); }
}
