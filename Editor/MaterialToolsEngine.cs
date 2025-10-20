using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

namespace BoxBearMaterialTools
{
    /// <summary>
    /// This class provides functionality for the MaterialTools package.
    /// </summary>
    public class MaterialToolsEngine
    {
        /// <summary> Takes a material and changes its shader, keeping properties. </summary>
        public static void SwitchMaterialShader(Material _material, string _shaderName)
        {
            // Retrieve the shader we're switching to.
            Shader shaderToAssign = Shader.Find(_shaderName);
            if (shaderToAssign == null)
            {
                Debug.LogError($"Could not find shader '{_shaderName}'");
                return;
            }

            // Protection against overriding Unity's URP Lit Shader.
            if (_material.shader.name == "Universal Render Pipeline/Lit" && _material.name == "Lit")
            {
                Debug.Log($"Material '{_material.name}' was protected from being overriten because it uses an Unity default shader.");
                return;
            }

            // Gather information of the original shader by creating a TempMaterial
            TempMaterial tempMaterial = new TempMaterial(_material);
            Debug.Log($"Temp Material color is: {tempMaterial.colorTint}");

            // Since we didn't fall into any protections, let's finally assign the new shader.
            _material.shader = shaderToAssign;

            // TRANSFERING PROPERTIES FROM THE ORIGINAL TO THE NEW SHADER.
            // By making sure the new shader complies with the property names, Unity will handle the transition
            // and will automatically make sure to maintain all of the maps, colors and values.
            // However, the standard 'Universal Render Pipeline/Lit' shader uses 'MetallicSmoothness' instead
            // of the usual Glosiness/Roughness approach.
            // Since Blender uses Roughness, below is a logic to *fix* the Roughness Map, since it's not
            // automatically assigned by the FBX exporter.
        }

        /// <summary> Returns a list containing all of the materials being used by the gameobject </summary>
        public static List<TempMaterial> SwitchShaders(GameObject _gameObject, Shader _newShader, bool logErrors, List<string> database)
        {

            List<TempMaterial> result = new List<TempMaterial>();

            // Protection against no GameObject being passed in the arguments.
            if (_gameObject == null)
            {
                if (logErrors) Debug.LogWarning("No GameObject selected in Scene View. Ignoring Command.");
                return result; // Returns an empty list of temp materials.
            }

            // Protection against the GameObject not being a 3D Model with a valid renderer.
            Renderer renderer = _gameObject.GetComponent<Renderer>();
            if (renderer == null)
            {
                if (logErrors) Debug.LogWarning("RetrieveAllMaterials() failed. GameObject has no Mesh Renderer component.");
                return result; // Returns an empty list of temp materials.
            }

            foreach (Material material in renderer.sharedMaterials)
            {
                Debug.Log($"Iterating material '{material.name}'");

                // This warns if the material is not 'Universal Render Pipeline/Lit'
                if (material.shader.name != "Universal Render Pipeline/Lit")
                {
                    if (logErrors)
                    {
                        Debug.LogWarning("Material is not 'Universal Render Pipeline/Lit'");
                        return result;
                    }
                }
                    // Shader Switch
                    foreach (string itemCode in database)
                    {
                        // ✅ FIX 1: Check material name, not fake folder path
                        if (material.name.Contains(itemCode, StringComparison.OrdinalIgnoreCase))
                        {
                            Debug.LogWarning($"Material {material.name} was identified as a Special shader as it contains item code '{itemCode}'");

                            // ✅ FIX 2: Build the real folder path
                            string folderPath = $"Assets/LibraryPackage/Database/{itemCode}";

                            // ✅ FIX 3: Find all materials inside that folder
                            string[] guids = AssetDatabase.FindAssets("t:Material", new[] { folderPath });

                            if (guids.Length == 0)
                            {
                                Debug.LogError("No Material found in folder: " + folderPath);
                                break;
                            }
                            else if (guids.Length > 1)
                            {
                                Debug.LogWarning("Multiple Materials found in folder; loading the first one.");
                            }

                            // ✅ FIX 4: Get correct path and load material
                            string materialPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                            Material newMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

                            if (newMaterial != null)
                            {
                                Debug.Log("Loaded Material: " + newMaterial.name + " at path: " + materialPath);
                                // Apply it to renderer
                                renderer.sharedMaterial = newMaterial;
                            }
                            else
                            {
                                Debug.LogError("Failed to load Material from: " + materialPath);
                            }
                        }
                    }

                    // Apply the new shader after replacement
                    material.shader = _newShader;

                // Get the _MainTex name as a base to find the roughness map.
                string mainTexPath = AssetDatabase.GetAssetPath(material.GetTexture("_MainTex"));
                mainTexPath = mainTexPath.Replace("Basecolor", "Roughness");

                Texture2D roughnessTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(mainTexPath);
                material.SetTexture("_Roughness_Map", roughnessTexture);
            }

            // Just testing
            /* foreach (string stg in database)
            {
                Debug.Log($"Item {stg} in database");
            }*/

            return result;
        }


        public static void RecursiveFix(GameObject obj, int level, StringBuilder sb, List<String> database)
        {
            sb.AppendLine();

            // Level-based identation (example : --- Child, ---Grandson)
            string indent = new string('-', level * 3);
            sb.Append(indent + obj.name);

            // Perform the fix
            Shader newShader = Shader.Find("Box Bear/OpaqueLit");
            if (newShader == null)
            {
                Debug.LogError("Can't find 'Universal Render Pipeline/Lit' shader.");
                return;
            }




            SwitchShaders(obj, newShader, false, database);


            if (level >= 2)
                //Debug.LogError("More than 2 generations found.");
                sb.Append(" More than 2 generations found.");

            // Recursion for the children
            foreach (Transform child in obj.transform)
            {
                RecursiveFix(child.gameObject, level + 1, sb, database);
            }
        }

        public static List<string> GetAllSubfoldersNames()  // Fixed casing to lowercase 's' for consistency
        {
            string folderPath = "Assets/LibraryPackage/Database";

            string[] subfolderArray = AssetDatabase.GetSubFolders(folderPath);
            List<string> subfolderList = subfolderArray.Select(Path.GetFileName).ToList();

            //Debug
            //Debug.Log($"Found {subfolderList.Count} subfolders:");
            foreach (string subfolder in subfolderList)
            {
                //Debug.Log($"- {subfolder}");
            }

            return subfolderList;
        }

    }
}
