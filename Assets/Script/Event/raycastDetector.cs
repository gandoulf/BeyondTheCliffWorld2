using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycastDetector : MonoBehaviour {

    [SerializeField] GameObject rayOrigin;

	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward, Color.green);
        if (Input.GetMouseButtonDown(0))
            if (Physics.Raycast(rayOrigin.transform.position, transform.forward, out hit, 3.0f))
                if (hit.transform.gameObject.tag == "dislocatorSphere")
                    hit.transform.gameObject.GetComponent<Dislocation>().dislockMesh();

    }
}
