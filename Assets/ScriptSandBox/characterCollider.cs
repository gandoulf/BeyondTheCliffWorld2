using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterCollider : MonoBehaviour {

    public GameObject collider;

	// Use this for initialization
	void Start () {
		
	}

    private void LateUpdate()
    {
        collider.transform.position = gameObject.transform.position;
        collider.transform.rotation = gameObject.transform.rotation;
    }
}
