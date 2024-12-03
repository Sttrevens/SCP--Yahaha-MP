using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LPSurvivalEngine
{
    public class BuildingSystem : Wieldable
    {
    [Space]
    [Header("Building System")]
    [Space]

    [Space]
    [Header("Placement Settings")]
    [Space]

    public float placementUpdateRate = 0.03f;
    public float placementMaxDistance = 5.0f;
    public float rotateSpeed = 180.0f;

    [Space]
    [Space]

    public LayerMask placementLayerMask;
    
    [Space]
    [Space]
    
    public Vector3 placementPosition;

    [Space]
    [Header("Assignments")]
    [Space]

    public GameObject inventory;
    
    
    private Building buildingObject;
    private BuildingPlacer currentBuildingPreview;
    private bool canPlace;
    private float YRotation;
    private float lastPlacementUpdateTime;
    private Camera cam;
    public static BuildingSystem instance;
    
    void Awake ()
    {
        instance = this;
        cam = Camera.main;
    }

    /*void Start ()
    {
        buildingSystem = FindObjectOfType<Inventory>(true).gameObject;
    }*/

    public void OnBuild(InputAction.CallbackContext context)
    {
        if(buildingObject == null || currentBuildingPreview == null || !canPlace)
            return;

        Instantiate(buildingObject.spawnPrefab, currentBuildingPreview.transform.position, currentBuildingPreview.transform.rotation);

        for(int x = 0; x < buildingObject.cost.Length; x++)
        {
            for(int y = 0; y < buildingObject.cost[x].quantity; y++)
            {
                Inventory.instance.RemoveItem(buildingObject.cost[x].item);
            }
        }

        buildingObject = null;
        Destroy(currentBuildingPreview.gameObject);
        currentBuildingPreview = null;
        canPlace = false;
        YRotation = 0;
    }

    public void OnBuildCancel(InputAction.CallbackContext context)
    {
        if (currentBuildingPreview != null)
            Destroy(currentBuildingPreview.gameObject);

        inventory.SetActive(true);
        PlayerController.instance.ToggleCursor(true);
    }

    public void SetNewBuildingRecipe (Building item)
    {
        buildingObject = item;
        inventory.SetActive(false);
        PlayerController.instance.ToggleCursor(false);

        currentBuildingPreview = Instantiate(item.previewPrefab).GetComponent<BuildingPlacer>();
    }

    void Update ()
    {
        if(buildingObject != null && currentBuildingPreview != null && Time.time - lastPlacementUpdateTime > placementUpdateRate)
        {
            lastPlacementUpdateTime = Time.time;

            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, placementMaxDistance, placementLayerMask))
            {
                currentBuildingPreview.transform.position = hit.point;
                currentBuildingPreview.transform.up = hit.normal;
                currentBuildingPreview.transform.Rotate(new Vector3(0, YRotation, 0), Space.Self);

                if(!currentBuildingPreview.CollidingWithObjects())
                {
                    if(!canPlace)
                        currentBuildingPreview.CanPlace();

                    canPlace = true;
                }
                else
                {
                    if(canPlace)
                        currentBuildingPreview.CannotPlace();

                    canPlace = false;
                }
            }
        }

        if(Keyboard.current.rKey.isPressed)
        {
            YRotation += rotateSpeed * Time.deltaTime;

            if(YRotation > 360.0f)
                YRotation = 0.0f;
        }    
    }

    void OnDestroy ()
    {
        if (currentBuildingPreview != null)
        Destroy(currentBuildingPreview.gameObject);
    }
}

}