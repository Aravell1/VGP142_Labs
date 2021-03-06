using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [Header("Objects")]
    public GameObject currentCheckpoint;
    public GameObject playerPrefab;

    public PlayerData Data;

    public EnemySpawn[] enemySpawnArray;

    public string activeScene;

    public bool pause = false;
    public bool playerInfoStored = false;

    int _score = 0;

    bool _gunEquipped = false;

    int _health = 3;
    int maxHealth = 3;
    int _lives = 5;

    protected override void Awake()
    {
        base.Awake();

        Instantiate(playerPrefab, GameObject.Find("Checkpoint").GetComponent<Checkpoints>().spawnPoint.position, GameObject.Find("Checkpoint").GetComponent<Checkpoints>().spawnPoint.rotation);

        LoadGame();
        Debug.Log(playerInfoStored);
        Debug.Log(Data.playerInfoStored);

        playerInfoStored = Data.playerInfoStored;
        if (playerInfoStored)
        {
            Debug.Log("moved player to " + Data.posX);
            Player.Instance.transform.position = new Vector3(Data.posX, Data.posY, Data.posZ);
            Player.Instance.transform.rotation = Quaternion.Euler(Data.rotX, Data.rotY, Data.rotZ);

            lives = Data.lives;
            score = Data.score;
        }

        enemySpawnArray = FindObjectsOfType<EnemySpawn>();

        for (int i = 0; i < enemySpawnArray.Length; i++)
            enemySpawnArray[i].SpawnEnemies();

        for (int i = 0; i < enemySpawnArray.Length; i++)
        {
            EnemySpawn temp = enemySpawnArray[i];
            int rand = UnityEngine.Random.Range(0, enemySpawnArray.Length);
            enemySpawnArray[i] = enemySpawnArray[rand];
            enemySpawnArray[rand] = temp;
        }

        for (int i = 0; i < enemySpawnArray.Length; i++)
        {
            if (i == 0)
                enemySpawnArray[i].spawnedEnemy.GetComponentInChildren<EnemyFire>().enemyDrop = 0;
            else
                enemySpawnArray[i].spawnedEnemy.GetComponentInChildren<EnemyFire>().enemyDrop = UnityEngine.Random.Range(1, enemySpawnArray[i].spawnedEnemy.GetComponentInChildren<EnemyFire>().pickupsPrefabArray.Length);
        }
    }

    public int lives
    {
        get
        {
            return _lives;
        }
        set
        {
            _lives = value;

            onLifeValueChange.Invoke(value);
        }
    }

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
                if (value > 0)
                    GameObject.FindWithTag("Player").GetComponentInChildren<Animator>().SetTrigger("Hit");
            }

            _health = value;
            if (_health > maxHealth)
            {
                _health = maxHealth;
            }

            onHealthValueChange.Invoke(_health);

            if (_health <= 0)
            {
                SoundManager.Instance.Play(Player.Instance.deathSound);
                GameObject.FindWithTag("Player").GetComponentInChildren<Animator>().SetTrigger("Death");
                lives--;
                Data.lives = lives;
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
            if (Player.Instance.GetComponent<Player>().mainCam && Player.Instance.GetComponent<Player>().fpsCam)
            {
                if (value == true)
                {
                    Player.Instance.GetComponent<Player>().fpsCam.enabled = true;
                    Player.Instance.GetComponent<Player>().mainCam.enabled = false;
                }
                else
                {
                    Player.Instance.GetComponent<Player>().fpsCam.enabled = false;
                    Player.Instance.GetComponent<Player>().mainCam.enabled = true;
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

            onScoreValueChange.Invoke(value);
        }
    }

    [HideInInspector] public UnityEvent<int> onHealthValueChange;
    [HideInInspector] public UnityEvent<int> onScoreValueChange;
    [HideInInspector] public UnityEvent<int> onLifeValueChange;

    public void EndGame()
    {
        activeScene = SceneManager.GetActiveScene().name;
        GameObject.Find("Canvas").GetComponent<CanvasManager>().deathMenu.SetActive(true);
    }

    private static readonly string key = "\0"; //you can change this :D
    public void SaveGame()
    {
        //Getting Data
        playerInfoStored = true;
        Data.playerInfoStored = true;
        Data.curScene = SceneManager.GetActiveScene().name;
        Data.CurrentDate = DateTime.Now.ToString("f");

        Data.lives = lives;
        Data.score = score;

        Data.posX = currentCheckpoint.GetComponent<Checkpoints>().spawnPoint.position.x;
        Data.posY = currentCheckpoint.GetComponent<Checkpoints>().spawnPoint.position.y;
        Data.posZ = currentCheckpoint.GetComponent<Checkpoints>().spawnPoint.position.z;

        Data.rotX = currentCheckpoint.GetComponent<Checkpoints>().spawnPoint.rotation.x;
        Data.rotY = currentCheckpoint.GetComponent<Checkpoints>().spawnPoint.rotation.y;
        Data.rotZ = currentCheckpoint.GetComponent<Checkpoints>().spawnPoint.rotation.z;

        //Showing Data
        string playerDataJSON = EncryptDecrypt(JsonUtility.ToJson(Data));
        Debug.Log("Saved Game Data: " + JsonUtility.ToJson(Data) + " : " + playerDataJSON);
        System.IO.File.WriteAllText(Application.dataPath + "/Saves/0.sav", playerDataJSON);
    }

    public void ResetSaveData()
    {
        Data.posX = 0f;
        Data.posY = 0f;
        Data.posZ = 0f;

        Data.rotX = 0f;
        Data.rotY = 0f;
        Data.rotZ = 0f;

        Data.playerInfoStored = false;

        Data.curScene = "\0";

        Data.CurrentDate = "\0";
        Data.lives = 5;
        Data.score = 0;

        string playerDataJSON = EncryptDecrypt(JsonUtility.ToJson(Data));
        Debug.Log("Saved Game Data: " + JsonUtility.ToJson(Data) + " : " + playerDataJSON);
        System.IO.File.WriteAllText(Application.dataPath + "/Saves/0.sav", playerDataJSON);
    }

    /* This part may differ from your part but the method is the same*/
    public void LoadGame()
    {
        health = maxHealth;

        //Getting Save File
        string playerDataJSON = File.ReadAllText(Application.dataPath + "/Saves/0.sav");
        Debug.Log("Loading: " + EncryptDecrypt(playerDataJSON) + " : " + playerDataJSON);
        JsonUtility.FromJsonOverwrite(EncryptDecrypt(playerDataJSON), Data);

        playerInfoStored = Data.playerInfoStored;
    }

    private static string EncryptDecrypt(string data)
    {
        string result = "";
        for (int i = 0; i < data.Length; i++) 
            result += (char)(data[i] ^ key[i % key.Length]); 
        return result;
    }
}
