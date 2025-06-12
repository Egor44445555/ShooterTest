using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 3f;

    Rigidbody2D rb;
    FloatingJoystick joystick;
    Animator anim;
    SpriteRenderer sprite;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        joystick = FindObjectOfType<FloatingJoystick>();
    }

    public void Update()
    {
        Vector2 direction = new Vector2(joystick.Horizontal, joystick.Vertical);

        if (direction.magnitude > 0.001f)
        {
            anim.SetBool("Run", true);
            direction.Normalize();
        }
        else
        {
            anim.SetBool("Run", false);
        }

        foreach (SpriteRenderer sprite in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.flipX = direction.x < 0;
        }

        rb.velocity = direction * speed;
    }

    public void Action()
    {
        print("Action");
    }
}
