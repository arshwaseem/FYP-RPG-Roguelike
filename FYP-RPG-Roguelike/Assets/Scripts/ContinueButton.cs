using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    public Button contBut;

    private void Start()
    {
        contBut.onClick.AddListener(cont);
    }
    public void cont()
    {
        SceneManager.LoadScene("DebugLevel");
    }
}
