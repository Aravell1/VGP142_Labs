using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    private bool loadScene = false;
    public string loadingSceneName;
    public TMP_Text loadingText;
    public Slider sliderBar;

    private void Start()
    {
        if (!loadScene)
        {
            loadScene = true;

            GameManager.Instance.LoadGame();

            if (GameManager.Instance.playerInfoStored)
                loadingSceneName = GameManager.Instance.Data.curScene;

            loadingText.text = "Loading...";

            StartCoroutine(LoadNewScene(loadingSceneName));
        }
    }

    IEnumerator LoadNewScene(string sceneName)
    {
        yield return new WaitForSeconds(1);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        while (!async.isDone)
        {
            float progress = Mathf.Clamp01(async.progress / 0.9f);
            sliderBar.value = progress;
            loadingText.text = progress * 100f + "%";
            yield return null;
        }
    }
}
