using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoamUIManager : MonoBehaviour
{
    [SerializeField] Slider hpSlider;
    [SerializeField] Slider mpSlider;
    [SerializeField] GameObject AlertWindow;
    [SerializeField] public TextMeshProUGUI playerLevel;

    public static RoamUIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeStatsUI();
    }

    public void UpdateHealth(float value)
    {
        hpSlider.value = value;
    }

    public void UpdateMana(float value)
    {
        mpSlider.value = value;
    }

    public void showAlertWindow(string message)
    {
        AlertWindow.GetComponentInChildren<TextMeshProUGUI>().text = message;
        AlertWindow.SetActive(true);
    }

    public void InitializeStatsUI()
    {
        hpSlider.maxValue = PlayerManager.Instance.playerStats.maxHP;
        hpSlider.minValue = 0;
        mpSlider.maxValue = PlayerManager.Instance.playerStats.maxMana;
        mpSlider.minValue = 0;
        UpdateHealth(PlayerManager.Instance.playerStats.currentHP);
        UpdateMana(PlayerManager.Instance.playerStats.currentMana);
        playerLevel.text = PlayerManager.Instance.playerStats.playerLvl.ToString();
    }
}
