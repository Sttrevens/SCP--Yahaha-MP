using UnityEngine;

namespace DestroyIt
{
    /// <summary>
    /// This script listens to a Destructible object's DestroyedEvent and spawns a dropped item when it is destroyed.
    /// </summary>
    [RequireComponent(typeof(Destructible))]
    public class DropItem : MonoBehaviour
    {
        private Destructible _destObj;

        [Header("Drop Settings")]
        [Tooltip("The item prefab that should drop when the object is destroyed.")]
        public GameObject[] itemsToDrop;

        [Tooltip("The position offset from the destructible object where the item will be spawned.")]
        public Vector3 dropOffset = Vector3.zero;

        [Tooltip("Randomize the drop position slightly?")]
        public bool randomizeDropPosition = true;

        [Tooltip("Randomize the drop rotation?")]
        public bool randomizeDropRotation = false;

        [Header("Impact Settings")]
        [Tooltip("The minimum force applied to the dropped item.")]
        public float minDropForce = 2f;

        [Tooltip("The maximum force applied to the dropped item.")]
        public float maxDropForce = 5f;

        [Tooltip("The random spread of the drop force (in each direction).")]
        public float spreadFactor = 1f;

        [Header("Drop Probability Settings")]
        [Tooltip("The probability (0-1) that each item will drop when the object is destroyed.")]
        [Range(0f, 1f)]
        public float dropProbability = 0.5f;

        private void Start()
        {
            // Get the Destructible script on the object
            _destObj = gameObject.GetComponent<Destructible>();
            if (_destObj != null)
                _destObj.DestroyedEvent += OnDestroyed;
        }

        private void OnDisable()
        {
            // Unregister the event listener to avoid memory leaks
            if (_destObj == null) return;
            _destObj.DestroyedEvent -= OnDestroyed;
        }

        /// <summary>
        /// When the Destructible object is destroyed, this method is called to spawn a drop item.
        /// </summary>
        private void OnDestroyed()
        {
            Debug.Log($"{_destObj.name} was destroyed at world coordinates: {_destObj.transform.position}");

            // Check if dropPrefab is set before attempting to spawn
            if (itemsToDrop != null)
            {
                // Calculate the spawn position (with randomization if enabled)
                Vector3 spawnPosition = _destObj.transform.position + dropOffset;

                if (randomizeDropPosition)
                {
                    spawnPosition += new Vector3(
                        Random.Range(-0.5f, 0.5f),
                        Random.Range(1f, 2.5f),
                        Random.Range(-0.5f, 0.5f)
                    );
                }

                // Create the dropped item at the calculated position
                foreach (var item in itemsToDrop)
                {
                    // Check if the item should drop based on dropProbability
                    if (Random.value <= dropProbability)
                    {
                        GameObject droppedItem = Instantiate(item, spawnPosition, Quaternion.identity);

                        if (randomizeDropRotation)
                        {
                            droppedItem.transform.rotation = Random.rotation;
                        }

                        // Apply random force to the dropped item (if it has a Rigidbody)
                        Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            Vector3 randomDirection = new Vector3(
                                Random.Range(-spreadFactor, spreadFactor),
                                Random.Range(minDropForce, maxDropForce),
                                Random.Range(-spreadFactor, spreadFactor)
                            ).normalized;

                            float forceAmount = Random.Range(minDropForce, maxDropForce);
                            rb.AddForce(randomDirection * forceAmount, ForceMode.Impulse);
                        }

                        // Log the drop
                        Debug.Log($"Dropped item: {droppedItem.name} at position: {spawnPosition}");
                    }
                    else
                    {
                        Debug.Log($"Item {item.name} did not drop due to probability check.");
                    }
                }
            }
            else
            {
                Debug.LogWarning("Drop prefab is not assigned, no item will be dropped.");
            }
        }
    }
}
