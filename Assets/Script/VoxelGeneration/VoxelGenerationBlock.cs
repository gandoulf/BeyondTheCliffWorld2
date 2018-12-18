using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshCollider))]

public class VoxelGenerationBlock : MonoBehaviour {

    public bool forPlayerSave;

#if UNITY_EDITOR
    public bool debug;
#endif

    private bool generated;

    public KeyCode key; //toDelete

    public bool instantiateVoxel = false; // create mesh voxel instantied
    public bool fading = false;
    public Voxel voxel; // voxel prefab
    public VoxelGenerationBlock voxelGenerator; // link to the mesh to build (needed by meshdislocking)
    

    [SerializeField] float blockSize; // voxel size
    [SerializeField] GameObject rayCast; // empty gaameObject for raycast
    [SerializeField] bool useOffsetPoint; // use an offset for the voxel grid
    [SerializeField] GameObject OffsetPoint; // the offset point (one of the voxel will have exactly it position
    [SerializeField] GameObject center;


    private BoxCollider BC;
    private Transform parent;

    private Vector3 localCenterPoint;
    private Vector3 meshSquareSize;
    private Vector3 raycastSquareSize;
    private Vector3 origin;
    private Vector3 originOffset;
    private Vector3 localScale;

    private GameObject AnchorList;
    private GameObject VoxelList;

    private List<GameObject> pointList;
    private int attachedVertex; // iterator giving the last anchorPoint with associated vertex (number of vertex anchored in pointlist)
    private int voxelConstructionEstimation; //estimation of the mesh construction (number of vertex in pointList)
    private int anchoredVoxelNBR;

    public bool useInstantiator = false;
    [SerializeField] VoxelInstantiator voxelInstantiator;
    [SerializeField] GameObject deadEnd;

    private CoroutineLimiter coroutineLimiter; // ref to the coroutine limiter

    //set and get
    public List<GameObject> GetPointList() { return pointList; }
    public GameObject GetDeadEnd() { return deadEnd; }
    public GameObject GetCenter() { return center; }

    // Use this for initialization
    void Start () {
        generated = false;
        coroutineLimiter = GameObject.Find("CoroutineLimiter").GetComponent<CoroutineLimiter>();

        BC = gameObject.GetComponent<BoxCollider>();
        parent = gameObject.transform.parent;

        localCenterPoint = gameObject.transform.localPosition + BC.center;
        meshSquareSize = BC.size;
        localScale = gameObject.transform.localScale;
        raycastSquareSize = new Vector3(BC.size.x * localScale.x * 1.5f, BC.size.y * localScale.y * 1.5f, BC.size.z * localScale.z * 1.5f);
        origin = new Vector3((localCenterPoint.x - (meshSquareSize.x / 2)) * localScale.x + (blockSize / 2),
                             (localCenterPoint.y - (meshSquareSize.y / 2)) * localScale.y + (blockSize / 2),
                             (localCenterPoint.z - (meshSquareSize.z / 2)) * localScale.z + (blockSize / 2));
        if (useOffsetPoint)
        {
            originOffset.x = (blockSize - ((OffsetPoint.transform.localPosition.x - origin.x) % blockSize));
            originOffset.y = (blockSize - ((OffsetPoint.transform.localPosition.y - origin.y) % blockSize));
            originOffset.z = (blockSize - ((OffsetPoint.transform.localPosition.z - origin.z) % blockSize));
            origin += originOffset;
        }
        pointList = new List<GameObject>();
        attachedVertex = 0;
        voxelConstructionEstimation = 0;
        BC.enabled = false;
        gameObject.GetComponent<MeshCollider>().enabled = true;

        VoxelList = GameObject.Find("VoxelList");
        if (VoxelList == null)
            VoxelList = new GameObject("VoxelList");
        AnchorList = new GameObject("AnchorList");
        AnchorList.transform.parent = parent;
        AnchorList.transform.position = parent.transform.position;
        AnchorList.transform.rotation = parent.transform.rotation;
        anchoredVoxelNBR = 0;
        if (deadEnd != null)
            deadEnd.name = "DeadEnd";
        if (instantiateVoxel && fading)
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        StartCoroutine("InstantiateBlock");
    }
	
	// toDelete
	void FixedUpdate () {
        if (Input.GetKeyDown(key))
            StartMeshDislocking();
    }

    #region pointListIterator

    public void setConstructionEstimation(int voxelNumber)
    {
        voxelConstructionEstimation = voxelNumber;
    }

    public int getListIterator()
    {
        if (voxelConstructionEstimation > 0)
            return voxelConstructionEstimation;
        return attachedVertex;
    }

