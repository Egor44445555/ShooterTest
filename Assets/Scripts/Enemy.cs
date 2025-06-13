using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public float damage = 5f;
    [SerializeField] public float intervalAttack = 0.5f;
    [SerializeField] public float attackDistance = 0.5f;
    [SerializeField] public float speed = 3f;
    [SerializeField] float detectionRadius = 3f;
    [SerializeField] LayerMask targetLayer;

    Rigidbody2D rb;
    Animator anim;
    Transform currentTarget;
    bool attack = false;
    float nextAttackTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public void Update()
    {
        FindTarget();

        if (currentTarget != null)
        {
            Vector2 targetCenter = currentTarget.GetComponent<Collider2D>().bounds.center;
            Vector2 direction = (targetCenter - (Vector2)GetComponent<Collider2D>().bounds.center).normalized;

            if (direction.magnitude > 0.001f)
            {
                anim.SetBool("Run", true);

                if (!attack)
                {
                    rb.velocity = direction * speed;
                }

                if (direction.x > 0)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else if (direction.x < 0)
                {
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                }
            }
            else
            {
                anim.SetBool("Run", false);
                rb.velocity = Vector2.zero;
            }

            float distance = Vector2.Distance(GetComponent<Collider2D>().bounds.center, targetCenter);

            if (distance <= attackDistance && Time.time >= nextAttackTime)
            {
                attack = true;
                currentTarget.GetComponent<Health>().TakeDamage(damage);
                nextAttackTime = Time.time + intervalAttack;
            }
            else if (distance > attackDistance)
            {
                attack = false;
            }
        }
        else
        {
            anim.SetBool("Run", false);
            rb.velocity = Vector2.zero;
        }
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
