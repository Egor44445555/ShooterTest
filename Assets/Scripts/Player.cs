using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public float speed = 3f;
    [SerializeField] float detectionRadius = 3f;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] public Transform weaponPoint;

    [HideInInspector] public Vector2 lastJoystickDirection;
    [HideInInspector] public Vector2 lastNonZeroDirection = Vector2.right;

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

    void Update()
    {
        Vector2 direction = new Vector2(joystick.Horizontal, joystick.Vertical);
        lastJoystickDirection = direction;

        Weapon weapon = transform.GetComponentInChildren<Weapon>();
        weapon.target = currentTarget;

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

        Vector2 aimDirection;

        if (currentTarget != null)
        {
            aimDirection = (currentTarget.position - weapon.transform.position).normalized;
            weapon.Fire();
        }
        else
        {
            aimDirection = (direction.magnitude > 0.001f) ? direction.normalized : lastNonZeroDirection;
        }

        RotateWeapon(weapon.transform, aimDirection);
        UpdateSpriteDirections(aimDirection);
        FindTarget();
    }

    void RotateWeapon(Transform weapon, Vector2 direction)
    {
        if (direction == Vector2.zero) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        if (weapon.GetComponent<SpriteRenderer>() != null)
        {
            bool facingLeft = direction.x < 0;            
            weapon.localEulerAngles = new Vector3(0f, facingLeft ? 180f : 0f, 0f);            
            weapon.GetChild(0).localEulerAngles = new Vector3(0f, 0f, facingLeft ? -angle : angle);
        }
        else
        {
            weapon.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void UpdateSpriteDirections(Vector2 direction)
    {
        foreach (SpriteRenderer sprite in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            if (sprite.name != "Gun" && sprite.name != "AK")
            {
                sprite.flipX = direction.x < 0;
            }
        }
    }

    public void Action()
    {
        transform.GetComponentInChildren<Weapon>().Fire();
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
