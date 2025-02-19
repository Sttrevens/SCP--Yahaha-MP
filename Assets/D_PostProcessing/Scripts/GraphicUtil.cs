// using UnityEngine;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.Universal;
//
// public class GraphicUtil : MonoBehaviour
// {
//     public UniversalRendererData defaultRendererData;
//
//     private bool enableLineStyle;
//     // Update is called once per frame
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.L))
//         {
//             if (!enableLineStyle)
//             {
//                 TryCreateRendererFeature();
//             }
//             else
//             {
//                 TryDeactivateRendererFeature();
//             }
//             enableLineStyle = !enableLineStyle;
//         }
//
//         if (Input.GetKeyDown(KeyCode.K))
//             ChangeLineStyle();
//     }
//
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
