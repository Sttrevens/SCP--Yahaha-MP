using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("PostProcessing/Invert Color")]

public class InvertColor : VolumeComponent, IPostProcessComponent
{
    [Tooltip("Is the color inverted?")]
    public BoolParameter invert = new BoolParameter(false);
    public bool IsActive()
    {
        return (bool)invert; // 将 BoolParameter 转换为普通 bool 类型
    }
    public bool IsTileCompatible()
    {
        return false;
    }
}
