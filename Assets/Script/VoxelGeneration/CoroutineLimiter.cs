using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineLimiter : MonoBehaviour {

    public float calculeTime = 0.01f;

    private float framTime;

	// Use this for initialization
	void Start () {
        framTime = 0;
	}

    private void FixedUpdate()
    {
        framTime = Time.realtimeSinceStartup;
    }

    public bool calculeAutorisation()
    {
        if (Time.realtimeSinceStartup > framTime + calculeTime)
            return false;
        return true;
    }
}
