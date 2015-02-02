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

	public float punchSpeed;
    public float retractSpeed;
    public float pushTerrainSpeed;
    public float pushPlayerSpeed;
    public Vector3 spriteOffset;
    public float maxPunchLength;
    public float minRetractDistance;
	public PunchState punchState;
	public float maxCharge;
	public float chargeSpeed;

	private PlayerInput playerInput;
    private Transform playerTransform;
    private Vector2 impactForce;
	private Vector2 velocity;
    private GameObject player;
    public GameObject gun;
	private float chargeLevel;
    
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
            gun.transform.eulerAngles = newRot;
			
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
				if (chargeLevel > maxCharge)
				{
					chargeLevel = maxCharge;
				}
			}
			else if (playerInput.inputFireUp)
			{
                Vector3 newRot = Vector3.zero;
                newRot.z = Vector2.Angle(Vector2.right, playerInput.aimAngle);
                if (playerInput.aimAngle.y < 0)
                {
                    newRot.z = 360 - newRot.z;
                }
                transform.localEulerAngles = newRot;
                gun.transform.eulerAngles = newRot;

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
            player.rigidbody2D.velocity += pushAngle.normalized * pushTerrainSpeed * chargeLevel;
            chargeLevel = 1;
        }
        else if (collider.gameObject.tag == "Player" && punchState == PunchState.Punching)
        {
			punchState = PunchState.Retracting;
            collider.rigidbody2D.velocity += velocity.normalized * pushPlayerSpeed * ( chargeLevel + 1 )/ 2;
			player.rigidbody2D.velocity += velocity.normalized * pushPlayerSpeed/2;

			Animator animator = collider.gameObject.GetComponent<Animator>();
			animator.Play("hit");
        }
        else if (collider.gameObject.tag == "Puncher" && punchState == PunchState.Punching)
        {
            punchState = PunchState.Retracting;
            player.rigidbody2D.velocity += velocity.normalized * pushPlayerSpeed / 2;
            impactForce = collider.gameObject.GetComponent<PunchController>().velocity.normalized;
        }
		else {
			Reset();
		}
    }

    public void Fire()
    {
        if (playerInput.aimAngle.magnitude > 0)
        {
            punchState = PunchState.Punching;
            velocity = playerInput.aimAngle * punchSpeed;

            //Animator animator = player.GetComponent<Animator>();
            //animator.Play("shooting");
        }
    }

	private void Reset() {
		punchState = PunchState.Ready;
		transform.position = playerTransform.position + spriteOffset;
		velocity = Vector2.zero;
        transform.localEulerAngles = Vector3.zero;

		Extender extender = GetComponentInChildren<Extender> ();
		extender.Reset ();
	}

}
