using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float smooth = 1f;

    Vector3 pos;
    Transform player;

    void Start()
    {
        player = FindObjectOfType<Player>().transform;
    }

    void Update()
    {
        if (player != null)
        {
            pos = player.position;
            pos.z = -10f;

            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime + smooth);
        }        
    }
}
