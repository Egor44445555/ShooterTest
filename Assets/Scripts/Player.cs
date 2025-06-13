using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public float speed = 3f;
    [SerializeField] float detectionRadius = 3f;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] public Transform weaponPoint;

    [SerializeField] public int inventorySizeWidth = 5;
    [SerializeField] public int inventorySizeHeight = 3;
    [SerializeField] public List<InventoryItem> inventoryItems = new List<InventoryItem>();

    [HideInInspector] public Vector2 lastJoystickDirection;
    [HideInInspector] public Vector2 lastNonZeroDirection = Vector2.right;
    [HideInInspector] public Vector2 aimDirection;

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

        if (currentTarget != null)
        {
            Vector3 targetCenter = currentTarget.GetComponent<Collider2D>().bounds.center;
            aimDirection = (targetCenter - weapon.transform.position).normalized;
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
        bool facingLeft = direction.x < 0;

        if (weapon.GetComponent<SpriteRenderer>() != null)
        {
            weapon.localScale = new Vector3(facingLeft ? -1f : 1f, 1f, 1f);
            float correctedAngle = facingLeft ? (180f + angle) : angle;            
            weapon.localEulerAngles = new Vector3(0, 0, correctedAngle);
        }
    }

    void UpdateSpriteDirections(Vector2 direction)
    {
        foreach (SpriteRenderer sprite in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            if (sprite.GetComponent<Weapon>() == null)
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
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Thing"))
        {
            bool found = false;
            int itemSlotX = 0;
            int itemSlotY = 0;

            for (int x = 0; x < inventorySizeWidth && !found; x++)
            {
                for (int y = 0; y < inventorySizeHeight && !found; y++)
                {
                    Vector2Int slotPosition = new Vector2Int(x, y);
                    InventoryItem occupiedSlotPosition = Array.Find<InventoryItem>(inventoryItems.ToArray(), item => item.gridPosition == slotPosition);

                    if (occupiedSlotPosition == null)
                    {
                        itemSlotX = x;
                        itemSlotY = y;
                        found = true;
                    }
                }
            }

            Vector2Int position = new Vector2Int(itemSlotX, itemSlotY);
            inventoryItems.Add(new InventoryItem(other.gameObject.name, position, other.gameObject.GetComponent<SpriteRenderer>().sprite));
            Destroy(other.gameObject);
        }
    }
}
