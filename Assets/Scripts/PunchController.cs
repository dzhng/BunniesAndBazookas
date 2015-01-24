using System;
using UnityEngine;
using System.Collections;

public class PunchController : MonoBehaviour
{

    public enum PunchState
    {
        Punching,
        Retracting,
        Ready
    }

    public PunchState punchState;
    public float punchSpeed = 80.0f;
    public float retractSpeed = 80.0f;
    public float pushTerrainSpeed = 20.0f;
    public Vector3 spriteOffset;
    public float maxPunchLength = 20f;
	public float puncherStrength = 2f;

    private Transform playerTransform;
    private PlayerInput playerInput;

    private Vector2 velocity;
	private GameObject playerObject;
	private PlayerControl playerCtrl;
    
	// Use this for initialization
	void Start ()
	{
		playerObject = transform.parent.gameObject;
		playerCtrl = transform.parent.GetComponent<PlayerControl>();
		playerInput = playerObject.GetComponent<PlayerInput>();

	    punchState = PunchState.Ready;
	    playerTransform = playerObject.transform;
	    transform.position = playerTransform.position + spriteOffset;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	    if (punchState == PunchState.Ready)
	    {
            Vector3 newRot = Vector3.zero;
            newRot.z = Vector2.Angle(Vector2.right, playerInput.aimAngle);
            if (playerInput.aimAngle.y < 0) {
                newRot.z = 360 - newRot.z;
            }
            transform.localEulerAngles = newRot;
            bool inputFire = Input.GetButtonDown("Fire"+playerCtrl.playerId);
	        if (inputFire) {
                Fire();
	        }
            else {
                transform.position = playerTransform.position + spriteOffset;
            }
	    }

	    if (punchState == PunchState.Punching)
	    {
	        if (Vector3.Distance(playerTransform.position, transform.position) > maxPunchLength)
	        {
	            punchState = PunchState.Retracting;
	        }
	    }

	    if (punchState == PunchState.Retracting)
	    {
	        Vector2 retractAngle = (playerTransform.position - transform.position).normalized;
            velocity = retractAngle * retractSpeed;
	    }
        Vector3 movement = velocity * Time.deltaTime;
        transform.position += movement;

	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.Equals(playerObject) && punchState == PunchState.Retracting)
        {
            punchState = PunchState.Ready;
            velocity = Vector2.zero;
        }
        else if (collider.gameObject.tag == "ground")
        {
            punchState = PunchState.Retracting;
            velocity = Vector2.zero;
            Vector2 pushAngle = playerTransform.position - transform.position;
            playerObject.rigidbody2D.velocity += pushAngle.normalized * pushTerrainSpeed;
        }
		// if it collides with a player, knock the player back
		else if (collider.gameObject.tag == "Player") {
			Rigidbody2D hitPlayer = collider.rigidbody2D;
			Vector2 rocketVelocity = rigidbody2D.velocity;
			hitPlayer.velocity = puncherStrength * (rocketVelocity + hitPlayer.velocity);
			playerCtrl.hitTime = Time.time;
		}
    }


    private void Fire()
    {
        punchState = PunchState.Punching;
        velocity = playerInput.aimAngle * punchSpeed;
    }


}