    public void AttacheVertex(int iterator)
    {
        ++anchoredVoxelNBR;
        if (iterator > attachedVertex)
            attachedVertex = iterator;
        if (fading)
            if (anchoredVoxelNBR == pointList.Count && generated)
            {
                startFading(true, 2.0f);
            }
    }

    public bool isFullVertex()
    {
        if (anchoredVoxelNBR == pointList.Count)
            return true;
        return false;
    }

    #endregion pointListIterator

    #region materialFade

    private void startFading(bool on, float speed)
    {
        StartCoroutine(fadeMaterial(on, speed));
    }

    private IEnumerator fadeMaterial(bool on, float speed)
    {
        float time = 0;

        Color first = gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color");
        Color second = gameObject.GetComponent<MeshRenderer>().material.GetColor("_SColor");
        if (on)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(first.r, first.g, first.b, 0));
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_SColor", new Color(second.r, second.g, second.b, 0));
            for (int i = 0; i < pointList.Count;++i)
            {
                pointList[i].GetComponent<AnchorPoint>().AnchoredVoxel.startFading(true, 2.0f, this);
            }
        }

        while (time < speed)
        {
            float falpha = Mathf.Lerp(0, first.a, time);
            float salpha = Mathf.Lerp(0, second.a, time);
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(first.r, first.g, first.b, on ? falpha : first.a - falpha));
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_SColor", new Color(second.r, second.g, second.b, on ? salpha : second.a - salpha));
            time += Time.deltaTime / speed;
            yield return new WaitForFixedUpdate();
        }
        if (on)
            for (int i = 0; i < pointList.Count; ++i)
            {
                pointList[i].GetComponent<AnchorPoint>().AnchoredVoxel.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        else
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        yield return 0;
    }


    #endregion materialFade

    #region MeshDislocking

    public bool StartMeshDislocking(VoxelGenerationBlock VGB = null)
    {
        if (VGB != null)
            voxelGenerator = VGB;
        StartCoroutine(DislockMesh(voxelGenerator != null ? voxelGenerator.getListIterator(): 0));
        return true;
    }

    private IEnumerator DislockMesh(int iterator)
    {
        if (voxelGenerator != null)
        {
            List<GameObject> destinatationPointList = voxelGenerator.GetPointList();
            voxelGenerator.setConstructionEstimation(iterator + pointList.Count);
            if (useInstantiator)
            {
                voxelInstantiator.StartInstantiateBlock(voxelGenerator);
                voxelGenerator.setConstructionEstimation(destinatationPointList.Count);
            }
            for (int point = 0; point < pointList.Count; ++point)
            {
                while (!coroutineLimiter.calculeAutorisation())
                    yield return new WaitForFixedUpdate();
                if (pointList[point].GetComponent<AnchorPoint>().AnchoredVoxel != null)
                {
                    while (!VoxelTransitionManager.voxelCanTransit())
                        yield return new WaitForFixedUpdate();
                    if (point < destinatationPointList.Count - iterator)
                        pointList[point].GetComponent<AnchorPoint>().Disanchoring(destinatationPointList[point + iterator], gameObject);
                    else if (voxelGenerator.GetDeadEnd() != null)
                        pointList[point].GetComponent<AnchorPoint>().Disanchoring(voxelGenerator.GetDeadEnd(), gameObject);
                    else
                        pointList[point].GetComponent<AnchorPoint>().DeleteVoxel();
                    //yield return new WaitForFixedUpdate();
                }
            }
        }
        else
        {
            for (int point = 0; point < pointList.Count; ++point)
                pointList[point].GetComponent<AnchorPoint>().DeleteVoxel();
        }
        attachedVertex = 0;
        voxelConstructionEstimation = 0;
        anchoredVoxelNBR = 0;
        if (fading)
            startFading(false, 2.0f);
        yield return 0;
    }

    #endregion MeshDislocking

    #region VoxelMesh

    private IEnumerator InstantiateBlock()
    {
        yield return new WaitForFixedUpdate();
        gameObject.layer = LayerMask.NameToLayer("construction");
        Vector3 iteration = new Vector3(((meshSquareSize.x - originOffset.x) * localScale.x) / blockSize,
                                        ((meshSquareSize.y - originOffset.y) * localScale.y) / blockSize,
                                        ((meshSquareSize.z - originOffset.z) * localScale.z) / blockSize);
        for (int z = 0; z < Mathf.Floor(iteration.z); ++z)
        {
            for(int y = 0; y < Mathf.Floor(iteration.y); ++y)
            {
                for (int x = 0; x < Mathf.Floor(iteration.x); ++x)
                {
                    GameObject go = new GameObject("Anchor Point");
                    go.AddComponent<AnchorPoint>();
                    go.transform.parent = parent;
                    go.transform.localPosition = new Vector3(origin.x + blockSize * x, origin.y + blockSize * y, origin.z + blockSize * z);
                    go.transform.localScale = new Vector3(blockSize, blockSize, blockSize);
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.parent = AnchorList.transform;
                    if (pointValidation(go))
                    {
                        pointList.Add(go);
                        go.GetComponent<AnchorPoint>().Initialisation(this, pointList.Count - 1);
                        if (instantiateVoxel && voxel != null)
                        {
                            Voxel newVoxel = Instantiate(voxel);
                            newVoxel.transform.position = go.transform.position;
                            newVoxel.transform.localScale = go.transform.localScale;
                            newVoxel.transform.localRotation = Quaternion.identity;
                            newVoxel.transform.parent = VoxelList.transform;
                            if (fading)
                                newVoxel.GetComponent<MeshRenderer>().enabled = false;
                            go.GetComponent<AnchorPoint>().Anchoring(newVoxel);
                        }
                    } 
                }
                while (!coroutineLimiter.calculeAutorisation())
                    yield return new WaitForFixedUpdate();
            }
            //yield return new WaitForFixedUpdate();
        }
        if (!forPlayerSave)
            gameObject.GetComponent<MeshCollider>().enabled = false;
        generated = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        yield return 0;
    }

    private bool pointValidation(GameObject point)
    {
        var myLayerMask = LayerMask.GetMask("construction");
        Vector3[] hitPoint = new Vector3[6];
        RaycastHit hit;

        if (point == null)
            return false;
        //raycast y axes
        rayCast.transform.localPosition = new Vector3(point.transform.localPosition.x, localCenterPoint.y + ((raycastSquareSize.y / 2)), point.transform.localPosition.z);
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000, myLayerMask);
#if UNITY_EDITOR
        if (debug)
            Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.blue);
