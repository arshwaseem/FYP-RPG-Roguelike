using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerCombat : TriggerBasic
{
    public GameObject UI;
    public GameObject PlayerDeactivate;
    public GameObject CamToDeactivate;
    public GameObject EventToDeactivate;
    public AsyncOperation EnemySpawner;

    public List<GameObject> EnemiesToSpawn;

    public bool isActive()
    {
        return true;
    }

    public override void TriggerInvoke()
    {
        base.TriggerInvoke();
        StartCoroutine(CombatInit());
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
