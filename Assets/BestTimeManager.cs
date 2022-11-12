using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BestTimeManager : MonoBehaviour
{
    public TextMeshProUGUI daText;

    void Start()
    {
        daText.text = "Your time was : " + PlayerPrefs.GetFloat("HighTime").ToString("0");
    }

    public void DeleteTime() 
    {
        PlayerPrefs.DeleteKey("HighTime");
        daText.text = "Your time was : " + PlayerPrefs.GetFloat("HighTime").ToString();
    }
}
