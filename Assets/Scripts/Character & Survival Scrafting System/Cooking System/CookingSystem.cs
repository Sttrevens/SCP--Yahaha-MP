using UnityEngine;
using System.Collections;
using LPSurvivalEngine;
using Unity.VisualScripting;

public class CookingSystem : MonoBehaviour, IInteractable
{
    // The prefab of the pot
    public GameObject potPrefab;
    // The prefab of the food
    public GameObject foodPrefab;
    private ItemDatabase rawFoodItem;
    // The audio clip for the cooking process
    private AudioClip startCookingSound;
    private AudioClip cookingSound;
    private AudioClip cookedSound;
    // The particle system for the cooking effect
    private ParticleSystem cookingParticles;
    // The position of the stove
    public Transform stovePosition;

    // The current pot object in the scene
    private GameObject currentPot;
    // The current food object in the scene
    private GameObject currentFood;
    // The audio source for playing sounds
    private AudioSource audioSource;

    // Whether the cooking process is ongoing
    private bool isCooking = false;

    // Whether the food is cooked
    private bool isCooked = false;

    private GameObject player;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (currentFood == null) {isCooking = false;
            audioSource.Stop();
        }
    }

    // This method is called when the player interacts with the stove
    public void OnInteract()
    {
        if (currentPot == null)
        {
            if (player.GetComponent<Inventory>().selectedItem.item.isPot)
            {
                potPrefab = player.GetComponent<Inventory>().selectedItem.item.dropPrefab;
                currentPot = Instantiate(potPrefab, stovePosition.position + Vector3.up, Quaternion.identity);
                startCookingSound = currentPot.GetComponent<ItemObject>().item.startCookingSound;
                cookingSound = currentPot.GetComponent<ItemObject>().item.cookingSound;
                cookedSound = currentPot.GetComponent<ItemObject>().item.cookedSound;
                Rigidbody potRigidbody = currentPot.GetComponent<Rigidbody>();
                potRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                potRigidbody.constraints |= RigidbodyConstraints.FreezeRotation;

                player.GetComponent<Inventory>().RemoveSelectedItem();
            }
        }
        if (currentPot != null && !isCooking)
        {
            Debug.Log("Food is available and cooking is not ongoing, starting to cook.");

            if (player.GetComponent<Inventory>().selectedItem.item.canBeCooked)
            {
                rawFoodItem = player.GetComponent<Inventory>().selectedItem.item;
                foodPrefab = rawFoodItem.dropPrefab;
            }

            if (foodPrefab != null)
            {
                currentFood = Instantiate(foodPrefab, currentPot.transform.position + Vector3.up * 0.5f, Quaternion.identity);
                Rigidbody potRigidbody = currentFood.GetComponent<Rigidbody>();
                potRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                potRigidbody.constraints |= RigidbodyConstraints.FreezeRotation;
                player.GetComponent<Inventory>().RemoveSelectedItem();
                StartCoroutine(CookFood(cookingSound));
            }
            }
    }

    // This method is called when the player puts food into the pot
    public void OnPlayerPutFoodInPot(GameObject food)
    {
        // If there is a current pot and cooking is not in progress
        if (currentPot != null && !isCooking)
        {
            Debug.Log("There is a current pot and cooking is not ongoing, instantiating food into the pot.");
            currentFood = Instantiate(foodPrefab, currentPot.transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
    }

    // The cooking logic implemented as a coroutine
    private IEnumerator CookFood(AudioClip cookingSound)
    {
        Debug.Log("Starting the cooking process.");

        if (!isCooking && cookingSound != null)
        {
            audioSource.PlayOneShot(startCookingSound);
            isCooking = true;
        }

        yield return new WaitForSeconds(1);
        if (cookingSound != null)
        {
            // Play the cooking sound
            audioSource.clip = cookingSound;
            audioSource.Play();
            Debug.Log("Playing the cooking sound.");
        }

        // Play the cooking particle effect
        //ParticleSystem cookingEffect = Instantiate(cookingParticles, currentPot.transform.position + Vector3.up, Quaternion.identity);
        //cookingEffect.Play();
        Debug.Log("Playing the cooking particle effect.");

        // Wait for the specified cooking time
        yield return new WaitForSeconds(rawFoodItem.cookTime);
        Debug.Log("Cooking time has passed.");

        // When cooking is completed, change the state of the food
        if (currentFood != null && currentFood.GetComponent<ItemObject>().item.canBeCooked)
        {
            Debug.Log("Destroying the original food as cooking is completed.");
            Destroy(currentFood);
            Debug.Log("Instantiating the cooked version of the food.");
            currentFood = Instantiate(currentFood.GetComponent<ItemObject>().item.cookedItem.dropPrefab, currentPot.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            Rigidbody potRigidbody = currentFood.GetComponent<Rigidbody>();
            potRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            potRigidbody.constraints |= RigidbodyConstraints.FreezeRotation;
            // Here you can modify the state of the food to set it as the cooked version (e.g., change the material or model)
        }

        // Stop the cooking particle effect
        //cookingEffect.Stop();

        CheckIfContinueCooking();
    }

    void CheckIfContinueCooking()
    {
        if (currentFood != null && currentFood.GetComponent<ItemObject>().item.canBeCooked) { StartCoroutine(CookFood(cookedSound)); }
    }

    // This method is used to clean up the instantiated pot and food
    public void ClearCooking()
    {
        if (currentPot != null)
        {
            Debug.Log("Destroying the current pot.");
            Destroy(currentPot);
        }

        if (currentFood != null)
        {
            Debug.Log("Destroying the current food.");
            Destroy(currentFood);
        }
    }

    public string GetInteractText()
    {
        return string.Format("{0}", "Cook");
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }
}