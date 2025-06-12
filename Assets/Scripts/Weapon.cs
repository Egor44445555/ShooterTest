using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float bulletSpeed = 10f;
    
    [SerializeField] public Transform firePoint;
    [SerializeField] public GameObject bulletPref;
    [SerializeField] public float interval = 1f;    
    [SerializeField] public Transform target;

    float nextFireTime;

    void Update()
    {
        transform.position = FindObjectOfType<Player>().weaponPoint.position;
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            GameObject bullet = bulletPref;
            bullet.GetComponent<Bullet>().target = target;
            Instantiate(bullet, firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + fireRate;
        }
    }
}
