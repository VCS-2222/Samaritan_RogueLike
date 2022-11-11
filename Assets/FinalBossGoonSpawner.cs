using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalBossGoonSpawner : MonoBehaviour
{
    public GameObject goon;
    public Transform thisPos;
    public FinalSkel finalSkel;
    public GameObject door1;
    public GameObject door2;
    public GameObject plat2;

    private void Start()
    {
        door1.SetActive(true);
        door2.SetActive(true);
        plat2.SetActive(true);
        finalSkel = FindObjectOfType<FinalSkel>();
        StartCoroutine(fight());
    }

    void Update()
    {
        if(finalSkel.isdead)
        {
            door1.SetActive(false);
            door2.SetActive(false);
            plat2.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene("EndCutscene");
    }

    IEnumerator fight()
    {
        while (!finalSkel.isdead)
        {
            yield return new WaitForSeconds(15f);

            Instantiate(goon, thisPos);

            yield return new WaitForSeconds(1f);

            Instantiate(goon, thisPos);
        }
    }
}
