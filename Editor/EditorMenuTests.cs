using UnityEditor;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace BoxBearMaterialTools
{
    public class EditorMenuTests : MonoBehaviour
    {
        #region tests


        [MenuItem("Material Tools/Tests/Print TempAssetPBR", false, 200)]
        public static void PrintTempAssetPBR()
        {
            GameObject selection = Selection.activeGameObject;
            Shader boxBearLitShader = Shader.Find("Box Bear/OpaqueLit");

            List<TempMaterial> materialList = new();
            if (selection != null) materialList = MaterialToolsEngine.SwitchShaders(selection, boxBearLitShader, true, new List<string>()); // The new List is because i implemented the database but this "SwitchToBoxBearLit" is deprecated. delete the whole thing later.

            foreach (TempMaterial tMat in materialList)
            {
                //Debug.Log($"Material Name: {material.name}");
                tMat.ReportOnConsole();
            }
        }

        [MenuItem("Material Tools/Tests/Print selected object's hierarchy", false, 2000)]
        public static void PrintSelectionHierarchy()
        {
            if (Selection.activeGameObject != null)
            {
                GameObject root = Selection.activeGameObject;
                StringBuilder logReport = new StringBuilder();
                logReport.Append($"Full report of all the materials in the hierarchy from '{root.name}' :");
                logReport.AppendLine();
                logReport.AppendLine("Please open up the console to view the list!");

                RecursiveCheck(root, 0, logReport);

                Debug.Log(logReport.ToString());
            }
            else
            {
                Debug.LogWarning("Can not print selection as there is no object selected in the Scene window's hierarchy.");
            }
        }

        [MenuItem("Material Tools/Tests/Switch Shader Test", false, 200)]
        public static void SwitchShaderTest()
        {
            GameObject selection = Selection.activeGameObject;
            if (selection == null)
            {
                Debug.LogError("No GameObject selected in Scene View. Ignoring Command.");
                return;
            }
            Renderer renderer = selection.GetComponent<Renderer>();

            foreach (Material material in renderer.sharedMaterials)
            {
                if (material.shader.name != "Universal Render Pipeline/Lit")
                {
                    Debug.LogWarning("Material is not 'Universal Render Pipeline/Lit'");
                }

                MaterialToolsEngine.SwitchMaterialShader(material, "Shader Graphs/BoxBearStandardShader");
            }
        }



        #region Private Methods
        private static void RecursiveCheck(GameObject obj, int level, StringBuilder sb)
        {
            sb.AppendLine();
            // Level-based identation (example : --- Child, ---Grandson)
            string indent = new string('-', level * 3);
            sb.Append(indent + obj.name);

            // Get it's material
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterials != null)
            {
                Shader newShader = Shader.Find("Shader Graphs/TriplanarShader");
                if (newShader != null)
                {
                    // Debug.LogWarning("Found Triplanar Shader!");
                }
                else
                {
                    // Debug.LogError("Could not find Triplanar Shader");
                }

                foreach (Material material in renderer.sharedMaterials)
                {
                    if (material != null)
                    {
                        sb.Append($" (material = '{material.name}', shader = {material.shader.name})");
                    }
                }
            }

            if (level >= 2)
                //Debug.LogError("More than 2 generations found.");
                sb.Append(" More than 2 generations found.");

            // Recursion for the children
            foreach (Transform child in obj.transform)
            {
                RecursiveCheck(child.gameObject, level + 1, sb);
            }
        }

        #endregion
    }
    #endregion
}
