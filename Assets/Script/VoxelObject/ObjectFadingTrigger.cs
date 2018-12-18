using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ObjectFadingTrigger : MonoBehaviour {

    [SerializeField] ObjectFading[] objectToFade;

    // Use this for initialization
    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            print("playerEnter");
            for (int i = 0; i < objectToFade.Length; ++i)
                objectToFade[i].show();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            print("playerExit");
            for (int i = 0; i < objectToFade.Length; ++i)
                objectToFade[i].hide();
        }
    }
}
