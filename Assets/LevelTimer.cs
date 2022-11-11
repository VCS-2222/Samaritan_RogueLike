using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public float time = 0f;
    public bool stopTimer = false;
    public float finalTime;
    public TMP_Text timerText;
    public PlayerScript ps;

    private void Update()
    {
        if(ps.isDead)
        {
            stopTimer = false;
        }

        if(!stopTimer)
        {
            time += Time.deltaTime;
        }
        else
        {
            finalTime = time;
        }

        timerText.text = time.ToString("0");
    }
}
