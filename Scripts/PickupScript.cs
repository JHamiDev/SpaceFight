using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupScript : MonoBehaviour
{
    public GameObject heartPickup;
    private GameObject[] heartsInScene;
    public GameObject[] powerups;
    private GameObject[] powerupsInScene;
    private float spawnDelay = 2f;
    private Vector2 spawnPos;
    private int spawnChance;

    private int powerupID;

    private Shooting shootingComponent;

    public Image powerupImageObj;
    public Text powerupTimerText;
    public Sprite[] powerupImages;

    void Start()
    {
        shootingComponent = gameObject.GetComponent<Shooting>();
        powerupImageObj.sprite = powerupImages[3];
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("HeartPickup"))
        {
            gameObject.GetComponent<Health>().health++;
            Destroy(collision.gameObject);
            FindObjectOfType<AudioManager>().Play("HealthSound");
        }

        if (collision.collider.CompareTag("Powerup"))
        {
            StopAllCoroutines();
            powerupTimerText.text = "--";
            powerupImageObj.sprite = powerupImages[3];


            switch (powerupID)
            {
                case 0:
                    shootingComponent.tripleShot = true;
                    shootingComponent.fastShot = false;
                    shootingComponent.powerShot = false;
                    StartCoroutine(powerupTimer(10, 0, powerupImages[0]));
                    break;
                case 1:
                    shootingComponent.tripleShot = false;
                    shootingComponent.fastShot = true;
                    shootingComponent.powerShot = false;
                    StartCoroutine(powerupTimer(10, 1, powerupImages[1]));
                    break;
                case 2:
                    shootingComponent.tripleShot = false;
                    shootingComponent.fastShot = false;
                    shootingComponent.powerShot = true;
                    StartCoroutine(powerupTimer(10, 2, powerupImages[2]));
                    break;
                default:
                    powerupImageObj.sprite = powerupImages[3];
                    break;
            }

            Destroy(collision.gameObject);

            FindObjectOfType<AudioManager>().Play("PowerupSound");
        }
    }

    void Update()
    {
        spawnDelay -= Time.deltaTime;
        heartsInScene = GameObject.FindGameObjectsWithTag("HeartPickup");
        powerupsInScene = GameObject.FindGameObjectsWithTag("Powerup");

        if (spawnDelay <= 0)
        {
            spawnChance = Random.Range(1, 25);
            if (spawnChance == 1 && heartsInScene.Length < 1)
            {
                spawnPos = new Vector2(Random.Range(-7f, 7f), Random.Range(-2f, 2f));
                Instantiate(heartPickup, spawnPos, Quaternion.identity);
            } else if (spawnChance == 2 && powerupsInScene.Length < 1)
            {
                spawnPos = new Vector2(Random.Range(-7f, 7f), Random.Range(-2f, 2f));
                powerupID = Random.Range(0, powerups.Length);
                Instantiate(powerups[powerupID], spawnPos, Quaternion.identity);
            }
            spawnDelay = 2f;
        }
    }

    private IEnumerator powerupTimer(int seconds, int powerupID, Sprite image)
    {
        int counter = seconds;

        powerupImageObj.sprite = image;

        while (counter > 0)
        {
            powerupTimerText.text = counter.ToString("0");
            yield return new WaitForSeconds(1);
            counter--;
        }

        powerupTimerText.text = "--";
        powerupImageObj.sprite = powerupImages[3];

        switch (powerupID)
        {
            case 0:
                shootingComponent.tripleShot = false;
                break;
            case 1:
                shootingComponent.fastShot = false;
                break;
            case 2:
                shootingComponent.powerShot = false;
                break;
        }
    }
}
