using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {
	
	public enum PunchState
	{
		Punching,
		Retracting,
		Ready,
		Charging
	}

	private PunchState punchState;
	private PlayerInput playerInput;
	private GameObject player;

	// Use this for initialization
	void Start () {
		player = transform.parent.gameObject;
		playerInput = player.GetComponent<PlayerInput>();
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
				if (chargeLevel > maxCharge)
				{
					chargeLevel = maxCharge;
				}
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
	}
}
