using UnityEngine;
using System.Collections;

public class Remover : MonoBehaviour
{
	public GameObject splash;
    public GameController gameController;	
	void OnTriggerEnter2D(Collider2D col)
	{
		// If the player hits the trigger...
		if(col.gameObject.tag == "Player")
		{
			// ... instantiate the splash where the player falls in.
			Instantiate(splash, col.transform.position, transform.rotation);
			// ... reload the level.
            gameController.EndGame(col.gameObject.GetComponent<PlayerController>().playerId);
            // ... destroy the player.
            Destroy(col.gameObject);

		}
	}

	IEnumerator ReloadGame()
	{			
		// ... pause briefly
		yield return new WaitForSeconds(2);
		// ... and then reload the level.

	}
}
