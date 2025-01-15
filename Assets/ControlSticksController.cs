using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ControlSticksController : MonoBehaviour, IInteractable
{
    public static ControlSticksController Instance { get; private set; } 
    //state machine
    public enum SpaceshipState
    {
        PreparingForTakeoff,
        Landing
    }
    
    public SpaceshipState CurrentState { get; private set; } = SpaceshipState.PreparingForTakeoff; 

    public bool ReciveIsFlying;
    [SerializeField] private bool IsPulled = false;
    public UnityEvent OnButtonPressed;
    public UnityEvent OnButtonReleased;

    [SerializeField] private float rotationAngle = 30f;
    [SerializeField] private float rotationSpeed = 100f;

    private Quaternion initialRotation;
    public bool isRotating = false;
    
    public ScreenFade screenFade;
    [SerializeField] private GameObject door;
    

    private void Awake()
    {
     
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }

    void Start()
    {
        CurrentState = SpaceshipState.PreparingForTakeoff;
        initialRotation = transform.localRotation;
        IsPulled = false;
        
    }

    public string GetInteractText()
    {
        if (LevelManager.Instance.isButtonSelected)
        {
            return string.Format("{0}", IsPulled ? "Close the hatch" : "Open the hatch");
        }
        return string.Format("{0}", "Select a level first");
    }

    public void OnInteract()
    {
        if(LevelManager.Instance.isButtonSelected) StartCoroutine(HandleSpaceshipState());
    }
    
    private IEnumerator HandleSpaceshipState()
    {
        if (CurrentState == SpaceshipState.PreparingForTakeoff)
        {
            IsPulled = true;
            LevelManager.Instance.LoadLevel();
            screenFade.TriggerScreenFade(false);
            TakeoffController.Instance.Rpc_OnInteract();
            AudioManager.Instance.PlayElevatorShakeSound(door);
            while (TakeoffController.Instance.IsFlying)
            {
                yield return null;
            }

            if (ReciveIsFlying && !isRotating)
            {
                
                yield return RotateToAngle(rotationAngle);
                OnButtonPressed?.Invoke();
                AudioManager.Instance.PlayElevatorCloseSound(door);
                yield return new WaitForSeconds(2f);
                SetState(SpaceshipState.Landing);
            }
        }
        else if (CurrentState == SpaceshipState.Landing)
        {
            if (!isRotating)
            {
                IsPulled = true;
                screenFade.TriggerScreenFade(true);
                yield return RotateToAngle(0f);
                OnButtonReleased?.Invoke();
                AudioManager.Instance.PlayElevatorCloseSound(door);
                yield return new WaitForSeconds(2f);
                
                TakeoffController.Instance.Rpc_OnInteract();
                AudioManager.Instance.PlayElevatorShakeSound(door);
                while (TakeoffController.Instance.IsFlying)
                {
                    yield return null;
                }
                LevelManager.Instance.DestroyLevel();
                SetState(SpaceshipState.PreparingForTakeoff);
            }
        }
    }

    private IEnumerator RotateToAngle(float targetAngle)
    {
        isRotating = true;

        Quaternion targetRotation = Quaternion.Euler(targetAngle, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);
        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null; 
        }

        transform.localRotation = targetRotation; 
        isRotating = false;
    }

    public void UpdateIsFlying(bool value)
    {
        ReciveIsFlying = value;
    }
    
    private void SetState(SpaceshipState newState)
    {
        CurrentState = newState;
    }
}
