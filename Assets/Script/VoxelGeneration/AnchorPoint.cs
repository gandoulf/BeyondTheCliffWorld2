using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPoint : MonoBehaviour {

    [SerializeField] private Voxel anchoredVoxel;
    public Voxel AnchoredVoxel { get { return anchoredVoxel; }}

    private VoxelGenerationBlock voxelGenerator = null;
    private int iterator = -1;

    public void Initialisation(VoxelGenerationBlock VGB = null, int iterator = -1)
    {
        voxelGenerator = VGB;
        this.iterator = iterator;
    }

    public void Anchoring(Voxel voxel)
    {
        anchoredVoxel = voxel;
        if (voxelGenerator != null)
            voxelGenerator.AttacheVertex(iterator);
    }

    public void Disanchoring(GameObject destination = null, GameObject origin = null)
    {
        anchoredVoxel.StartTransition(destination, voxelGenerator.gameObject, origin, 1.0f);
        if (voxelGenerator.fading)
            anchoredVoxel.startFading(false, 2.0f, voxelGenerator);
        anchoredVoxel = null;
    }

    public void DeleteVoxel()
    {
        anchoredVoxel.DeleteVoxel();
        anchoredVoxel = null;
    }
}
