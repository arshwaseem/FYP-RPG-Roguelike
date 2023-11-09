using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowCamera : MonoBehaviour
{

    [SerializeField] GameObject Camera;
    Vector3 offset;
    GameObject player;
    Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerPlaceHolder");
        playerTransform = player.GetComponent<Transform>();
        offset = Camera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerTransform.position + offset;
    }
}
