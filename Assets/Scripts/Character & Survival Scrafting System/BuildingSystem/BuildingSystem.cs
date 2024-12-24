using UnityEngine;
using UnityEngine.InputSystem;

namespace LPSurvivalEngine
{
    public class BuildingSystem : Wieldable
    {
        public GameObject UIPlayer;
        [Space]
        [Header("Building System")]
        [Space]

        [Space]
        [Header("Placement Settings")]
        [Space]

        public float placementUpdateRate = 0.03f;
        public float placementMaxDistance = 0.1f;
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
            UIPlayer = GameObject.Find("UIPlayer");
            inventory = UIPlayer.transform.Find("Menu").gameObject;
                
            instance = this;
            cam = Camera.main;

            placementLayerMask = LayerMask.GetMask("Terrain", "Floor");
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

        void Update()
        {
            if (buildingObject != null && currentBuildingPreview != null && Time.time - lastPlacementUpdateTime > placementUpdateRate)
            {
                lastPlacementUpdateTime = Time.time;

                Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, placementMaxDistance, placementLayerMask))
                {
                    Vector3 targetPosition = hit.point;

                    // Clamp the position to ensure it's within a certain range of the player (e.g., 5 units from the player)
                    float maxDistanceFromPlayer = 2.0f;  // Define the max radius from the player
                    Vector3 playerPosition = transform.position;  // Assuming this script is on the player object

                    // Calculate the distance between the player and the hit point
                    Vector3 directionToTarget = targetPosition - playerPosition;

                    // If the distance is greater than the max allowed distance, clamp it
                    if (directionToTarget.magnitude > maxDistanceFromPlayer)
                    {
                        targetPosition = playerPosition + directionToTarget.normalized * maxDistanceFromPlayer;
                    }

                    // Set the preview building's position
                    currentBuildingPreview.transform.position = targetPosition;
                    currentBuildingPreview.transform.up = hit.normal;
                    currentBuildingPreview.transform.Rotate(new Vector3(0, YRotation, 0), Space.Self);

                    if (!currentBuildingPreview.CollidingWithObjects())
                    {
                        if (!canPlace)
                            currentBuildingPreview.CanPlace();

                        canPlace = true;
                    }
                    else
                    {
                        if (canPlace)
                            currentBuildingPreview.CannotPlace();

                        canPlace = false;
                    }
                }
            }

            if (Keyboard.current.rKey.isPressed)
            {
                YRotation += rotateSpeed * Time.deltaTime;

                if (YRotation > 360.0f)
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