using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 4f;
    [SerializeField] public float damage = 3f;

    [HideInInspector] public Transform target;

    Rigidbody2D rb;
    Player player;
    Transform startPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        startPoint = transform;
    }

    void Update()
    {
        Vector2 shootDirection;

        if (target != null)
        {
            shootDirection = (target.position - startPoint.position).normalized;
        }
        else
        {
            shootDirection = player.lastJoystickDirection.magnitude > 0.1f ? player.lastJoystickDirection : (Vector2)startPoint.position;
        }

        rb.velocity = shootDirection * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
