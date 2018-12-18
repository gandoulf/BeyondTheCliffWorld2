using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSizeDetector : MonoBehaviour {

    enum Axes
    {
        x = 0,
        y = 1,
        z = 2,
        none = 4
    };

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

    private List<GameObject> pointList;

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
        pointList = new List<GameObject>();

        pointInstantiation(origin);

        /*print("localCenterPoint: " + localCenterPoint);
        print("meshSquareSize: " + meshSquareSize);
        print("ocalCenterPoint - (meshSquareSize / 2): " + (localCenterPoint - (meshSquareSize / 2)));
        print("Origin: " + origin);*/
        BC.enabled = false;
        gameObject.GetComponent<MeshCollider>().enabled = true;
        int pointIterator = 0;
        while (pointIterator < pointList.Count)
            if (pointValidation(pointList[pointIterator], pointIterator))
                ++pointIterator;
        //UnityEditor.EditorApplication.isPaused = true;
    }
	
	// Update is called once per frame
	void Update () {
   
	}

    private void recursiveInstantiation(Vector3 pos)
    {
        if (!pointList.Exists(X => X.transform.localPosition == new Vector3(pos.x + blockSize, pos.y, pos.z)))
            pointInstantiation(new Vector3(pos.x + blockSize, pos.y, pos.z));
        if (!pointList.Exists(X => X.transform.localPosition == new Vector3(pos.x - blockSize, pos.y, pos.z)))
            pointInstantiation(new Vector3(pos.x - blockSize, pos.y, pos.z));
        if (!pointList.Exists(X => X.transform.localPosition == new Vector3(pos.x, pos.y + blockSize, pos.z)))
            pointInstantiation(new Vector3(pos.x, pos.y + blockSize, pos.z));
        if (!pointList.Exists(X => X.transform.localPosition == new Vector3(pos.x, pos.y - blockSize, pos.z)))
            pointInstantiation(new Vector3(pos.x, pos.y - blockSize, pos.z));
        if (!pointList.Exists(X => X.transform.localPosition == new Vector3(pos.x, pos.y, pos.z + blockSize)))
            pointInstantiation(new Vector3(pos.x, pos.y, pos.z + blockSize));
        if (!pointList.Exists(X => X.transform.localPosition == new Vector3(pos.x, pos.y, pos.z - blockSize)))
            pointInstantiation(new Vector3(pos.x, pos.y, pos.z - blockSize));
    }

    private void pointInstantiation(Vector3 pos)
    {
        if (pos.x > localCenterPoint.x + (meshSquareSize.x / 2) || pos.x < localCenterPoint.x - (meshSquareSize.x / 2) ||
            pos.y > localCenterPoint.y + (meshSquareSize.y / 2) || pos.y < localCenterPoint.y - (meshSquareSize.y / 2) ||
            pos.z > localCenterPoint.z + (meshSquareSize.z / 2) || pos.z < localCenterPoint.z - (meshSquareSize.z / 2))
            return;
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.parent = parent;
        go.transform.localScale = new Vector3(blockSize, blockSize, blockSize);
        go.transform.localPosition = pos;
        go.transform.localRotation = Quaternion.identity;
        go.GetComponent<BoxCollider>().enabled = false;
        go.tag = "cube";
        pointList.Add(go);
        recursiveInstantiation(pos);
    }

    private bool pointValidation(GameObject point, int pointIterator)
    {
        Vector3[] hitPoint = new Vector3[6];
        RaycastHit hit;

        if (point == null)
            return false;

        //print("point: " + point.transform.position);

        //raycast y axes
        rayCast.transform.localPosition = new Vector3(point.transform.localPosition.x, localCenterPoint.y + (raycastSquareSize.y / 2), point.transform.localPosition.z);
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000);
        //Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.green);
        hitPoint[0] = hit.point;
        if (hit.collider == null)
        {
            //print(hit.collider.gameObject.tag);
            Destroy(point);
            pointList.RemoveAt(pointIterator);
            return false;
        }

        /*    rayCast.transform.localPosition = new Vector3(point.transform.localPosition.x, localCenterPoint.y - (raycastSquareSize.y / 2), point.transform.localPosition.z);
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000);
        //Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.green);
        hitPoint[1] = hit.point;
        if (hit.collider == null)
        {
            //print(hit.collider.gameObject.tag);
            Destroy(point);
            pointList.RemoveAt(pointIterator);
            return false;
        }*/

        //raycast x axes
        rayCast.transform.localPosition = new Vector3(localCenterPoint.x + (raycastSquareSize.x / 2), point.transform.localPosition.y, point.transform.localPosition.z);
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000);
        //Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.green);
        hitPoint[2] = hit.point;
        if (hit.collider == null)
        {
            //print(hit.collider.gameObject.tag);
            Destroy(point);
            pointList.RemoveAt(pointIterator);
            return false;
        }

        /*rayCast.transform.localPosition = new Vector3(localCenterPoint.x - (raycastSquareSize.x / 2), point.transform.localPosition.y, point.transform.localPosition.z);
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000);
        //Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.green);
        hitPoint[3] = hit.point;
        if (hit.collider == null)
        {
            //print(hit.collider.gameObject.tag);
            Destroy(point);
            pointList.RemoveAt(pointIterator);
            return false;
        }*/

        //raycast z axes
        rayCast.transform.localPosition = new Vector3(point.transform.localPosition.x, point.transform.localPosition.y, localCenterPoint.z + (raycastSquareSize.z / 2));
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000);
        //Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.green);
        hitPoint[4] = hit.point;
        if (hit.collider == null)
        {
            //print(hit.collider.gameObject.tag);
            Destroy(point);
            pointList.RemoveAt(pointIterator);
            return false;
        }

        /*rayCast.transform.localPosition = new Vector3(point.transform.localPosition.x, point.transform.localPosition.y, localCenterPoint.z - (raycastSquareSize.z / 2));
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000);
        //Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.green);
        hitPoint[5] = hit.point;
        if (hit.collider == null)
        {
            //print(hit.collider.gameObject.tag);
            Destroy(point);
            pointList.RemoveAt(pointIterator);
            return false;
        }*

        //print(hitPoint[0] + " " + hitPoint[1] + " " + hitPoint[2] + " " + hitPoint[3] + " " + hitPoint[4] + " " + hitPoint[5]);
        //print("\n\n");

        /*if (point.transform.position.z > hitPoint[0].z || point.transform.position.z < hitPoint[1].z ||
            point.transform.position.x > hitPoint[2].x || point.transform.position.x < hitPoint[3].x ||
            point.transform.position.y > hitPoint[4].y || point.transform.position.y < hitPoint[5].y)
            print("ok");
        else
            Destroy(point);*/

        return true;
    }
}
