using System;
using UnityEngine;
using System.Collections;

public class PunchController : MonoBehaviour
{

    public enum PunchState
    {
        Punching,
        Retracting,
        Ready,
        Charging
    }

    private PunchState punchState;
	private Vector2 velocity;

	public float punchSpeed;
    public float retractSpeed;
    public float pushTerrainSpeed;
    public float pushPlayerSpeed;
    public Vector3 spriteOffset;
    public float maxPunchLength;
    public float minRetractDistance;

    private Transform playerTransform;
    private PlayerInput playerInput;
    private Vector2 impactForce;

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
        playerInput = player.GetComponent<PlayerInput>();

		Reset();
	}
	
	// Update is called once per frame
	void Update () {
	    if (punchState == PunchState.Ready)
	    {
            Vector3 newRot = Vector3.zero;
            newRot.z = Vector2.Angle(Vector2.right, playerInput.aimAngle);
            if (playerInput.aimAngle.y < 0) {
                newRot.z = 360 - newRot.z;
            }
            transform.localEulerAngles = newRot;

	        if (playerInput.inputFireDown)
	        {
                punchState = PunchState.Charging;
	        }
            else
            {
                transform.position = playerTransform.position + spriteOffset;
            }
	    }
        else if (punchState == PunchState.Charging) {
            if (playerInput.inputFireDown)
            {
                chargeLevel += Time.deltaTime * chargeSpeed;
            }
            else if (playerInput.inputFireUp)
            {
                Fire();
            }
        }
        else  if (punchState == PunchState.Punching)
	    {
	        if (Vector3.Distance(playerTransform.position, transform.position) > maxPunchLength)
	        {
	            punchState = PunchState.Retracting;
	        }
	    }
	    else if (punchState == PunchState.Retracting)
	    {
            if (Vector3.Distance(playerTransform.position, transform.position) < minRetractDistance)
            {
				Reset();
            }

	        Vector2 retractAngle = (playerTransform.position - transform.position).normalized;
            velocity = retractAngle * retractSpeed;
            if (impactForce.magnitude > .1f)
            {
                velocity += impactForce * punchSpeed;
                impactForce *= .95f;
            }
	    }

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
            player.rigidbody2D.velocity += pushAngle.normalized * pushTerrainSpeed * Mathf.Min(maxCharge, chargeLevel);
            chargeLevel = 1;
        }
        else if (collider.gameObject.tag == "Player" && punchState == PunchState.Punching)
        {
			punchState = PunchState.Retracting;
            collider.rigidbody2D.velocity += velocity.normalized * pushPlayerSpeed;
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
        punchState = PunchState.Punching;
        if (playerInput.aimAngle.magnitude > 0)
        {
            velocity = playerInput.aimAngle * punchSpeed;
        }
        //Vector2 normalized = playerInput.aimAngle;
        //if (normalized.magnitude > 0) {
            //punchState = PunchState.Punching;
            //velocity = playerInput.aimAngle * punchSpeed;
        //}
    }

	private void Reset() {
		punchState = PunchState.Ready;
		transform.position = playerTransform.position + spriteOffset;
		velocity = Vector2.zero;
	}

}
