using System;
using UnityEngine;
using System.Collections;

public class PunchController : MonoBehaviour
{
	public float punchSpeed;
    public float retractSpeed;
    public float pushTerrainSpeed;
    public float pushPlayerSpeed;
    public Vector3 spriteOffset;
    public float maxPunchLength;
    public float minRetractDistance;

    private Transform playerTransform;
    private Vector2 impactForce;
	private Vector2 velocity;

    private GameObject player;
    private float chargeLevel = 1;
    private float maxCharge = 5.0f;
    public float chargeSpeed;
    
	// Use this for initialization
	void Start ()
	{
        impactForce = Vector2.zero;
        player = transform.parent.gameObject;
	    playerTransform = player.transform;

		Reset();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 movement = velocity * Time.deltaTime;
        transform.position += movement;
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.Equals(player) && punchState == PunchState.Retracting)
        {
			Reset();
        }
        else if (collider.gameObject.tag == "Terrain" && punchState == PunchState.Punching)
        {
            punchState = PunchState.Retracting;
            Vector2 pushAngle = player.transform.position - transform.position;
            player.rigidbody2D.velocity += pushAngle.normalized * pushTerrainSpeed * chargeLevel;
            chargeLevel = 1;
        }
        else if (collider.gameObject.tag == "Player" && punchState == PunchState.Punching)
        {
			punchState = PunchState.Retracting;
            collider.rigidbody2D.velocity += velocity.normalized * pushPlayerSpeed * chargeLevel;
			player.rigidbody2D.velocity += -velocity.normalized * pushPlayerSpeed/2;
        }
        else if (collider.gameObject.tag == "Puncher" && punchState == PunchState.Punching)
        {
            punchState = PunchState.Retracting;
            impactForce = collider.gameObject.GetComponent<PunchController>().velocity.normalized;
        }
		else {
			Reset();
		}
    }

    private void Fire()
    {
        if (playerInput.aimAngle.magnitude > 0)
        {
            punchState = PunchState.Punching;
            velocity = playerInput.aimAngle * punchSpeed;
        }
    }

	private void Reset() {
		punchState = PunchState.Ready;
		transform.position = playerTransform.position + spriteOffset;
		velocity = Vector2.zero;
        GetComponentInChildren<String>().Reset();
	}

}
