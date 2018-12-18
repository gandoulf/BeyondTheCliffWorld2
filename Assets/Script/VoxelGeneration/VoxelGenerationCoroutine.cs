using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGenerationCoroutine : MonoBehaviour {

    private enum Axes
    {
        x = 0,
        y = 1,
        z = 2,
        none = 4
    };

    private class VoxelPoint : MonoBehaviour
    {
        public void init(VoxelGenerationCoroutine VGC, Vector3 pos)
        {
            _VGC = VGC;
            this.pos = pos;
            StartCoroutine("instantiate");
        }

        private VoxelGenerationCoroutine _VGC;
        public Vector3 pos;

        private IEnumerator instantiate()
        {
            yield return new WaitForSeconds(0.1f);
            _VGC.recursiveInstantiation(pos);
        }

    }

    [SerializeField] float blockSize;
    [SerializeField] Axes axeOriginReference;
    [SerializeField] Axes stuckAxe;
    [SerializeField] GameObject rayCast;

    private BoxCollider BC;
    private Transform parent;

    private Vector3 localCenterPoint;
    private Vector3 meshSquareSize;
    private Vector3 raycastSquareSize;
    private Vector3 origin;

    private List<VoxelPoint> pointList;

    // Use this for initialization
    void Start () {
        BC = gameObject.GetComponent<BoxCollider>();
        parent = gameObject.transform.parent;

        localCenterPoint = gameObject.transform.localPosition + BC.center;
        meshSquareSize = BC.size;
        raycastSquareSize = new Vector3(BC.size.x * 1.5f, BC.size.y * 1.5f, BC.size.z * 1.5f);
        origin = new Vector3(localCenterPoint.x - (axeOriginReference == Axes.x ? meshSquareSize.x / 2 - blockSize / 2 : stuckAxe == Axes.x ? -(meshSquareSize.x / 2 - blockSize / 2) : 0),
                             localCenterPoint.y - (axeOriginReference == Axes.y ? meshSquareSize.y / 2 - blockSize / 2 : stuckAxe == Axes.y ? -(meshSquareSize.y / 2 - blockSize / 2) : 0),
                             localCenterPoint.z - (axeOriginReference == Axes.z ? meshSquareSize.z / 2 - blockSize / 2 : stuckAxe == Axes.z ? -(meshSquareSize.z / 2 - blockSize / 2) : 0));
        pointList = new List<VoxelPoint>();
        BC.enabled = false;
        pointInstantiation(origin);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void recursiveInstantiation(Vector3 pos)
    {
        if (!pointList.Exists(X => X.pos == new Vector3(pos.x + blockSize, pos.y, pos.z)))
        {
            pointInstantiation(new Vector3(pos.x + blockSize, pos.y, pos.z));
        }
        if (!pointList.Exists(X => X.pos == new Vector3(pos.x - blockSize, pos.y, pos.z)))
        {
            pointInstantiation(new Vector3(pos.x - blockSize, pos.y, pos.z));
        }
        if (!pointList.Exists(X => X.pos == new Vector3(pos.x, pos.y + blockSize, pos.z)))
        {
            pointInstantiation(new Vector3(pos.x, pos.y + blockSize, pos.z));
        }
        if (!pointList.Exists(X => X.pos == new Vector3(pos.x, pos.y - blockSize, pos.z)))
        {
            pointInstantiation(new Vector3(pos.x, pos.y - blockSize, pos.z));
        }
        if (!pointList.Exists(X => X.pos == new Vector3(pos.x, pos.y, pos.z + blockSize)))
        {
            pointInstantiation(new Vector3(pos.x, pos.y, pos.z + blockSize));
        }
        if (!pointList.Exists(X => X.pos == new Vector3(pos.x, pos.y, pos.z - blockSize)))
        {
            pointInstantiation(new Vector3(pos.x, pos.y, pos.z - blockSize));
        }
    }

    private void pointInstantiation(Vector3 pos)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.parent = parent;
        go.transform.localPosition = pos;
        go.GetComponent<BoxCollider>().enabled = false;
        if (pointValidation(go))
        {
            go.transform.localScale = new Vector3(blockSize, blockSize, blockSize);
            go.transform.localPosition = pos;
            go.transform.localRotation = Quaternion.identity;
            go.GetComponent<BoxCollider>().enabled = false;
            go.tag = "cube";
            go.AddComponent<VoxelPoint>();
            go.GetComponent<VoxelPoint>().init(this, go.transform.localPosition);
            pointList.Add(go.GetComponent<VoxelPoint>());
        }
    }



    private bool pointValidation(GameObject point)
    {
        Vector3[] hitPoint = new Vector3[6];
        RaycastHit hit;

        if (point == null)
            return false;
        //raycast y axes
        rayCast.transform.localPosition = new Vector3(point.transform.localPosition.x, localCenterPoint.y + (raycastSquareSize.y / 2), point.transform.localPosition.z);
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000);
        //Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.green);
        hitPoint[0] = hit.point;
        if (hit.collider == null)
        {
            Destroy(point);
            return false;
        }

        //raycast x axes
        rayCast.transform.localPosition = new Vector3(localCenterPoint.x + (raycastSquareSize.x / 2), point.transform.localPosition.y, point.transform.localPosition.z);
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000);
        //Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.green);
        hitPoint[2] = hit.point;
        if (hit.collider == null)
        {
            //print(hit.collider.gameObject.tag);
            Destroy(point);
            return false;
        }

        //raycast z axes
        rayCast.transform.localPosition = new Vector3(point.transform.localPosition.x, point.transform.localPosition.y, localCenterPoint.z + (raycastSquareSize.z / 2));
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000);
        //Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.green);
        hitPoint[4] = hit.point;
        if (hit.collider == null)
        {
            //print(hit.collider.gameObject.tag);
            Destroy(point);
            return false;
        }

        return true;
    }
}
