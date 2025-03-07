using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Fusion;
using LPSurvivalEngine;

public class ControlSticksController : NetworkBehaviour, IInteractable
{
    public static ControlSticksController Instance { get; private set; } 
    //state machine
    public enum SpaceshipState
    {
        PreparingForTakeoff,
        Landing
    }
    
    [Networked] public SpaceshipState CurrentState { get; set; }

    public bool ReciveIsFlying;

    [Networked] public bool IsPulled { get; set; } = false;
    public UnityEvent OnButtonPressed;
    public UnityEvent OnButtonReleased;

    [SerializeField] private float rotationAngle = 30f;
    [SerializeField] private float rotationSpeed = 100f;

    private Quaternion initialRotation;
    public bool isRotating = false;
    
    public ScreenFade screenFade;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject spaceship;

    [Networked]
    public int currentDays { get; set; } = 1;

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

    public override void Spawned()
    {
        CurrentState = SpaceshipState.PreparingForTakeoff;
        initialRotation = transform.localRotation;
        IsPulled = false;
        
    }

    public string GetInteractText()
    {
        if (LevelManager.Instance.isButtonSelected)
        {
            return string.Format("{0}", IsPulled ? "Go home" : "Go to the Destination");
        }
        return string.Format("{0}", "Select a destination first");
    }

    public void OnInteract()
    {
        if(LevelManager.Instance.isButtonSelected) RPC_OnInteract();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnInteract()
    {
        Debug.Log($"isRotating: {isRotating}, IsFlying: {TakeoffController.Instance.IsFlying}, HandleSpaceshipState is null: {HandleSpaceshipState() == null}");
        if ((!isRotating && !TakeoffController.Instance.IsFlying))
        {
            StartCoroutine(HandleSpaceshipState());
        }
    }
    
    private IEnumerator HandleSpaceshipState()
    {
        if (CurrentState == SpaceshipState.PreparingForTakeoff)
        {
            yield return RotateToAngle(rotationAngle);
            IsPulled = false;
            AudioManager.Instance.PlayElevatorShakeSound(spaceship);
                yield return new WaitForSeconds(2f);
                TakeoffController.Instance.Rpc_OnInteract();
                Rpc_ScreenFade(false);
                yield return new WaitForSeconds(2f);
                LevelManager.Instance.LoadLevel();
                yield return RotateToAngle(19.303f);
            while (TakeoffController.Instance.IsFlying)
            {
                yield return null;
            }

            if (!TakeoffController.Instance.IsFlying && !isRotating)
            {
                Debug.Log("[ControlSticksController] StartRotation called");
                door.GetComponent<EnterRoom>().RPC_StartRotation();
                AudioManager.Instance.PlayElevatorCloseSound(door);
                yield return new WaitForSeconds(2f);
                SetState(SpaceshipState.Landing);
                IsPulled = true;
            }
        }
        else if (CurrentState == SpaceshipState.Landing)
        {
                IsPulled = true;
                Rpc_ScreenFade(true);
                yield return RotateToAngle(rotationAngle);
                door.GetComponent<EnterRoom>().RPC_ResetRotation();
                AudioManager.Instance.PlayElevatorCloseSound(door);
                yield return new WaitForSeconds(2f);

                AudioManager.Instance.PlayElevatorShakeSound(spaceship);
                yield return new WaitForSeconds(2f);
                TakeoffController.Instance.Rpc_OnInteract();
                
                while (TakeoffController.Instance.IsFlying)
                {
                    yield return null;
                }
                LevelManager.Instance.DestroyLevel();
                yield return RotateToAngle(19.303f);
                SetState(SpaceshipState.PreparingForTakeoff);
                if (currentDays < 3)
                {
                    currentDays++;
                }

                foreach (var player in GameObject.FindObjectsOfType<HealthSystem>())
                {
                        player.Rpc_Respawn();
                }
                IsPulled = false;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_ScreenFade(bool fade)
    {
        screenFade.TriggerScreenFade(fade);
    }

    private IEnumerator RotateToAngle(float targetAngle)
    {
        isRotating = true;

        Quaternion targetRotation = Quaternion.Euler(targetAngle, 180f, initialRotation.eulerAngles.z);
        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
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
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_HandlePlayerDeath()
{
    HealthSystem[] players = GameObject.FindObjectsOfType<HealthSystem>();
    bool allDied = true;

    foreach (var player in players)
    {
        Debug.Log("Player " + player.name + " health: " + player.playerHealth);
        if (player.playerHealth > 0)
        {
            allDied = false;
            break;
        }
    }

    if (allDied)
    {
        Debug.Log("All players are dead");
        if (CurrentState == SpaceshipState.Landing)
            StartCoroutine(HandleSpaceshipState());
    }
}
}
