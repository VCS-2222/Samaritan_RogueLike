using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MecUI;

    void Start() 
    {
        Time.timeScale = 1f;
        MecUI.SetActive(false);
    }

    public void PlayGame() 
    {
        SceneManager.LoadScene("IntroCutscene");
    }

    public void MechanicsExpIn() 
    {
        MecUI.SetActive(true);
    }

    public void MechanicsExpOut() 
    {
        MecUI.SetActive(false);
    }

    public void QuitGame() 
    {
        Application.Quit();
    }
}
