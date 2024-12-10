using UnityEditor;
using UnityEngine;

namespace LPSurvival {
public class Documentation : EditorWindow
{
    [MenuItem("Tools/GrownAssets/SurvivalSystem/Help")]

    private static void OnGUI() 
    {
        Application.OpenURL("https://grownassetss-organization.gitbook.io/grown-assets-or-survival-engine/");
    }

}
 
}