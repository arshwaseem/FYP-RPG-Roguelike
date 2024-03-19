using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowCamera : MonoBehaviour
{
    Vector3 offset;
    GameObject player;
    [SerializeField]Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        offset = gameObject.transform.position - playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerTransform.position + offset;
    }
}
