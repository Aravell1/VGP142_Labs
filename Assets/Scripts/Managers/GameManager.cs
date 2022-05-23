using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [Header("Cameras")]
    public Camera mainCam;
    public Camera fpsCam;

    public string activeScene;

    public bool pause = false;

    int _score = 0;

    bool _gunEquipped = false;
    
    int _health = 3;
    int maxHealth = 3;

    public int health
    {
        get
        {
            return _health;
        }
        set
        {
            GameObject.Find("Canvas").GetComponent<CanvasManager>().healthSlider.value = value;

            if (_health > value)
            {                
                if (value > 0)
                    GameObject.FindWithTag("Player").GetComponentInChildren<Animator>().SetTrigger("Hit");
            }

            _health = value;
            if (_health > maxHealth)
            {
                _health = maxHealth;
            }

            onLifeValueChange.Invoke(value);

            if (_health <= 0)
            {
                GameObject.FindWithTag("Player").GetComponentInChildren<Animator>().SetTrigger("Death");
            }

            Debug.Log("Health Set to: " + health.ToString());
        }
    }

    public bool gunEquipped
    {
        get
        {
            return _gunEquipped;
        }
        set
        {
            if (mainCam && fpsCam)
            {
                if (value == true)
                {
                    fpsCam.enabled = true;
                    mainCam.enabled = false;
                }
                else
                {
                    fpsCam.enabled = false;
                    mainCam.enabled = true;
                }
            }
            _gunEquipped = value;
        }
    }

    public int score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
        }
    }

    [HideInInspector] public UnityEvent<int> onLifeValueChange;

    public void EndGame()
    {
        activeScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Gameover");
    }       
}
