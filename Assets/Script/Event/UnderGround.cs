using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class UnderGround : MonoBehaviour {

    [SerializeField] float speed;
    [SerializeField] GameObject platform;
    [SerializeField] VoxelInstantiator VI;

    private GameObject platforme;
    private GameObject player;
    private bool recuperation;

    private GameObject oldPlat;

    // Use this for initialization
    void Start () {
        recuperation = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && recuperation == false)
        {
            player = other.gameObject;
            player.GetComponent<PlayerDollisionDetection>().controller.GetComponent<FirstPersonController>().setWalkSpeed(0);
            player.GetComponent<PlayerDollisionDetection>().controller.GetComponent<FirstPersonController>().setRunSpeed(0);
            recuperation = true;
            platforme = Instantiate(platform);
            platforme.transform.position = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y - other.gameObject.GetComponent<CapsuleCollider>().height , other.gameObject.transform.position.z);
            Invoke("fillPlatform", 0.2f);
        }
    }

    private void fillPlatform()
    {
        VI.StartInstantiateBlock(platforme.GetComponentInChildren<VoxelGenerationBlock>());
        StartCoroutine(goToPlatform());
    }

    private IEnumerator goToPlatform()
    {
        VoxelGenerationBlock VGB = platforme.GetComponentInChildren<VoxelGenerationBlock>();
        while (platforme.transform.position.y < 2)
        {
            if (VGB.isFullVertex())
            {
                float y = speed * Time.deltaTime;
                Vector3 move = new Vector3(0, y, 0);
                platforme.transform.position += move;
                player.transform.position += move;
                player.GetComponent<PlayerDollisionDetection>().controller.transform.position += move;
            }
            yield return new WaitForFixedUpdate();
        }
        player.GetComponent<PlayerDollisionDetection>().controller.GetComponent<FirstPersonController>().setWalkSpeed(5);
        player.GetComponent<PlayerDollisionDetection>().controller.GetComponent<FirstPersonController>().setRunSpeed(10);
        oldPlat = platforme;
        Invoke("deleteVGB", 2.0f);
        recuperation = false;
        yield return 0;
    }

    private void deleteVGB()
    {
        oldPlat.GetComponentInChildren<VoxelGenerationBlock>().StartMeshDislocking();
        Destroy(oldPlat, 2.0f);
    }
}
