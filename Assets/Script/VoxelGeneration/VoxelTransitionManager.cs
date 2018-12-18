using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelTransitionManager{

    public static int maxVoxel = 2000;

    private static int voxelInTransit = 0;

    //set and get
    public static int GetMaxVoxel() { return maxVoxel; }
    public static int GetVoxelInTransit() { return voxelInTransit; }

    public static bool voxelCanTransit()
    {
        return (voxelInTransit < maxVoxel ? true : false);
    }

    public static void addTransitVoxel()
    {
        ++voxelInTransit;
    }

    public static void delTransitVoxel()
    {
        --voxelInTransit;
    }
}
