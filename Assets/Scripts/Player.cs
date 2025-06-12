using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public float speed = 3f;
    [SerializeField] float detectionRadius = 3f;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] public Transform weaponPoint;

    public Vector2 lastJoystickDirection;
    Vector2 lastNonZeroDirection = Vector2.right;

    Rigidbody2D rb;
    FloatingJoystick joystick;
    Animator anim;
    Transform currentTarget;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        joystick = FindObjectOfType<FloatingJoystick>();
    }

    public void Update()
    {
        Vector2 direction = new Vector2(joystick.Horizontal, joystick.Vertical);
        lastJoystickDirection = direction;

        if (direction.magnitude > 0.001f)
        {
            lastNonZeroDirection = direction.normalized;
            anim.SetBool("Run", true);
            rb.velocity = lastNonZeroDirection * speed;
        }
        else
        {
            anim.SetBool("Run", false);
            rb.velocity = Vector2.zero;
        }

        if (currentTarget != null)
        {
            direction = (Vector2)currentTarget.position;
            lastNonZeroDirection = direction.normalized;
        }

        foreach (SpriteRenderer sprite in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            if (sprite.name != "Gun" && sprite.name != "AK")
            {
                sprite.flipX = lastNonZeroDirection.x < 0;
            }
            else if (sprite.name == "Gun" || sprite.name == "AK")
            {
                float targetRotationY = lastNonZeroDirection.x < 0 ? 180f : 0f;
                sprite.transform.localEulerAngles = new Vector3(0f, targetRotationY, 0f);
            }
        }

        FindNearestTarget();
    }

    public void Action()
    {
        transform.GetComponentInChildren<Weapon>().Fire();
    }

    void FindNearestTarget()
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
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
