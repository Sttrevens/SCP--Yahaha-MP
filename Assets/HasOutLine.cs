using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Linework.Common.Attributes;
using UnityEngine.Serialization;

public class HasOutLine : MonoBehaviour
{
    [SerializeField] [RenderingLayerMask] private uint outlineLayer;
    
    private Renderer[] _renderers;
    private uint _originalLayer;

    private void Start()
    {
        // 要是
        _renderers = TryGetComponent<Renderer>(out var meshRenderer)
            ? new[] {meshRenderer} : GetComponentsInChildren<Renderer>();
        _originalLayer = _renderers[0].renderingLayerMask;
    }

    public void SetOutLine(bool enabled)
    {
        foreach (var rend in _renderers)
        {
            rend.renderingLayerMask = enabled 
                ? _originalLayer | 1u << (int)Mathf.Log(outlineLayer, 2)
                : _originalLayer;  
            Debug.Log("现在的mesh的LayerMask" + rend.renderingLayerMask);
        }
    }
}
