using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orientationtest : MonoBehaviour {
    [SerializeField] private GameObject origin;
    [SerializeField] private GameObject destinationOrigin;
    [SerializeField] private GameObject destination;
    [SerializeField] private GameObject curveDirection;
    // Use this for initialization
    void Start () {
        GameObject originA = origin.GetComponent<VoxelGenerationBlock>() == null ? origin : origin.GetComponent<VoxelGenerationBlock>().GetCenter();
        GameObject originB = destinationOrigin.GetComponent<VoxelGenerationBlock>() == null ? destinationOrigin : destinationOrigin.GetComponent<VoxelGenerationBlock>().GetCenter();
        Vector3 MeshSplitSide = originB.transform.position - originA.transform.position;
        MeshSplitSide = new Vector3(MeshSplitSide.x, 0, MeshSplitSide.z);

        if (destination != null)
        {
            Vector3 dest = destination.transform.position - gameObject.transform.position;
            Vector3 up = new Vector3(0, 1, 0);
            Vector3 dest2D = new Vector3(dest.x, 0, dest.z);
            up.Normalize();
            dest.Normalize();
            Vector3 thirdAxes = Vector3.Cross(dest, up);
            thirdAxes.Normalize();

            Debug.DrawRay(gameObject.transform.position, dest, Color.blue);
            Debug.DrawRay(gameObject.transform.position, up, Color.green);
            Debug.DrawRay(gameObject.transform.position, thirdAxes, Color.red);

            if (dest2D.x < MeshSplitSide.x && dest2D.z < MeshSplitSide.z)
                dest = MeshSplitSide.x > 0 ? dest -= thirdAxes : dest += thirdAxes;
            else
                dest = MeshSplitSide.x > 0 ? dest += thirdAxes : dest -= thirdAxes;

            //dest.Normalize();
            //dest += new Vector3(0, -0.8f, 0);
            //dest.Normalize();
            curveDirection.transform.localPosition = new Vector3(dest.x, dest.y, dest.z);
        }
        else
        {
            curveDirection.transform.localPosition = new Vector3(0, -Random.Range(0, 2.0f), 0);
        }
        //curveDirection.transform.localPosition = new Vector3 (Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f)); //fun
    }

    // Update is called once per frame
    void Update () {
		
	}
}
