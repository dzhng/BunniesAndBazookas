using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public int playerId;
    PlayerInput playerInput;
    public GameController gameController;

	// Use this for initialization
	void Start () {
        playerInput = GetComponent<PlayerInput>();
	}
	
	// Update is called once per frame
	void Update () {
        if (playerInput.restartPressed)
        {
            gameController.RestartGame();
        }
	}
}
