using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int health = 5;

    public float Speed = 0.125f;

    public float moveThreshold = 0.5f;

    public float animSpeed = 0.125f;

    public Sprite[] moveSprites;

    public Sprite[] jumpSprites;

    public float knockbackStunLength = 0.1f;

    public float knockbackSpeed = 0.25f;

    private Rigidbody rb;

    private int facing;

    private int animFrame;

    private SpriteRenderer sprite;

    private float animTimer;

    private float knockbackStunTimer;

    private Vector3 knockbackDirection;

    // Use this for initialization
    void Start () {
		rb = GetComponent<Rigidbody>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        facing = 0;
        animFrame = 0;
        animTimer = animSpeed;
        knockbackStunTimer = 0;
    }

    // Update is called once per frame
    void FixedUpdate() {
        // Reset the velocity and angular velocity to remove any weird applied forces
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (knockbackStunTimer > 0)
        {
            rb.position += knockbackDirection * knockbackSpeed;
            knockbackStunTimer -= Time.fixedDeltaTime;
        }
        else
        {
            // Movement
            UpdateMove();
        }
	}

    /// <summary>
    /// The movement component of the game's regular updates.
    /// Handles calculations for the character's movement and moving animations.
    /// </summary>
    private void UpdateMove()
    {
        Vector3 move = Vector3.zero;

        bool[] d = { false, false, false, false };

        if (Input.GetAxisRaw("Vertical") > 0)
        {
            move.z = Speed;
            d[0] = true;
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            move.z = -Speed;
            d[1] = true;
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            move.x = Speed;
            d[2] = true;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            move.x = -Speed;
            d[3] = true;
        }

        if (d[0] && !d[2] && !d[3])
        {
            facing = 2;
        }
        else if (d[1] && !d[2] && !d[3])
        {
            facing = 0;
        }
        else if (!d[0] && !d[1] && d[2])
        {
            facing = 1;
        }
        else if (!d[0] && !d[1] && d[3])
        {
            facing = 3;
        }
        if (d[0] || d[1] || d[2] || d[3])
        {
            animTimer -= Time.fixedDeltaTime;
            if (animTimer <= 0)
            {
                animFrame++;
                animFrame %= 2;
                resetAnimTimer();
            }
        }
        else
        {
            if (facing == 1 || facing == 3)
            {
                animFrame = 0;
            }
        }

        sprite.sprite = moveSprites[facing * 2 + animFrame];
        //UpdateSprite();

        //Transform swordParent = GetComponentsInChildren<Transform>();

        rb.position += move;
    }

    private void resetAnimTimer()
    {
        animTimer = animSpeed;
    }

    /*
    void OnCollisionEnter(Collision other)
    {
        if(other.CompareTag("Enemy"))
        {
            takeDamage(1);
        }
    }
    */

    public void takeDamage(int amount)
    {
        health -= amount;
        if(health <= 0)
        {
            GameManager.instance.gameOver();
        }
    }

    public void Knockback(Vector3 direction)
    {
        knockbackStunTimer = knockbackStunLength;
        knockbackDirection = direction.normalized;
    }
}
