using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Unity.Collections;

namespace BoxBearMaterialTools
{
    /// <summary>
    /// This class provides functionality for the MaterialTools package.
    /// </summary>
    public class MaterialToolsEngine
    {
        /// <summary> 
        /// Returns a list containing all of the materials being used by the gameobject 
        /// </summary>
        public static List<TempMaterial> SwitchShaders(GameObject _gameObject, Shader _newShader, bool logErrors, List<string> database, StringBuilder logReport)
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
                logReport.Append($"(Skipped, no renderer component)");
                return result; // Returns an empty list of temp materials.
            }

            foreach (Material material in renderer.sharedMaterials)
            {
                //Debug.Log($"Iterating material '{material.name}'");
                //logReport.Append($"({material.name})");

                // Protection against the shader not being 'Universal Render Pipeline/Lit'
                
                if (material.shader.name != "Universal Render Pipeline/Lit")
                {
                    logReport.Append($"(Skipped, shader is not Lit)");
                    // return result; Result gets out of the method completelly and skips the other slots of present.
                    continue;   // Continue gets out of the foreach loop.
                }

                // Convert each material into a TempMaterial, then add it to the "result" List. This list that this method returns
                // is then used by RecursiveFix (Method that called this one) to gather information about which and how the materials
                // changed.
                // ---
                // 10/23/2025 - I'm trying to have the Temp Material be created only -AFTER- the shader check above.
                // This is because trying to create a TempMaterial from a 'Box Bear/OpaqueLit' is problematic.
                // Perhaps the protection should be in the constructor level of the TempMaterial class.
                result.Add(new TempMaterial(material));

                // Shader Switch
                foreach (string itemCode in database)
                {
                    // FIX 1: Check material name, not fake folder path
                    if (material.name.Contains(itemCode, StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.LogWarning($"Material {material.name} was identified as a Special shader as it contains item code '{itemCode}'");

                        // FIX 2: Build the real folder path
                        string folderPath = $"Assets/LibraryPackage/Database/{itemCode}";

                        // FIX 3: Find all materials inside that folder
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

                        // FIX 4: Get correct path and load material
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

        /// <summary>
        /// This method gets called once per GameObject. Recursively interating through all the children by calling
        /// itself again once per children.
        /// </summary>
        /// <param name="obj"> GameObject to interate from. Might be a root if {level = 0} or passed through the logic inside itself. </param>
        /// <param name="level"> Pass 0 to initiate the Recursive fix, indicating it's the root GameObject. </param>
        /// <param name="logReport"></param>
        /// <param name="database"> List<String> containing all the codes for the Special Shaders.</param>
        public static void RecursiveFix(GameObject obj, int level, StringBuilder logReport, List<String> database)
        {
            logReport.AppendLine();

            // Level-based identation (This will represent a hierarchy in text form).
            string indent = new string(String.Empty);
            for (int i = 0; i < level; i++)
            {
                indent += "|  ";
            }
            
            // Append the hierarchy line with tab for alignment
            const int padWidth = 50;  // Adjust if names are longer/shorter
            string line = (indent + obj.name).PadRight(padWidth);
            logReport.Append(line);  // Now with Line for proper breaks!

            // Perform the fix
            Shader newShader = Shader.Find("Box Bear/OpaqueLit");
            if (newShader == null)
            {
                Debug.LogError("Can't find 'Universal Render Pipeline/Lit' shader.");
                return;
            }

            List<TempMaterial> tempMatList = SwitchShaders(obj, newShader, false, database, logReport);

            foreach (TempMaterial tempMat in tempMatList)
            {
                logReport.Append($"({tempMat.materialName})");
                Debug.LogWarning($"{obj.name} : {tempMat.materialName}");
            }

            // Recursion for the children
            foreach (Transform child in obj.transform)
            {
                RecursiveFix(child.gameObject, level + 1, logReport, database);
            }
        }

        /// <summary>
        /// Retrieves a list of immediate subdirectory names from the specified directory path, returning them as a List<string>
        /// </summary>
        public static List<string> GetAllSubfoldersNames(String _folderPath)
        {
            string[] subfolderArray = AssetDatabase.GetSubFolders(_folderPath);
            List<string> subfolderList = subfolderArray.Select(Path.GetFileName).ToList();

            return subfolderList;
        }

        /// <summary>
        /// Retrieves the Package path.
        /// </summary>
        /// <param name="isRelease"> Set to true before exporting as a Unity Package.</param>
        /// <returns></returns>
        public static String GetPath(bool isRelease)
        {
            string result = "Assets/LibraryPackage/";
            if (isRelease)
            {
                result = "Packages/com.boxbearllc.materialtools/";
            }
            return result;
        }
    }
}