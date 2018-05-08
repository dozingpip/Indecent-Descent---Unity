using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FloorType { Normal, Ice, Sticky, Cracked, None };

public class Player : MonoBehaviour {

	public static Player instance = null;

	public int health = 5;

	public float Speed = 0.125f;

	public float jumpSpeed = 0.5f;

	public float moveThreshold = 0.5f;

	public float animSpeed = 0.125f;

	public Sprite[] moveSprites;

	public Sprite[] jumpSprites;

	public float knockbackStunLength = 0.1f;

	public float knockbackSpeed = 0.25f;

	public float gravityStrength = 1;

	public float iceFriction = 0.5f;

	public float terminalVelocity = -1;

	private Rigidbody rb;

	private int facing;

	private int animFrame;

	private SpriteRenderer sprite;

	private float animTimer;

	private float knockbackStunTimer;

	private Vector3 knockbackDirection;

	private Vector3 lateralVelocity;

	private float yVelocity;

	private bool isGrounded;

	private FloorType standing;

	private List<GameObject> standingOn;

	//Awake is always called before any Start functions
	void Awake()
	{
		//Check if instance already exists
		if (instance == null)
			//if not, set instance to this
			instance = this;

		//If instance already exists and it's not this:
		else if (instance != this)
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);
	}

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		sprite = GetComponentInChildren<SpriteRenderer>();
		facing = 0;
		animFrame = 0;
		animTimer = animSpeed;
		knockbackStunTimer = 0;
		isGrounded = false;
		standing = FloorType.None;
		lateralVelocity = new Vector3();
		standingOn = new List<GameObject>();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
		{
			yVelocity = jumpSpeed;
			isGrounded = false;
			animFrame = 0;
		}
	}

	// Update is called once per frame
	void FixedUpdate() {
		// Reset the velocity and angular velocity to remove any weird applied forces
		if(standing == FloorType.Ice)
		{

				lateralVelocity *= (1 - (iceFriction * 5));
		}
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
		Vector3 move = lateralVelocity;

		int i = 0;
		while (i < standingOn.Count)
		{
			GameObject tile = standingOn[i];
			if (!tile.activeSelf)
			{
				standingOn.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}

		//Debug.Log(standingOn);

		if (standingOn.Count == 0)
		{
			//Debug.Log("Not Grounded");
			standing = FloorType.None;
			isGrounded = false;
		}
		else
		{
			isGrounded = true;
		}

		

		//if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
		//{
		//	yVelocity = jumpSpeed;
		//	isGrounded = false;
		//	animFrame = 0;
		//}

		bool[] d = { false, false, false, false };

		Vector3 lateralMovement = new Vector3();

		if (Input.GetAxisRaw("Vertical") > 0)
		{
			lateralMovement.z = Speed;
			d[0] = true;
		}
		else if (Input.GetAxisRaw("Vertical") < 0)
		{
			lateralMovement.z = -Speed;
			d[1] = true;
		}
		if (Input.GetAxisRaw("Horizontal") > 0)
		{
			lateralMovement.x = Speed;
			d[2] = true;
		}
		else if (Input.GetAxisRaw("Horizontal") < 0)
		{
			lateralMovement.x = -Speed;
			d[3] = true;
		}

		switch(standing)
		{
			case FloorType.Ice:
				lateralVelocity += (lateralMovement.normalized * iceFriction);
				lateralVelocity.x = Mathf.Clamp(lateralVelocity.x, -Speed * 1.5f, Speed * 1.5f);
				lateralVelocity.z = Mathf.Clamp(lateralVelocity.z, -Speed * 1.5f, Speed * 1.5f);
				break;
			case FloorType.Sticky:
				lateralVelocity = lateralMovement/2;
				break;
			default:
				lateralVelocity = lateralMovement;
				break;

		}
		move = lateralVelocity;

		
		// Handles the sprite
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

		if (!isGrounded)
		{
			animTimer -= Time.fixedDeltaTime;
			if (animTimer <= 0)
			{
				animFrame++;
				animFrame %= 3;
				resetAnimTimer();
			}
			sprite.sprite = jumpSprites[facing * 3 + animFrame];
		}
		else
		{
			if (d[0] || d[1] || d[2] || d[3])
			{
				if(standing == FloorType.Sticky)
				{
					animTimer -= Time.fixedDeltaTime/2;
				}
				else
				{
					animTimer -= Time.fixedDeltaTime;
				}
				if (animTimer <= 0)
				{
					animFrame++;
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
			animFrame %= 2;
			sprite.sprite = moveSprites[facing * 2 + animFrame];
		}
		//UpdateSprite();

		//Transform swordParent = GetComponentsInChildren<Transform>();
		move.y = yVelocity;
		if(!isGrounded)
		{
			yVelocity -= gravityStrength * Time.fixedDeltaTime;
			if(yVelocity < terminalVelocity)
			{
				yVelocity = terminalVelocity;
			}
		}
		
		//Debug.Log("yVelocity " + yVelocity);
		rb.position += move;
		//isGrounded = false;
	}

	private void resetAnimTimer()
	{
		animTimer = animSpeed;
	}

	void OnCollisionEnter(Collision other)
	{

		/*
		if(other.collider.tag.Contains("Enemy"))
		{
			takeDamage(1);
		}
		*/
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag.Contains("Tile"))
		{
			isGrounded = true;
			yVelocity = 0;
			standingOn.Add(other.gameObject);
		}
		if (other.tag.Contains("Cracked"))
		{
			standing = FloorType.Cracked;
			CrackedTile crackedFloor = other.gameObject.GetComponent<CrackedTile>();
			if (!crackedFloor.isCracking())
			{
				crackedFloor.steppedOn();
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		//bool pitFall = false;
		if (other.tag.Contains("Normal"))
		{
			standing = FloorType.Normal;
		}
		else if (other.tag.Contains("Ice"))
		{
			standing = FloorType.Ice;
		}
		else if(other.tag.Contains("Mud"))
		{
			standing = FloorType.Sticky;
		}
		else if(other.tag.Contains("Cracked"))
		{
			standing = FloorType.Cracked;
			//CrackedTile crackedFloor = other.gameObject.GetComponent<CrackedTile>();
			//if(!crackedFloor.isCracking())
			//{
			//	crackedFloor.steppedOn();
			//}
			//if(!crackedFloor.gameObject.activeSelf)
			//{
			//	isGrounded = false;
			//	pitFall = true;
			//}
		}
		//Debug.Log("Standing " + standing);
		//if(!pitFall && (other.tag.Contains("Tile")) && !isGrounded)
		//{
		//	Debug.Log("hit floor");
		//	isGrounded = true;
		//	yVelocity = 0;
		//}
	}

	void OnTriggerExit(Collider other)
	{
		if((other.tag.Contains("Tile")))
		{
			standingOn.Remove(other.gameObject);
		}
	}

	public void takeDamage(int amount)
	{
		if(knockbackStunTimer <= 0)
		{
			health -= amount;
			if (health <= 0)
			{
				GameManager.instance.gameOver();
			}
		}
	}

	public void Knockback(Vector3 direction)
	{
		knockbackStunTimer = knockbackStunLength;
		knockbackDirection = direction.normalized;
	}

	public void checkFooting()
	{
		standing = FloorType.None;
		isGrounded = false;
	}
}
