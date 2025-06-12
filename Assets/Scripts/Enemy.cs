using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public float speed = 3f;
    [SerializeField] float detectionRadius = 3f;
    [SerializeField] LayerMask targetLayer;

    Rigidbody2D rb;
    Animator anim;
    Transform currentTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public void Update()
    {
        if (currentTarget != null)
        {
            Vector2 direction = (currentTarget.position - transform.position).normalized;

            if (direction.magnitude > 0.001f)
            {
                anim.SetBool("Run", true);
                rb.velocity = currentTarget.position * speed;
            }
            else
            {
                anim.SetBool("Run", false);
                rb.velocity = Vector2.zero;
            }
        }

        FindTarget();
    }

    void FindTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, detectionRadius, targetLayer);
        float minDistance = Mathf.Infinity;

        currentTarget = null;

        foreach (Collider2D target in targets)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                currentTarget = target.transform;

                if (transform.GetComponentInChildren<Weapon>())
                {
                    transform.GetComponentInChildren<Weapon>().target = currentTarget;
                }                
            }
        }
    }
}
