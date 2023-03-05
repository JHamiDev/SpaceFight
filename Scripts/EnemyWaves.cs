using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyWaves : MonoBehaviour
{
    [System.Serializable]
    public class Level
    {
        public string name;
        public Wave[] waves;
    }

    [System.Serializable]
    public class Wave
    {
        public string name;
        public GameObject[] enemiesToSpawn;
    }

    public Text levelCounter;
    public Text waveCounter;

    private int numWave;
    private int numLevel;
    private int numEnemiesAlive;
    private int difficultyLevel;
    private int levelsCompleted;

    public GameObject player;
    public GameObject levelCompleteScreenUI;
    public GameObject GameOverScreenUI;
    private GameObject scoreObject;
    public GameObject[] enemies;
    private GameObject[] enemiesAlive;

    public Transform playerSpawnPoint;
    public Transform[] enemySpawnPoints;

    public static bool WaveDelayIsActive = false;
    public static bool levelComplete;

    private Wave _wave;
    private Level _level;
    public Level[] levels;

    private PickupScript pickupScript;

    void Start()
    {
        pickupScript = player.GetComponent<PickupScript>();

        scoreObject = GameObject.Find("Score");

        levelComplete = false;

        PlayerMovement.isDashing = false;

        if (MainMenu.isEndlessMode)
        {
            numLevel = 0;
        } else
        {
            numLevel = MainMenu.selectedLevel - 1;
        }

        numWave = 0;
        _level = levels[numLevel];
        _wave = _level.waves[numWave];

        levelCounter.text = _level.name;

        Time.timeScale = 1f;
        PlayerMovement.GameIsOver = false;

        difficultyLevel = 0;

        StartCoroutine(WaveDelay());

        WaveSetup();
    }

    void Update()
    {
        enemiesAlive = GameObject.FindGameObjectsWithTag("Enemy");
        numEnemiesAlive = enemiesAlive.Length;

        //update wave counter text
        waveCounter.text = _wave.name;

        //check if level has been completed (not on endless mode) and display level complete screen
        if (numEnemiesAlive <= 0 && numWave == _level.waves.Length - 1 && !levelComplete && !MainMenu.isEndlessMode)
        {
            levelCompleteScreenUI.SetActive(true);
            Time.timeScale = 0f;
            levelComplete = true;

            if (levelComplete)
            {
                FindObjectOfType<AudioManager>().Play("LevelCompleteSound");
            }

            levelsCompleted = PlayerPrefs.GetInt("LevelsCompleted", 0);

            if (levelsCompleted == numLevel)
            {
                levelsCompleted = numLevel + 1;
                PlayerPrefs.SetInt("LevelsCompleted", levelsCompleted);
            }

            if (numLevel == levels.Length - 1)
            {
                levelCompleteScreenUI.transform.GetChild(3).gameObject.SetActive(false);
            }
        }

        //spawn waves of enemies and reset player position
        if (numEnemiesAlive <= 0 && numWave < _level.waves.Length - 1)
        {
            numWave++;
            _wave = _level.waves[numWave];

            WaveSetup();
        } else if (numEnemiesAlive <= 0 && numWave == _level.waves.Length - 1 && MainMenu.isEndlessMode) //check if level has been completed on endless mode
        {
            if (numLevel == levels.Length - 1)
            {
                //reset to level 1 and increase enemy stats
                numLevel = 0;
                _level = levels[numLevel];
                numWave = 0;
                _wave = _level.waves[numWave];

                if (difficultyLevel <= 3)
                {
                    difficultyLevel++;
                    numPlusSigns += "+";

                    foreach (GameObject enemy in enemies)
                    {
                        enemy.GetComponent<EnemyBehavior>().speed++;
                        enemy.GetComponent<EnemyBehavior>().bulletForce++;
                        enemy.GetComponent<EnemyBehavior>().numBullets++;
                    }
                }
            } else
            {
                numLevel++;
                _level = levels[numLevel];
                numWave = 0;
                _wave = _level.waves[numWave];
            }

            levelComplete = true;

            if (levelComplete)
            {
                FindObjectOfType<AudioManager>().Play("LevelCompleteSound");
            }

            WaveSetup();
        }
    }

    void DestroyAllBullets()
    {
        GameObject[] playerBullets = GameObject.FindGameObjectsWithTag("PlayerBullet");
        GameObject[] enemyBullets = GameObject.FindGameObjectsWithTag("EnemyBullet");

        foreach (GameObject playerbullet in playerBullets)
        {
            Destroy(playerbullet);
        }

        foreach (GameObject enemyBullet in enemyBullets)
        {
            Destroy(enemyBullet);
        }
    }

    bool isPlayerRenderingOff;
    float delayCounter = 3;
    string numPlusSigns;
    IEnumerator WaveDelay()
    {
        isPlayerRenderingOff = player.GetComponent<SpriteRenderer>().forceRenderingOff;
        player.GetComponent<SpriteRenderer>().forceRenderingOff = false;

        Time.timeScale = 0f;
        WaveDelayIsActive = true;

        delayCounter = 3;
        while (delayCounter > 1)
        {
            delayCounter -= Time.unscaledDeltaTime;
            waveCounter.text = delayCounter.ToString("0");
            if (levelComplete)
            {
                levelCounter.text = "Level Complete!";
            }

            //yield until next frame
            yield return null;
        }

        //update level counter text
        if (levelComplete && delayCounter <= 1)
            {
                //update level counter text
                if (difficultyLevel <= 0)
                {
                    levelCounter.text = _level.name;
                } else
                {
                    for (int i = 0; i < difficultyLevel; i++)
                    {
                        levelCounter.text = _level.name + numPlusSigns;
                    }
                }

                levelComplete = false;
            }

        player.GetComponent<SpriteRenderer>().forceRenderingOff = isPlayerRenderingOff;

        WaveDelayIsActive = false;
        Time.timeScale = 1f;
    }

    void WaveSetup()
    {
        StartCoroutine(WaveDelay());

        for (int i = 0; i < _wave.enemiesToSpawn.Length; i++)
        {
            Instantiate(_wave.enemiesToSpawn[i], enemySpawnPoints[i].position, Quaternion.identity);
        }

        player.transform.position = playerSpawnPoint.position;

        DestroyAllBullets();
    }

    public void NextLevel()
    {
        numLevel++;
        _level = levels[numLevel];
        numWave = 0;
        _wave = _level.waves[numWave];

        levelCompleteScreenUI.SetActive(false);

        LevelCleanup();

        WaveSetup();
    }

    public void Retry()
    {
        if (MainMenu.isEndlessMode)
        {
            numLevel = 0;
            _level = levels[numLevel];
            levelCounter.text = _level.name;

            foreach (GameObject enemy in enemies)
            {
                //reset enemy stats
                enemy.GetComponent<EnemyBehavior>().speed -= 1 * difficultyLevel;
                enemy.GetComponent<EnemyBehavior>().bulletForce -= 1 * difficultyLevel;
                enemy.GetComponent<EnemyBehavior>().numBullets -= 1 * difficultyLevel;
            }

            scoreObject.GetComponent<Score>().score = 0;
            scoreObject.GetComponent<Score>().UpdateScore(0);
        }
        numWave = 0;
        _wave = _level.waves[numWave];

        levelComplete = false;
        PlayerMovement.GameIsOver = false;
        levelCompleteScreenUI.SetActive(false);
        GameOverScreenUI.SetActive(false);

        LevelCleanup();

        WaveSetup();
    }

    private GameObject[] heartPickups;
    private GameObject[] powerups;
    private void LevelCleanup()
    {
        player.GetComponent<Health>().health = 5;
        player.GetComponent<Shooting>().tripleShot = false;
        player.GetComponent<Shooting>().fastShot = false;
        player.GetComponent<Shooting>().powerShot = false;

        player.GetComponent<PickupScript>().StopAllCoroutines();
        pickupScript.powerupImageObj.sprite = pickupScript.powerupImages[3];
        pickupScript.powerupTimerText.text = "--";

        heartPickups = GameObject.FindGameObjectsWithTag("HeartPickup");
        foreach (GameObject heartPickup in heartPickups)
        {
            Destroy(heartPickup);
        }

        powerups = GameObject.FindGameObjectsWithTag("Powerup");
        foreach (GameObject powerup in powerups)
        {
            Destroy(powerup);
        }

        foreach (GameObject enemy in enemiesAlive)
        {
            Destroy(enemy);
        }
    }
}
