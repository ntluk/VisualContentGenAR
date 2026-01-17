using System.Collections.Generic;
using UnityEngine;
using Meta.XR.BuildingBlocks;
using Meta.XR.MRUtilityKit;
using UnityEngine.Serialization;

public class RoomManager : MonoBehaviour
{
    public bool SetupRoom;

    public AnchorPositionMapper anchorPositionMapper;
    public SpatialAnchorLoaderBuildingBlock anchorLoader;

    public AnchorPrefabSpawner wallArtSpawner;
    public AnchorPrefabSpawner wallFaceSpawner;
    public AnchorPrefabSpawner floorSpawner;
    public AnchorPrefabSpawner ceilingSpawner;
    public AnchorPrefabSpawner tableSpawner;
    public AnchorPrefabSpawner couchSpawner;
    public AnchorPrefabSpawner storageSpawner;

    public GameObject defaultPainting;
    
    void Awake()
    {
       
    }
    
    void Start()
    {
        if (!SetupRoom)
            SetupScene();
    }
    
    void Update()
    {
        //if (!SetupRoom)
        //PlaceAnchors();

    }

    private void SetupScene()
    {
        anchorPositionMapper.enabled = true;
    }
    
    public void ResetRoom()
    {
        //delete GOs
        //...
        
        wallArtSpawner.enabled = false;
        wallArtSpawner.PrefabsToSpawn.Clear();
        AnchorPrefabSpawner.AnchorPrefabGroup paintingGroup = new AnchorPrefabSpawner.AnchorPrefabGroup();
        paintingGroup.Prefabs = new List<GameObject> { defaultPainting };
        wallArtSpawner.PrefabsToSpawn.Add(paintingGroup);
        wallArtSpawner.enabled = true;
        
        wallFaceSpawner.enabled = false;
        floorSpawner.enabled = false;
        ceilingSpawner.enabled = false;
        tableSpawner.enabled = false;
        couchSpawner.enabled = false;
        storageSpawner.enabled = false;
    }
}