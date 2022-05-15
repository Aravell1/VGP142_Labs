using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;
    public Button restartButton;
    public Button menuButton;
    public Button quitButton;
    public Button resumeButton;

    [Header("Menus")]
    public GameObject pauseMenu;

    [Header("Bars")]
    public Slider healthSlider;

    [Header("Text")]
    public TMP_Text healthText;

    // Start is called before the first frame update
    void Start()
    {
        if (startButton)
        {
            startButton.onClick.AddListener(() => StartGame());
        }
        if (restartButton)
        {
            restartButton.onClick.AddListener(() => Restart());
        }
        if (menuButton)
        {
            menuButton.onClick.AddListener(() => Menu());
        }
        if (quitButton)
        {
            quitButton.onClick.AddListener(() => QuitGame());
        }
        if (resumeButton)
        {
            resumeButton.onClick.AddListener(() => ResumeGame());
        }
        if (healthSlider && healthText)
        {
            healthSlider.onValueChanged.AddListener((value) => OnSliderValueChange(value));
            healthText.text = healthSlider.value.ToString();
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
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
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
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    void OnSliderValueChange(float value)
    {
        healthText.text = value.ToString();
    }
}
