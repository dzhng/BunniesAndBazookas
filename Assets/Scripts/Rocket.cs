using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour 
{
	public GameObject explosion;		// Prefab of explosion effect.
	public float rocketMass;

	void Start () {
		// Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
		Destroy(gameObject, 2);
	}

	void OnExplode() {
		// Create a quaternion with a random rotation in the z-axis.
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

		// Instantiate the explosion where the rocket is with the random rotation.
		Instantiate(explosion, transform.position, randomRotation);
	}
	
	void OnTriggerEnter2D (Collider2D col) {
		// Instantiate the explosion and destroy the rocket.
		OnExplode();
		Destroy (gameObject);

		// if it collides with a player, knock the player back
        if (col.gameObject.tag == "Player") {
			Rigidbody2D hitPlayer = col.rigidbody2D;
			Vector2 rocketVelocity = rigidbody2D.velocity;
			hitPlayer.velocity = rocketMass * (rocketVelocity + hitPlayer.velocity);

			PlayerControl player = col.gameObject.GetComponent<PlayerControl>();
			player.hitTime = Time.time;
        }
	}
}
