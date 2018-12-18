using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour {

    public GameObject curveDirection;

    [SerializeField] Vector3 curveMultiplier;
    public bool randomCurveMultiplier;
    [SerializeField] Vector3 curveMultiplierMin;
    [SerializeField] Vector3 curveMultiplierMax;

    [SerializeField] float transitionTime;
    [SerializeField] AnimationCurve curve;

    public bool randomTransitionTime;
    [SerializeField] float transitionTimeMin;
    [SerializeField] float transitionTimeMax;

    private GameObject origin { get; set; }
    private GameObject destinationOrigin { get; set; }
    private GameObject destination { get; set; }
    //[SerializeField] private GameObject destinationCheck;

    private float transition;
    private Vector3 initialPosition;
    private Vector3 initialScale;

    private Coroutine currentCoroutine;

    //color for fading
    private Color meshColor;
    private Color meshSColor;
    private Color voxelColor;
    private Color voxelSColor;

    //set and get
    public Vector3 GetCurveMultiplier() { return curveMultiplier; }
    public void SetCurveMultiplier(Vector3 newCurveMultiplier) { curveMultiplier = newCurveMultiplier; }

    public float GetTransitionTime() { return transitionTime; }
    public void SetTransitionTime(float newTransitionTime) { transitionTime = newTransitionTime; }
    public void SetTransitionTimeMin(float newTransitionTime) { transitionTimeMin = newTransitionTime; }
    public void SetTransitionTimeMax(float newTransitionTime) { transitionTimeMax = newTransitionTime; }

    public void SetCurve(AnimationCurve newCurve) { curve = newCurve; }

    public float GetTransition() { return transition; }

    void Start () {
        transition = 0;
        initialPosition = gameObject.transform.position;
        initialScale = gameObject.transform.localScale;
        currentCoroutine = null;
    }

    #region materialFade

    public void startFading(bool on, float speed, VoxelGenerationBlock mesh)
    {
        voxelColor = gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color");
        voxelSColor = gameObject.GetComponent<MeshRenderer>().material.GetColor("_SColor");
        meshColor = mesh.gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color");
        meshSColor = mesh.gameObject.GetComponent<MeshRenderer>().material.GetColor("_SColor");
        StartCoroutine(fadeMaterial(on, speed));
    }

    private IEnumerator fadeMaterial(bool on, float speed)
    {
        float time = 0;

        gameObject.GetComponent<MeshRenderer>().enabled = true;
        if (on)
        {
            while (time < speed / 2)
            {
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(Mathf.Lerp(voxelColor.r, meshColor.r, time), Mathf.Lerp(voxelColor.g, meshColor.g, time),
                                                                                              Mathf.Lerp(voxelColor.b, meshColor.b, time), Mathf.Lerp(voxelColor.a, meshColor.a, time)));
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_SColor", new Color(Mathf.Lerp(voxelSColor.r, meshSColor.r, time), Mathf.Lerp(voxelSColor.g, meshSColor.g, time),
                                                                                               Mathf.Lerp(voxelSColor.b, meshSColor.b, time), Mathf.Lerp(voxelSColor.a, meshSColor.a, time)));

                time += Time.deltaTime / speed;
                yield return new WaitForFixedUpdate();
            }

            time = 0;
            while (time < speed / 2)
            {
                float falpha = Mathf.Lerp(meshColor.a, 0, time);
                float salpha = Mathf.Lerp(meshSColor.a, 0, time);
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(meshColor.r, meshColor.g, meshColor.b, falpha));
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_SColor", new Color(meshSColor.r, meshSColor.g, meshSColor.b, salpha));

                time += Time.deltaTime / speed;
                yield return new WaitForFixedUpdate();
            }
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", voxelColor);
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_SColor", voxelSColor);
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(meshColor.r, meshColor.g, meshColor.b, 0));
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_SColor", new Color(meshSColor.r, meshSColor.g, meshSColor.b, 0));

            while (time < speed / 2)
            {
                float falpha = Mathf.Lerp(0, voxelColor.a, time);
                float salpha = Mathf.Lerp(0, voxelSColor.a, time);
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(meshColor.r, meshColor.g, meshColor.b, falpha));
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_SColor", new Color(meshSColor.r, meshSColor.g, meshSColor.b, salpha));

                time += Time.deltaTime / speed;
                yield return new WaitForFixedUpdate();
            }

            time = 0;
            while (time < speed / 2)
            {
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(Mathf.Lerp(meshColor.r, voxelColor.r, time), Mathf.Lerp(meshColor.g, voxelColor.g, time),
                                                                                              Mathf.Lerp(meshColor.b, voxelColor.b, time), Mathf.Lerp(meshColor.a, voxelColor.a, time)));
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_SColor", new Color(Mathf.Lerp(meshSColor.r, voxelSColor.r, time), Mathf.Lerp(meshSColor.g, voxelSColor.g, time),
                                                                                               Mathf.Lerp(meshSColor.b, voxelSColor.b, time), Mathf.Lerp(meshSColor.a, voxelSColor.a, time)));

                time += Time.deltaTime / speed;
                yield return new WaitForFixedUpdate();
            }
        }

        yield return 0;
    }

    #endregion materialFade

    public void StartTransition(GameObject newDestination = null, GameObject newDestinationOrigin = null, GameObject newOrigin = null, float time = 0)
    {
        Start();
        destinationOrigin = newDestinationOrigin;
        origin = newOrigin;
        if (newDestination != null)
        {
            destination = newDestination;
            //destinationCheck = destination;
        }
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        VoxelTransitionManager.addTransitVoxel();
        if (randomCurveMultiplier)
            curveMultiplier = new Vector3(Random.Range(curveMultiplierMin.x, curveMultiplierMax.x), Random.Range(curveMultiplierMin.y, curveMultiplierMax.y), Random.Range(curveMultiplierMin.z, curveMultiplierMax.z));
        if (randomTransitionTime)
            transitionTime = Random.Range(transitionTimeMin, transitionTimeMax);
        currentCoroutine = StartCoroutine(transite(time));
    }

    private IEnumerator transite(float time)
    {
        if (time > 0)
            yield return new WaitForSeconds(time);
        setVoxelOrientation(destinationOrigin); // update the director vector
        gameObject.GetComponent<BoxCollider>().enabled = false;
        while (destination != null && transition < 1)
        {
            transform.position = Vector3.Lerp(initialPosition, destination.transform.position, transition);
            transform.localScale = Vector3.Lerp(initialScale, destination.transform.localScale, transition);
            transform.position = new Vector3(initialPosition.x + (transform.position.x - initialPosition.x) + (curveDirection.transform.localPosition.x * (curve.Evaluate(transition) * curveMultiplier.x)),
                                             initialPosition.y + (transform.position.y - initialPosition.y) + (curveDirection.transform.localPosition.y * (curve.Evaluate(transition) * curveMultiplier.y)),
                                             initialPosition.z + (transform.position.z - initialPosition.z) + (curveDirection.transform.localPosition.z * (curve.Evaluate(transition) * curveMultiplier.z)));
            transition += Time.deltaTime / transitionTime;
            yield return new WaitForFixedUpdate();
        }
        transform.position = destination.transform.position;
        gameObject.GetComponent<BoxCollider>().enabled = true;
        if (destination.GetComponent<AnchorPoint>() != null)
            destination.GetComponent<AnchorPoint>().Anchoring(this);
        else if (destination.name == "DeadEnd")
            Destroy(this.gameObject);
        VoxelTransitionManager.delTransitVoxel();
        yield return 0;
    }

    private void setVoxelOrientation(GameObject destination)
    {
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

            if (dest2D.x < MeshSplitSide.x && dest2D.z < MeshSplitSide.z)
                dest = MeshSplitSide.x > 0 ? dest += thirdAxes : dest -= thirdAxes;
            else
                dest = MeshSplitSide.x > 0 ? dest -= thirdAxes : dest += thirdAxes;

            dest = new Vector3(dest.x, dest.y * -1, dest.z);
            if (dest.y > 0)
                dest += up;
            else
                dest -= up;
            dest.Normalize();

            //dest.Normalize();
            //dest += new Vector3(0, -0.8f, 0);
            //dest.Normalize();
            curveDirection.transform.localPosition = new Vector3(dest.x, dest.y, dest.z);
        } else {
            curveDirection.transform.localPosition = new Vector3(0, -Random.Range(0, 2.0f), 0);
        }
        //curveDirection.transform.localPosition = new Vector3 (Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f)); //fun
    } 

    public void DeleteVoxel()
    {
        Destroy(gameObject);
    }
}
