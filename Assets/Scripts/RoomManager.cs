using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Meta.XR.BuildingBlocks;
using Meta.XR.MRUtilityKit;
using UnityEngine.Serialization;

public class RoomManager : MonoBehaviour
{
    public bool SetupMode;

    public SpatialAnchorCoreBuildingBlock anchorSystem;
    public AnchorPositionMapper anchorPositionMapper;
    public SpatialAnchorLoaderBuildingBlock anchorLoader;

    public AnchorPrefabSpawner wallArtSpawner;
    public AnchorPrefabSpawner wallFaceSpawner;
    public AnchorPrefabSpawner floorSpawner;
    public AnchorPrefabSpawner ceilingSpawner;
    public AnchorPrefabSpawner tableSpawner;
    public AnchorPrefabSpawner couchSpawner;
    public AnchorPrefabSpawner storageSpawner;
    public AnchorPrefabSpawner doorSpawner;
    public AnchorPrefabSpawner windowSpawner;
    public AnchorPrefabSpawner screenSpawner;
    public AnchorPrefabSpawner plantSpawner;
    public AnchorPrefabSpawner lampSpawner;
    public AnchorPrefabSpawner bedSpawner;
    public AnchorPrefabSpawner otherSpawner;
    // unkown?
    
    public GameObject defaultPainting;
    public GameObject virtualPainting;
    private bool placedFrame;
    
    void Start()
    {
        if (!SetupMode)
            SetupScene();
        
    }
    
    void Update()
    {
        //if (!SetupRoom)
        //PlaceAnchors();

    }

    private void LateUpdate()
    {
        if (!placedFrame && wallArtSpawner != null)
            if(wallArtSpawner.AnchorPrefabSpawnerObjects != null && wallArtSpawner.AnchorPrefabSpawnerObjects.Count > 0)
            {
                wallArtSpawner.AnchorPrefabSpawnerObjects.Values.ElementAt(0).gameObject.SetActive(true);
                
                /*defaultPainting.gameObject.transform.position = wallArtSpawner.AnchorPrefabSpawnerObjects.Values.ElementAt(0).gameObject
                    .transform.position;
                defaultPainting.gameObject.transform.rotation = wallArtSpawner.AnchorPrefabSpawnerObjects.Values.ElementAt(0).gameObject
                    .transform.rotation;
                defaultPainting.gameObject.transform.localScale = wallArtSpawner.AnchorPrefabSpawnerObjects.Values.ElementAt(0).gameObject
                    .transform.localScale;
                    
                //set z? coord if IF in Wall
                
                defaultPainting.gameObject.SetActive(true);
                placedFrame = true;*/
            }
        
    }

    private void SetupScene()
    {
        anchorPositionMapper.enabled = true;
    }

    private void ReplacePrefabWithInteractionFrame()
    {
        
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