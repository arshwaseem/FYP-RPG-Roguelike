using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class cutsceneController : MonoBehaviour
{

    public GameObject Loader;
    public VideoPlayer player;
    public TextMeshProUGUI loadText;
    public Slider loadSlider;

    private void Start()
    {
        player.GetComponent<VideoPlayer>().loopPointReached += Player_loopPointReached;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Player_loopPointReached(VideoPlayer source)
    {
        SceneManager.LoadScene("FullDemo");
    }
}
