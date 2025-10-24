using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;


namespace BoxBearMaterialTools
{
    /// <summary>
    /// Entry point for the functionality of the Material Tools. This class is responsible for adding the "Material Tools"
    /// menu in the Unity editor.
    /// </summary>
    public class EditorMenu
    {
        [MenuItem("Material Tools/Fix Shaders", false, 10)]
        public static void FixShaders()
        {
            // Retrieve the GameObject selected in the Scene Window.
            GameObject selection = Selection.activeGameObject; 

            // Protection against having no GameObject selected.
            if (selection == null)
            {
                Debug.LogWarning("Fix All Shaders failed. Please select a gameobject in the Scene window.");
                return;
            }

            // Here we create a StringBuilder that will constantly have text added to throughout the whole process.
            // The result at the end will be the Log Report.
            StringBuilder logReport = new ();
                
            // Create the header for the Log Report.
            logReport.Append($"Fix Shaders Report.  Root GameObject('{selection.name}') :");
            logReport.AppendLine();

            // Get the list of codes for the Special Shaders by looking into the "Database" folder.
            List<String> database = MaterialToolsEngine.GetAllSubfoldersNames(MaterialToolsEngine.GetPath(true) + "Database");
            // Debug.LogWarning(MaterialToolsEngine.GetPath(false) + "Database");

            MaterialToolsEngine.RecursiveFix(selection, 0, logReport, database);

            // Open up the Log Report in Notepad, showing the user what happened to each material.
            MaterialToolsDebugger.OpenInNotepad(logReport);
        }

        [MenuItem("Material Tools/Open Documentation", false, 20)]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://boxbear-my.sharepoint.com/:w:/g/personal/rodrigo_boxbear_co_uk/EQkHoMX2z_BPl-iKoAPfiRIBmy8yLsv5qyo_pMXwCowbeQ?e=jfn29e");
        }


    }
}