using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    bool endGame = false;

    // Update is called once per frame
    void Update()
    {
        if (endGame == true)
            if (Input.GetKeyDown(KeyCode.Return))
                EndGame();
    }
    public void EndGameQuery()
    {
        endGame = !endGame;
    }

    public void EndGame()
    {        
        SceneManager.LoadScene("Gameover");
    }
}
