using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public string activeScene;

    public bool pause = false;

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
            if (_health > value)
            {
                GameObject.Find("Canvas").GetComponent<CanvasManager>().healthSlider.value = value;
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

    [HideInInspector] public UnityEvent<int> onLifeValueChange;

    public void EndGame()
    {
        activeScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Gameover");
    }       
}
