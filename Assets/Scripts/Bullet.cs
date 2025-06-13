using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 4f;
    [SerializeField] public float damage = 3f;

    [HideInInspector] public Transform target;
    [HideInInspector] public Vector2 fireDirection;
    [HideInInspector] public Vector2 firePointPosition;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Launch();
    }
    
    void Launch()
    {
        Vector2 shootDirection;

        if (target != null)
        {
            Vector2 targetCenter = target.GetComponent<Collider2D>().bounds.center;
            shootDirection = (targetCenter - firePointPosition).normalized;
        }
        else
        {
            shootDirection = fireDirection.normalized;
        }

        rb.velocity = shootDirection * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Health>() && collision.gameObject.GetComponent<Player>() == null)
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
        }

        Destroy(gameObject);       
    }
}
