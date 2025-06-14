using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] float interval = 0.5f;
    [SerializeField] public Transform firePoint;
    [SerializeField] public GameObject bulletPref;
    [SerializeField] public Transform target;
    [SerializeField] public int maxBulletCount = 10;

    [HideInInspector] public int bulletCount = 0;

    float nextFireTime;

    void Start()
    {
        bulletCount = maxBulletCount;
        UIManager.main.AmmoCountCheck();
    }

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
        if (Time.time >= nextFireTime && bulletCount > 0)
        {
            GameObject bulletObj = Instantiate(bulletPref, firePoint.position, firePoint.rotation);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            
            bullet.target = target;
            bullet.firePointPosition = firePoint.position;
            bullet.fireDirection = FindObjectOfType<Player>().aimDirection;
            bulletCount -= 1;
            UIManager.main.AmmoCountCheck();
            nextFireTime = Time.time + interval;
        }
    }
}
