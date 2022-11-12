using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public PlayerScript playerScript;
    public GameObject dm;
    public GameObject db;

    void Start() 
    {
        dm.SetActive(false);
        db.SetActive(false);
    }

    void Update() 
    {
        if(playerScript.isDead) 
        {
            dm.SetActive(true);
            db.SetActive(true);
        }
    }

    public void GoBackToMenu() 
    {
        SceneManager.LoadScene("Menu");
    }
}
