using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]

public class StorySpeakerEvent : MonoBehaviour , ISoundEvent {

    [SerializeField] bool delete;

    [SerializeField] AudioClip soundToPlay;
    [SerializeField] string subTittle;
    
    [SerializeField] GameObject cameraViewDirection;
    [SerializeField] float angleAutorisation;

    private bool played;
    private Vector3 watchingDirection;

	// Use this for initialization
	void Start () {
        played = false;
        if (cameraViewDirection != null)
            watchingDirection = cameraViewDirection.transform.position - gameObject.transform.position;
	}

    public AudioClip IsSomethingToPlay(GameObject controller)
    {
        AudioClip ret = null;
        if (played == false)
        {
            print(Vector3.Angle(watchingDirection.normalized, controller.transform.forward.normalized));
            if (cameraViewDirection != null && Vector3.Angle(watchingDirection.normalized, controller.transform.forward.normalized) > angleAutorisation)
                return ret;
            played = true;
            ret = soundToPlay;
        }
        return ret;
    }
}
