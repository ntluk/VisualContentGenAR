using System;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.BuildingBlocks;

public class AnchorPositionMapper : MonoBehaviour
{
    public SpatialAnchorCoreBuildingBlock anchorSystem;
    public List<PrefabAnchorPairs> prefabAnchorPairs = new List<PrefabAnchorPairs>();

    private Dictionary<Guid, GameObject> mappings;
    
    [Serializable] 
    public class PrefabAnchorPairs
    {
        public string uuid;
        public GameObject prefab;

        public Guid anchorUuid
        {
            get
            {
                Guid g;
                if (Guid.TryParse(uuid, out g))
                    return g;
                return Guid.Empty;
            }
        }
    }

    private void Awake()
    {
        mappings = new Dictionary<Guid, GameObject>();
        foreach (var p in prefabAnchorPairs)
        {
            mappings.Add(p.anchorUuid, p.prefab);
        }
    }

    private void OnEnable()
    {
        anchorSystem.OnAnchorsLoadCompleted.AddListener(SpawnPrefabsAtAnchorPos);
    }
    
    private void SpawnPrefabsAtAnchorPos(List<OVRSpatialAnchor> anchors)
    {
        foreach (var anchor in anchors)
        {
            // set object transform
            Guid id = anchor.Uuid;
            GameObject gameObject = mappings[id];
            gameObject.transform.position = anchor.transform.position;
            gameObject.transform.rotation = anchor.transform.rotation;
            gameObject.SetActive(true);
            
        }
    }

}