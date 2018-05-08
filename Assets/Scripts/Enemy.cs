using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	LineRenderer laser;

	public Material trackingMaterial;

	public Material firingMaterial;

	public float verticalTrackingRange = 2;

	public float trackingWidth = 0.05f;

	public float firingWidth = 0.5f;

	public float lockOnTime = 3;

	public float fireTime = 1.5f;

	public float fireLength = 0.2f;

	public float laserLength = 20f;

	private float laserTimer;

	// Use this for initialization
	void Start () {
		laser = GetComponent<LineRenderer>();
		laser.SetPosition(0, transform.position);
		laser.startWidth = trackingWidth;
		laser.material = trackingMaterial;
		laser.enabled = false;

		lockOnTime *= 1 - ((0.05f) * Mathf.Sqrt(GameManager.floor));

		laserTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Player.instance.transform.position.y < transform.position.y + verticalTrackingRange && Player.instance.transform.position.y > transform.position.y - verticalTrackingRange)
		{
			laser.enabled = true;
			laserTimer += Time.deltaTime;
			if(laserTimer >= lockOnTime)
			{
				laser.material = firingMaterial;
				if(laserTimer >= lockOnTime + fireTime)
				{
					laser.startWidth = firingWidth;
					Vector3 direction = laser.GetPosition(1) - laser.GetPosition(0);
					RaycastHit hit;
					if (Physics.Raycast(laser.GetPosition(0), direction, out hit, 20))
					{
						if(hit.collider.CompareTag("Player"))
						{
							Player player = hit.collider.gameObject.GetComponent<Player>();
							player.takeDamage(1);
							player.Knockback(direction);
						}
					}
					if(laserTimer >= lockOnTime + fireTime + fireLength)
					{
						laserTimer = 0;
					}
				}
			}
			else
			{

				laser.startWidth = trackingWidth;
				Vector3 targetPosition = Player.instance.transform.position;
				targetPosition.y = transform.position.y;
				Vector3 direction = targetPosition - laser.GetPosition(0);
				direction = direction.normalized * laserLength;
				targetPosition = direction + laser.GetPosition(0);
				laser.SetPosition(1, targetPosition);
				laser.material = trackingMaterial;
			}
		}
		else
		{
			laserTimer = 0;
			laser.enabled = false;
		}
	}
}
