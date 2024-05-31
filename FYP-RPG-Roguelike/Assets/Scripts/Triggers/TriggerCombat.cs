using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TriggerCombat : TriggerBasic
{
    public GameObject UI;
    public GameObject PlayerDeactivate;
    public GameObject CamToDeactivate;
    public GameObject EventToDeactivate;
    public AsyncOperation EnemySpawner;
    public GameObject Fader;
    public GameObject alWin;

    public List<GameObject> EnemiesToSpawn;

    public bool isActive()
    {
        return true;
    }

    public override void TriggerInvoke()
    {
        base.TriggerInvoke();
        Fader.SetActive(true);
        StartCoroutine(FadeIn());
    }

    public bool DeactivateRoamObjects()
    {
        UI.SetActive(false);
        PlayerDeactivate.SetActive(false);
        CamToDeactivate.SetActive(false);
        EventToDeactivate.SetActive(false);
        return true;
    }

    public bool ReactivateRoamObjects()
    {
        UI.SetActive(true);
        PlayerDeactivate.SetActive(true);
        CamToDeactivate.SetActive(true);
        EventToDeactivate.SetActive(true);
        return true;
    }

    public IEnumerator FadeIn ()
    {
        Color fadeColor = Fader.GetComponent<Image>().color;

        while( Fader.GetComponent<Image>().color.a < 1)
        {
            fadeColor = new Color(0, 0, 0, fadeColor.a + (2 * Time.deltaTime));
            Fader.GetComponent<Image>().color = fadeColor;
            yield return null;
        }

        StartCoroutine(CombatInit());

        yield return null;
    }

    public IEnumerator CombatInit()
    {
        yield return new WaitUntil(() => DeactivateRoamObjects() == true);
        PlayerManager.Instance.currentTrigger = this;
        AsyncOperation _combatLoad = SceneManager.LoadSceneAsync("CombatScene", LoadSceneMode.Additive);
        _combatLoad.allowSceneActivation = false;
        while (!_combatLoad.isDone)
        {
            yield return null;

            if (_combatLoad.progress >= 0.9f)
            {
                _combatLoad.allowSceneActivation = true;
                alWin.GetComponentInChildren<TextMeshProUGUI>().text = "Wait For Your Time Bar To Fill, Then Click on Enemy To Select and Perform an Attack or Magic";
                alWin.SetActive(true);
                Fader.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                Fader.SetActive(false);

            }
        };

        }

    public bool spawnEnemies()
    {
        GameObject[] placeHolders = GameObject.FindGameObjectsWithTag("EnemyPlaceHolder");
        Debug.Log(placeHolders.Length);
        for (int i = 0; i < placeHolders.Length; i++)
        {
            GameObject _prefab = EnemiesToSpawn[i];
            Debug.Log("Spawning: " + _prefab.GetComponent<CombatCharacterController>().characterData.charName);
            Instantiate(_prefab, placeHolders[i].transform.position, placeHolders[i].transform.rotation);
        }

        return true;
    }


}
