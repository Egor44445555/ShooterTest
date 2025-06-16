using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    [SerializeField] public float speed = 3f;
    [SerializeField] float detectionRadius = 3f;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] public Transform weaponPoint;
    [SerializeField] public GameObject[] weapons;

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
    int currentWeapon = 0;
    float correctedAngleWeapon;
    Vector3 basedScaleWeapon;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        joystick = FindObjectOfType<FloatingJoystick>();

        PlayerSaveData playerSaveData = JsonSave.main.LoadPlayerData();
        List<InventoryItem> newInventoryItem = new List<InventoryItem>();

        basedScaleWeapon = transform.GetComponentInChildren<Weapon>().transform.localScale;

        foreach (InventoryItem item in inventoryItems)
        {
            item.name = item.name + "_" + item.uniqueId;
        }

        if (playerSaveData != null)
        {
            if (playerSaveData.inventoryItem == null || playerSaveData.inventoryItem.Length == 0)
            {
                foreach (InventoryItem item in inventoryItems)
                {
                    newInventoryItem.Add(new InventoryItem(item.name, item.gridPosition, item.image, item.uniqueId));
                }
            }
            else
            {
                foreach (InventoryItem item in playerSaveData.inventoryItem)
                {
                    newInventoryItem.Add(new InventoryItem(item.name, item.gridPosition, item.image, item.uniqueId));
                }
            }

            inventoryItems = newInventoryItem;
        }

        JsonSave.main.SavePlayerData(inventoryItems.ToArray());
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
            weapon.localScale = basedScaleWeapon;
            weapon.localEulerAngles = new Vector3(0, 0, angle);
            weapon.localScale = new Vector3(weapon.localScale.x, facingLeft ? -weapon.localScale.y : weapon.localScale.y, weapon.localScale.z);
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

    public void ChangeWeapon()
    {
        currentWeapon = (currentWeapon >= weapons.Length - 1) ? 0 : currentWeapon + 1;

        Weapon oldWeapon = transform.GetComponentInChildren<Weapon>();
        Vector3 oldPosition = oldWeapon.transform.position;
        
        Vector2 currentAimDirection = aimDirection;
        
        Destroy(oldWeapon.gameObject);
        
        GameObject newWeapon = Instantiate(weapons[currentWeapon], oldPosition, Quaternion.identity, transform);
        
        newWeapon.transform.localScale = Vector3.one;
        newWeapon.transform.localRotation = Quaternion.identity;        
        RotateWeapon(newWeapon.transform, currentAimDirection);
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
            Weapon weapon = transform.GetComponentInChildren<Weapon>();
            bool found = false;
            Vector2Int freeSlotPosition = Vector2Int.zero;

            for (int x = 0; x < inventorySizeWidth && !found; x++)
            {
                for (int y = 0; y < inventorySizeHeight && !found; y++)
                {
                    Vector2Int currentPosition = new Vector2Int(x, y);
                    
                    bool isOccupied = inventoryItems.Any(item => item.gridPosition == currentPosition);
                    
                    if (!isOccupied)
                    {
                        freeSlotPosition = currentPosition;
                        found = true;
                    }
                }
            }

            if (found && other.gameObject.GetComponent<Ammunition>() == null)
            {
                inventoryItems.Add(new InventoryItem(other.gameObject.name + "_" + System.Guid.NewGuid().ToString(), freeSlotPosition, other.gameObject.GetComponent<SpriteRenderer>().sprite, System.Guid.NewGuid().ToString()));
                JsonSave.main.SavePlayerData(inventoryItems.ToArray());
            }

            if (other.gameObject.GetComponent<Ammunition>())
            {
                weapon.bulletCount += other.gameObject.GetComponent<Ammunition>().ammo;
                weapon.bulletCount = weapon.bulletCount > weapon.maxBulletCount ? weapon.maxBulletCount : weapon.bulletCount;
                UIManager.main.AmmoCountCheck();
            }

            Destroy(other.gameObject);
        }
    }
    
    InventorySlot FindNearestFreeSlot(Dictionary<Vector2, InventorySlot> slots, Vector2 originalPosition)
    {
        var orderedSlots = slots.Values
            .Where(s => !s.isOccupied)
            .OrderBy(s => Vector2.Distance(originalPosition, s.gridPosition))
            .ToList();

        return orderedSlots.FirstOrDefault();
    }
}
