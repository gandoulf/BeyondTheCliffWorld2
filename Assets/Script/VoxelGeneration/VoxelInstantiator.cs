using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelInstantiator : MonoBehaviour {

    [SerializeField] bool fadding;

    [SerializeField] VoxelGenerationBlock voxelGenerator;
    [SerializeField] Voxel voxel;

    [SerializeField] bool changeVoxelSpeed;

    [SerializeField] float voxelSpeed;
    [SerializeField] float voxelSpeedMin;
    [SerializeField] float voxelSpeedMax;

    private CoroutineLimiter coroutineLimiter;

    // Use this for initialization
    void Start () {
        coroutineLimiter = GameObject.Find("CoroutineLimiter").GetComponent<CoroutineLimiter>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //StartInstantiateBlock();
        }
    }

    public bool StartInstantiateBlock(VoxelGenerationBlock VGB = null)
    {
        if (VGB != null)
            voxelGenerator = VGB;
        if (voxelGenerator == null)
            return false;
        StartCoroutine(InstantiateBlock(voxelGenerator.getListIterator()));
        return true;
    }

    private IEnumerator InstantiateBlock(int iterator)
    {
        List<GameObject> pointList = voxelGenerator.GetPointList();
        voxelGenerator.setConstructionEstimation(pointList.Count);
        for (int point = 0; point < pointList.Count - iterator; ++point)
        {
            while (!VoxelTransitionManager.voxelCanTransit() && !coroutineLimiter.calculeAutorisation())
                yield return new WaitForFixedUpdate();
            Voxel _voxel = Instantiate(voxel);
            if (changeVoxelSpeed)
            {
                voxel.SetTransitionTime(voxelSpeed);
                voxel.SetTransitionTimeMin(voxelSpeedMin);
                voxel.SetTransitionTimeMax(voxelSpeedMax);
            }
            voxel.gameObject.transform.position = gameObject.transform.position;
            _voxel.StartTransition(pointList[point + iterator], voxelGenerator.gameObject, gameObject);
            if (fadding)
                _voxel.startFading(false, 2.0f, voxelGenerator);
            //yield return new WaitForFixedUpdate();
        }
    }
}
