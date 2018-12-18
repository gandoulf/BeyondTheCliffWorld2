using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDollisionDetection : MonoBehaviour
{

    public GameObject controller;
    public PlayerSpeaker speaker;

    // Use this for initialization
    void Start()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "SoundEvent")
        {
            AudioClip tmp;
            tmp = other.GetComponent<ISoundEvent>().IsSomethingToPlay(controller);
            speaker.giveSentance(tmp);
        }
    }
}