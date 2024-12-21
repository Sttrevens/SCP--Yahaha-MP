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
    public AudioClip cookingSound;
    // The audio clip for when the cooking is completed
    public AudioClip cookedSound;
    // The particle system for the cooking effect
    public ParticleSystem cookingParticles;
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
    // The time needed for cooking
    private float cookingTime = 5f;

    // Whether the food is cooked
    private bool isCooked = false;

    private GameObject player;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // This method is called when the player interacts with the stove
    public void OnInteract()
    {
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
                currentFood = Instantiate(foodPrefab, stovePosition.position + Vector3.up, Quaternion.identity);
                StartCoroutine(CookFood());
            }
            }

            // If there is no current pot, instantiate one and place it on the stove
            if (currentPot == null)
        {
            Debug.Log("No current pot exists, instantiating a new pot.");
            currentPot = Instantiate(potPrefab, stovePosition.position + Vector3.up, Quaternion.identity);
        }

        // If the food is already cooked
        if (isCooked)
        {
            if (currentFood != null)
            {
                Debug.Log("Food is cooked, destroying the current food object.");
                Destroy(currentFood);
            }

            Debug.Log("Cooking is set to false as the food is cooked.");
            isCooking = false;
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
    private IEnumerator CookFood()
    {
        Debug.Log("Starting the cooking process.");
        isCooking = true;

        if (cookedSound != null)
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
        yield return new WaitForSeconds(cookingTime);
        Debug.Log("Cooking time has passed.");

        // When cooking is completed, change the state of the food
        if (currentFood != null && !isCooked)
        {
            Debug.Log("Destroying the original food as cooking is completed.");
            Destroy(currentFood);
            Debug.Log("Instantiating the cooked version of the food.");
            currentFood = Instantiate(rawFoodItem.cookedItem.dropPrefab, currentPot.transform.position + Vector3.up * 0.5f, Quaternion.identity);
            // Here you can modify the state of the food to set it as the cooked version (e.g., change the material or model)
        }

        if (cookedSound != null)
        {
            // Play the sound for cooking completion
            audioSource.clip = cookedSound;
            audioSource.Play();
            Debug.Log("Playing the sound for cooking completion.");
        }

        // Stop the cooking particle effect
        //cookingEffect.Stop();

        Debug.Log("Setting the food as cooked.");
        isCooked = true;
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