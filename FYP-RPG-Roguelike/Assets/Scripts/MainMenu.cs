using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Slider LoadBar;
    public TextMeshProUGUI LoadText;
    public GameObject LoadScreen;
    public Button PlayButton;
    public Button QuitButton;

    private void Start()
    {
        QuitButton.onClick.AddListener(exitGame);
        PlayButton.onClick.AddListener(startGame);
    }

    public void exitGame()
    {
        Application.Quit();
    }

    private void startGame()
    {
        LoadScreen.SetActive(true);
        StartCoroutine(playGame());
    }

    IEnumerator playGame()
    {
        yield return null;

        AsyncOperation loadingScreen = SceneManager.LoadSceneAsync("FullDemo");
        loadingScreen.allowSceneActivation = false;

        while(!loadingScreen.isDone)
        {
            LoadText.text = "Now Loading...";
            LoadBar.value = loadingScreen.progress;

            if(loadingScreen.progress >= 0.9f)
            {
                LoadBar.value = 1f;
                LoadText.text = "Press Any Key To Continue";
                if (Input.anyKeyDown)
                {
                    loadingScreen.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}
