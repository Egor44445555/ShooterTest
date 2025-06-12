using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] public float maxHitPoint = 2f;
    [SerializeField] public GameObject healthBlock;
    [SerializeField] public Image healthBar;

    [HideInInspector] public bool isDestroyed = false;
    [HideInInspector] public float hitPoint = 2f;

    void Start()
    {
        hitPoint = maxHitPoint;
    }

    void Update()
    {
        healthBar.fillAmount = hitPoint / maxHitPoint;

        if (hitPoint < maxHitPoint)
        {
            healthBlock.SetActive(true);
        }

        if (hitPoint <= 0 && !isDestroyed)
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
    
    public void TakeDamage(float dmg)
    {
        hitPoint -= dmg;
    }
}
