using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class EmoteController : NetworkBehaviour
{
    public string emoteName;
    public AnimationClip animationClip;
    
    private GameObject _currentPlayer;
    private PlayerMovement _playerMovement;
    private NetworkMecanimAnimator _animator;

    // Start is called before the first frame update
    public override void Spawned()
    {
        _currentPlayer = GameObject.Find("CurrentPlayer");
        if (_currentPlayer != null && HasStateAuthority)
        {
            _playerMovement = _currentPlayer.GetComponent<PlayerMovement>();
            _animator = _currentPlayer.GetComponent<NetworkMecanimAnimator>();
            if (_animator != null)
            {
                _animator.SetTrigger(emoteName);
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        _playerMovement.isEmoting = true;
    }
    
    void OnDestroy()
    {
        _playerMovement.isEmoting = false;
        _animator.SetTrigger("Cancel " + emoteName);
        
        Debug.Log("Emote Destroyed");
    }
}
