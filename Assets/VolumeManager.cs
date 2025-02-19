// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.Universal;
//
// public class PostProcessingVolumeManager : MonoBehaviour
// {
//     // 在类加载时就创建实例（饿汉式单例）
//     public static PostProcessingVolumeManager Instance;
//
//     List<ScriptableRendererFeature> rendererFeaturesList = new List<ScriptableRendererFeature>();
//     public UniversalRendererData defaultRendererData;
//     private void Awake()
//     {
//         // 在确保只有一个Manager的时候没必要搞得太复杂，这个已经够用了
//         Instance = this;
//         Volume volume = GetComponent<Volume>();
//         
//     }
//     // Start is called before the first frame update
//     void Start()
//     {
//         
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         
//     }
//     
//     /// <summary>
//     /// 修改shader属性的一个Demo
//     /// </summary>
//     private void ChangeLineStyle()
//     {
//         var volume = GetComponentInChildren<Volume>();
//         if (!volume.profile.TryGet(out LineStyle lineStyle))
//         {
//             lineStyle = volume.profile.Add<LineStyle>();
//         }
//
//         lineStyle.lineStrength.value += 1;
//     }
//
//     /// <summary>
//     /// Attempts to create a renderer feature based on the provided parameters or configuration.
//     /// </summary>
//     /// <returns>True if the renderer feature was successfully created; otherwise, false.</returns>
//     public ScriptableRendererFeature TryCreateRendererFeature()
//     {
//         // var defaultRendererData = GetDefaultRendererData();
//         if (defaultRendererData == null)
//             return null;
//         var findResult = defaultRendererData.rendererFeatures.Find(f => f && f.GetType() == typeof(LineStyleRendererFeature));
//         if (findResult)
//             return findResult;
//
//         ScriptableRendererFeature feature = CreateEffectFeature();
//         defaultRendererData.rendererFeatures.Add(feature);
//         defaultRendererData.SetDirty();
//         return feature;
//     }
//
//     /// <summary>
//     /// Attempts to deactivate a renderer feature based on the provided parameters or configuration.
//     /// </summary>
//     /// <returns>True if the renderer feature was successfully deactivated; otherwise, false.</returns>
//     public void TryDeactivateRendererFeature()
//     {
//         if (defaultRendererData == null)
//             return;
//
//         defaultRendererData.rendererFeatures.RemoveAll(feature => feature == null);
//         
//         for (int i = defaultRendererData.rendererFeatures.Count - 1; i >= 0; i--)
//         {
//             var feature = defaultRendererData.rendererFeatures[i];
//             if (feature.GetType() == typeof(LineStyleRendererFeature))
//             {
//                 defaultRendererData.rendererFeatures.RemoveAt(i);
//                 DestroyImmediate(feature);
//                 break;
//             }
//         }
//         
//         defaultRendererData.SetDirty();
//     }
//
//     /// <summary>
//     /// Creates a new effect feature using the specified configuration or parameters.
//     /// </summary>
//     /// <returns>A boolean value indicating whether the effect feature was successfully created.</returns>
//     private ScriptableRendererFeature CreateEffectFeature()
//     {
//         var feature = ScriptableObject.CreateInstance(typeof(LineStyleRendererFeature)) as ScriptableRendererFeature;
//         feature.Create();
//         feature.SetActive(true);
//         return feature;
//     }
// }
