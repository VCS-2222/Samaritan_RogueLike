using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDoorScript : MonoBehaviour
{
    public GameObject Door1;
    public GameObject Door2;
    public GameObject plat;
    public GameObject theFinalBossPrefab;
    public Transform fbt;

    public GameObject enemySpawner;

    private void Start()
    {
        enemySpawner.SetActive(false);
        Door1.SetActive(false);
        Door2.SetActive(false);
        plat.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            enemySpawner.SetActive(true);
            Door1.SetActive(true);
            Door2.SetActive(true);
            plat.SetActive(true);
            Instantiate(theFinalBossPrefab, fbt);

            Destroy(this);
        }
    }
}
