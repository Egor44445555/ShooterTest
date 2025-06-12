using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] float interval = 0.5f;
    [SerializeField] float bulletSpeed = 10f;    
    [SerializeField] public Transform firePoint;
    [SerializeField] public GameObject bulletPref;
    [SerializeField] public Transform target;

    float nextFireTime;

    void Update()
    {
        transform.position = FindObjectOfType<Player>().weaponPoint.position;
        
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            Player player = FindObjectOfType<Player>();
            
            if (player.lastNonZeroDirection.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(player.lastNonZeroDirection.y, player.lastNonZeroDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            GameObject bulletObj = Instantiate(bulletPref, firePoint.position, firePoint.rotation);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            
            bullet.target = target;
            bullet.firePointPosition = firePoint.position;
            bullet.fireDirection = firePoint.right;
            
            nextFireTime = Time.time + interval;
        }
    }
}