#endif
        hitPoint[0] = hit.point;
        rayCast.transform.localPosition = new Vector3(point.transform.localPosition.x, localCenterPoint.y - ((raycastSquareSize.y / 2)), point.transform.localPosition.z);
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000, myLayerMask);
#if UNITY_EDITOR
        if (debug)
            Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.blue);
#endif
        hitPoint[1] = hit.point;
        if (point.transform.position.y > hitPoint[0].y || point.transform.position.y < hitPoint[1].y)
        {
            Destroy(point);
            return false;
        }
        /*if (hit.collider == null)
        {
            Destroy(point);
            return false;
        }*/

        //raycast x axes
        rayCast.transform.localPosition = new Vector3(localCenterPoint.x + ((raycastSquareSize.x / 2)), point.transform.localPosition.y, point.transform.localPosition.z);
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000, myLayerMask);
#if UNITY_EDITOR
        if (debug)
            Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.red);
#endif
        hitPoint[2] = hit.point;
        rayCast.transform.localPosition = new Vector3(localCenterPoint.x - ((raycastSquareSize.x / 2)), point.transform.localPosition.y, point.transform.localPosition.z);
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000, myLayerMask);
#if UNITY_EDITOR
        if (debug)
            Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.red);
#endif
        hitPoint[3] = hit.point;
        if (point.transform.position.x > hitPoint[2].x || point.transform.position.x < hitPoint[3].x)
        {
            Destroy(point);
            return false;
        }
        /*if (hit.collider == null)
        {
            //print(hit.collider.gameObject.tag);
            Destroy(point);
            return false;
        }*/

        //raycast z axes
        rayCast.transform.localPosition = new Vector3(point.transform.localPosition.x, point.transform.localPosition.y, localCenterPoint.z + ((raycastSquareSize.z / 2)));
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000, myLayerMask);
#if UNITY_EDITOR
        if (debug)
            Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.green);
#endif
        hitPoint[4] = hit.point;
        rayCast.transform.localPosition = new Vector3(point.transform.localPosition.x, point.transform.localPosition.y, localCenterPoint.z - ((raycastSquareSize.z / 2)));
        Physics.Raycast(rayCast.transform.position, point.transform.position - rayCast.transform.position, out hit, 1000, myLayerMask);
#if UNITY_EDITOR
        if (debug)
            Debug.DrawRay(rayCast.transform.position, point.transform.position - rayCast.transform.position, Color.green);
#endif
        hitPoint[5] = hit.point;
        if (point.transform.position.z > hitPoint[4].z || point.transform.position.z < hitPoint[5].z)
        {
            Destroy(point);
            return false;
        }
        /*if (hit.collider == null)
        {
            //print(hit.collider.gameObject.tag);
            Destroy(point);
            return false;
        }*/

        return true;
    }

    #endregion VoxelMesh

}
