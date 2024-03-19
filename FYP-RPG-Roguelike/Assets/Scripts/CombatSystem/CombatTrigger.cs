using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatTrigger : MonoBehaviour
{

    public List<GameObject> Enemies;
    public BoxCollider areaOfEffect;
    private static bool created = false;
    // Start is called before the first frame update
    void Start()
    {
        areaOfEffect = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCombat();
        }
    }

    public void StartCombat()
    {
        this.gameObject.transform.position = new Vector3(gameObject.transform.position.x, 999, gameObject.transform.position.z);
        if (PlayerManager.Instance.EnemiesInTrigger.Count >0)
            PlayerManager.Instance.EnemiesInTrigger.Clear();

        foreach(var enemy in Enemies)
        {
            PlayerManager.Instance.EnemiesInTrigger.Add(enemy);
        }
        PlayerManager.lastFightTrigger = this.gameObject;
    }

    public void DestroyCombatTrigger()
    {
        Destroy(this.gameObject);
    }

}
