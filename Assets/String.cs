using UnityEngine;
using System.Collections;

public class String : MonoBehaviour {

	GameObject playerObject;
	float originalLength;

	// Use this for initialization
	void Start () {
		playerObject = transform.root.gameObject;
		originalLength = transform.renderer.bounds.size.x;
	}
	
	// Update is called once per frame
	void Update () {
        //Vector3 dir = playerObject.transform.position - transform.position;
        //dir = playerObject.transform.InverseTransformDirection(dir);
        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		float length = Vector3.Distance (playerObject.transform.position, transform.parent.position);
		transform.localScale = new Vector3 (length/originalLength, 1, 0);
		transform.localPosition = new Vector3 (-length/2, 0, 0); 
	}

    public void Reset()
    {
        //transform.localPosition = Vector3.zero;
    }
}
