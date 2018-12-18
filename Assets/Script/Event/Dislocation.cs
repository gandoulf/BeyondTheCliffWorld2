using UnityEngine;
using System.Collections;

public class Dislocation : MonoBehaviour {

    public bool disloc;
    [SerializeField] private VoxelGenerationBlock VGB;

    private bool dislocated;

    [SerializeField] StorySpeakerEvent Speaker;
    [SerializeField] float timeBeforeSpeakerEnabling;

	// Use this for initialization
	void Start () {
        dislocated = false;
        disloc = false;
	}

    private void Update()
    {
        if (disloc == true)
        {
            VGB.StartMeshDislocking();
            disloc = false;
        }
    }

    /*void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.tag);
        if (collision.gameObject.tag == "Player" && dislocated == false) {
            VGB.StartMeshDislocking();
            dislocated = true;
        }
    }*/

    public void dislockMesh()
    {
        if (dislocated == false)
        {
            VGB.StartMeshDislocking();
            dislocated = true;
            Invoke("EnableSpeaker", timeBeforeSpeakerEnabling);
        }
    }

    private void EnableSpeaker()
    {
        if (Speaker)
            Speaker.gameObject.SetActive(true);
    }
}
