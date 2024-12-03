using UnityEngine;
using UnityEngine.UIElements;

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
                    Instantiate(item, spawnPosition, Quaternion.identity);

                    if (randomizeDropRotation)
                    {
                        item.transform.rotation = Random.rotation;
                    }

                    // Optionally, you can apply velocity or forces to the dropped item (e.g., Rigidbody)
                    Rigidbody rb = item.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);  // Example of giving the item an upward force
                    }

                    // Log the drop
                    Debug.Log($"Dropped item: {item.name} at position: {spawnPosition}");
                }
            }
            else
            {
                Debug.LogWarning("Drop prefab is not assigned, no item will be dropped.");
            }
        }
    }
}
