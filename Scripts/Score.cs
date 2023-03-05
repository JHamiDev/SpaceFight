using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour
{
    public int score;
    public int highScore;

    private int levelsCompleted;

    private Text scoreText;
    private Text highscoreText;
    private Text gameOverScoreText;
    private Text gameOverHighscoreText;

    private Transform lockPanelsObj;
    private Transform levelButtonsObj;

    private GameObject[] levelButtons;
    private GameObject[] levelCompleteImages;
    public GameObject[] lockPanels;

    private static bool created = false;

    void Awake()
    {
        if (!created)
        {
            //make first instance persist
            Object.DontDestroyOnLoad(gameObject);
            created = true;
        } else
        {
            //destroy duplicate instances from scene reloads
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Game"))
        {
            score = 0;
            scoreText = GameObject.FindGameObjectWithTag("ScoreCounter").GetComponent<Text>();

            Transform gameOverScreen = GameObject.FindObjectOfType<Canvas>().transform.GetChild(4);
            gameOverScoreText = gameOverScreen.GetChild(4).GetComponent<Text>();
            gameOverHighscoreText = gameOverScreen.GetChild(5).GetComponent<Text>();

            if (MainMenu.isEndlessMode)
            {
                scoreText.text = "Score:" + score;
                Debug.Log("OnSceneLoaded: " + scene.name);
            } else
            {
                scoreText.enabled = false;
                scoreText.GetComponentInChildren<SpriteRenderer>().enabled = false;
                gameOverScoreText.enabled = false;
                gameOverHighscoreText.enabled = false;
            }   
        }

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Menu"))
        {
            highscoreText = GameObject.FindObjectOfType<Canvas>().transform.GetChild(3).GetChild(4).GetComponent<Text>();
            highScore = PlayerPrefs.GetInt("Highscore", 0);
            highscoreText.text = "Highscore:" + highScore;

            levelsCompleted = PlayerPrefs.GetInt("LevelsCompleted", 0);

            //deactivate level lock panels according to "levelsCompleted"
            lockPanelsObj = GameObject.FindObjectOfType<Canvas>().transform.GetChild(3).GetChild(5);

            lockPanels = new GameObject[lockPanelsObj.childCount];
            for (int i = 0; i < lockPanels.Length; i++)
            {
                lockPanels[i] = lockPanelsObj.GetChild(i).gameObject;
            }

            int panelIndex = 0;

            foreach (GameObject lockPanel in lockPanels)
            {
                if (panelIndex < levelsCompleted)
                {
                    lockPanel.SetActive(false);
                }

                panelIndex++;
            }


            //deactivate level complete indicators according to "levelsCompleted"
            levelButtonsObj = GameObject.FindObjectOfType<Canvas>().transform.GetChild(3).GetChild(3);

            levelButtons = new GameObject[levelButtonsObj.childCount];
            for (int i = 0; i < levelButtons.Length; i++)
            {
                levelButtons[i] = levelButtonsObj.GetChild(i).gameObject;
            }

            int levelButtonIndex = 0;
            levelCompleteImages = new GameObject[levelButtons.Length];
            foreach (GameObject levelButton in levelButtons)
            {
                levelCompleteImages[levelButtonIndex] = levelButton.transform.GetChild(1).gameObject;

                levelButtonIndex++;
            }

            int levelCompleteImageIndex = 0;
            foreach (GameObject levelCompleteImage in levelCompleteImages)
            {
                if (levelCompleteImageIndex >= levelsCompleted)
                {
                    levelCompleteImage.SetActive(false);
                }

                levelCompleteImageIndex++;
            }

            
            Debug.Log("OnSceneLoaded: " + scene.name);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        if (scoreText != null)
        {
            scoreText.text = "Score:" + score;
        }
    }

    public void UpdateScore(int scoreToAdd)
    {
        if (MainMenu.isEndlessMode)
        {
            score += scoreToAdd;
            scoreText.text = "Score:" + score;
            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("Highscore", highScore);
            }
        }
    }

    public void UpdateGameOverScoreText()
    {
        gameOverScoreText.text = "Score:" + score;
        gameOverHighscoreText.text = "Highscore:" + PlayerPrefs.GetInt("Highscore", 0);
    }
}
