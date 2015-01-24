using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

    public bool isFacingRight;
    public Vector2 aimAngle;
    public bool inputFire;

    PlayerController controller;

    private string HOR_INPUT = "Horizontal_Joy_";
    private string VER_INPUT = "Vertical_Joy_";
    private string FIRE_1_INPUT = "Fire1_Joy_";

    // Use this for initialization
	void Start () {
        controller = GetComponent<PlayerController>();
        int pid = controller.playerId;
        HOR_INPUT += pid;
        VER_INPUT += pid;
        FIRE_1_INPUT += pid;

        aimAngle = Vector2.zero;
    }
	
	// Update is called once per frame
	void Update () {
        HandleInput();
	}

    void HandleInput()
    {
        inputFire = Input.GetButton(FIRE_1_INPUT);
        float horizontal = Input.GetAxis(HOR_INPUT);
        float vertical = Input.GetAxis(VER_INPUT);
        aimAngle = new Vector2(horizontal, vertical).normalized;
		isFacingRight = (horizontal > 0);
    }
}
