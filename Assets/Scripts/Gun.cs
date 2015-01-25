using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public float chargeSpeed;

	private PlayerInput playerInput;
	private GameObject player;
	private PunchController punchController;

	private float chargeLevel = 1;
	private float maxCharge = 5.0f;

	// Use this for initialization
	void Start () {
		player = transform.parent.gameObject;
		playerInput = player.GetComponent<PlayerInput>();
		punchController = transform.GetComponentInChildren<PunchController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (punchController.punchState == PunchController.PunchState.Ready)
		{
			Vector3 newRot = Vector3.zero;
			newRot.z = Vector2.Angle(Vector2.right, playerInput.aimAngle);
			if (playerInput.aimAngle.y < 0) {
				newRot.z = 360 - newRot.z;
			}
			transform.localEulerAngles = newRot;
			
			if (playerInput.inputFireDown) {
				punchController.punchState = PunchController.PunchState.Charging;
			}
		}
		else if (punchController.punchState == PunchController.PunchState.Charging) {
			if (playerInput.inputFireDown) {
				chargeLevel += Time.deltaTime * chargeSpeed;
				if (chargeLevel > maxCharge)
				{
					chargeLevel = maxCharge;
				}
			}
			else if (playerInput.inputFireUp)
			{
				punchController.Fire(playerInput.aimAngle, chargeLevel);
			}
		}
	}
}
