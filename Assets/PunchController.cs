﻿using System;
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
    public float punchSpeed = 20.0f;
    public float retractSpeed = 20.0f;
    public float pushTerrainSpeed = 20.0f;
    public float pushPlayerSpeed = 20.0f;
    public Vector3 spriteOffset;
    public float maxPunchLength = 20f;
    public float minRetractDistance = 4f;
    private Transform playerTransform;
    private PlayerInput playerInput;
    private Vector2 impactForce;

    private Vector2 velocity;
    private GameObject player;
    private bool isCharging;
    private float chargeLevel = 0;
    private float maxCharge = 20f;
    public float chargeSpeed;
    
	// Use this for initialization
	void Start ()
	{
        impactForce = Vector2.zero;
        player = transform.parent.gameObject;
	    punchState = PunchState.Ready;
	    playerTransform = player.transform;
        playerInput = player.GetComponent<PlayerInput>();
	    transform.position = playerTransform.position + spriteOffset;
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
                HandleFire();
	        }
            else
            {
                transform.position = playerTransform.position + spriteOffset;
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
                punchState = PunchState.Ready;
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
            punchState = PunchState.Ready;
            velocity = Vector2.zero;
        }
        else if (collider.gameObject.tag == "Terrain" && punchState == PunchState.Punching)
        {
            punchState = PunchState.Retracting;
            velocity = Vector2.zero;
            Vector2 pushAngle = player.transform.position - transform.position;
            player.rigidbody2D.velocity += pushAngle.normalized * pushTerrainSpeed;
        }
        else if (collider.gameObject.tag == "Player" && punchState == PunchState.Punching)
        {
            collider.rigidbody2D.velocity += velocity.normalized * pushPlayerSpeed;
			player.rigidbody2D.velocity += -velocity.normalized * pushPlayerSpeed/2;
        }
        else if (collider.gameObject.tag == "Puncher" && punchState == PunchState.Punching)
        {
            punchState = PunchState.Retracting;
            impactForce = collider.gameObject.GetComponent<PunchController>().velocity.normalized;
        }
    }

    private void HandleFire() {
        if (!isCharging) {
            isCharging = true;
            StartCoroutine("CalculateCharge");
        }
    }

    IEnumerator CalculateCharge() {
        while (playerInput.inputFire)
        {
            chargeLevel += Time.deltaTime * chargeSpeed;
            yield return null;
        }
        Debug.Log("done charging");
        Fire();
    }
    private void Fire()
    {
        punchState = PunchState.Punching;
        velocity = playerInput.aimAngle * Mathf.Min(chargeLevel, maxCharge);
        chargeLevel = 0f;
        isCharging = false;
    }

}
