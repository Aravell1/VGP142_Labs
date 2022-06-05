using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;
    public Button restartButton;
    public Button menuButton;
    public Button loadButton;
    public Button quitButton;
    public Button resumeButton;
    public Button endButton;
    public Button deathQuitButton;
    public Button deathLoadButton;

    [Header("Menus")]
    public GameObject pauseMenu;
    public GameObject deathMenu;

    [Header("Bars")]
    public Slider healthSlider;

    [Header("Text")]
    public TMP_Text healthText;
    public TMP_Text scoreText;
    public TMP_Text livesText;

    // Start is called before the first frame update
    void Start()
    {
        if (startButton)
            startButton.onClick.AddListener(StartGame);
        if (restartButton)
            restartButton.onClick.AddListener(Restart);
        if (menuButton)
            menuButton.onClick.AddListener(Menu);
        if (loadButton)
            loadButton.onClick.AddListener(LoadGame);
        if (quitButton)
            quitButton.onClick.AddListener(QuitGame);
        if (resumeButton)
            resumeButton.onClick.AddListener(ResumeGame);
        if (endButton)
            endButton.onClick.AddListener(EndGame);
        if (deathQuitButton)
            deathQuitButton.onClick.AddListener(QuitGame);
        if (deathLoadButton)
            deathLoadButton.onClick.AddListener(LoadGame);
        if (healthSlider && healthText)
        {
            GameManager.Instance.onHealthValueChange.AddListener(UpdateHealth);
            UpdateHealth(GameManager.Instance.health);
        }
        if (scoreText)
        {
            GameManager.Instance.onScoreValueChange.AddListener(UpdateScore);
            UpdateScore(GameManager.Instance.score);
        }
        if (livesText)
        {
            GameManager.Instance.onLifeValueChange.AddListener(UpdateLives);
            UpdateLives(GameManager.Instance.lives);
        }
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (!GameManager.Instance.pause)
                {
                    pauseMenu.SetActive(true);
                    GameManager.Instance.pause = true;
                }
                else
                {
                    pauseMenu.SetActive(false);
                    GameManager.Instance.pause = false;
                }
                Player.Instance.OnPause();
            }
        }
    }

    void UpdateLives(int value)
    {
        livesText.text = "Lives: " + value;
    }
    void UpdateHealth(int value)
    {
        healthSlider.value = value;
        healthText.text = value.ToString();
    }
    void UpdateScore(int value)
    {
        scoreText.text = "Score: " + value;
    }

    public void LoadGame()
    {
        if (GameManager.Instance.pause)
        {
            GameManager.Instance.pause = false;
            pauseMenu.SetActive(false);
            Player.Instance.GetComponent<Player>().OnPause();
        }

        if (Player.Instance.gameObject)
            Destroy(Player.Instance.gameObject);

        SceneManager.LoadScene("LoadingScene");
    }
    public void StartGame()
    {
        if (Player.Instance.gameObject)
            Destroy(Player.Instance.gameObject);

        GameManager.Instance.ResetSaveData();
        SceneManager.LoadScene("LoadingScene");
    }
    public void Restart()
    {
        SceneManager.LoadScene(GameManager.Instance.activeScene);
    }
    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        GameManager.Instance.pause = false;
        Player.Instance.OnPause();
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void EndGame()
    {
        GameManager.Instance.EndGame();
    }
}
