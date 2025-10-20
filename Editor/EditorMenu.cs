using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;


namespace BoxBearMaterialTools
{
    public class EditorMenu
    {
        #region Tools

        [MenuItem("Material Tools/Fix All Shaders", false, 10)]
        public static void FixShadersAllHierarchy()
        {
            GameObject selection = Selection.activeGameObject;

            if (selection == null)
            {
                Debug.LogWarning("Fix All Shaders failed. Please select a gameobject in the Scene window.");
                return;
            }

            
                StringBuilder logReport = new StringBuilder();
                logReport.Append($"Fix All Shaders Report.  Root GameObject('{selection.name}') :");
                logReport.AppendLine();
                logReport.AppendLine("Please select this message to view the list.");

                List<String> database = MaterialToolsEngine.GetAllSubfoldersNames();
                MaterialToolsEngine.RecursiveFix(selection, 0, logReport, database);

            /* Debug.Log(logReport.ToString()); */
            MaterialToolsDebugger.OpenInNotepad(logReport);


            //*/
        }

        [MenuItem("Material Tools/Open Documentation", false, 20)]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://boxbear-my.sharepoint.com/:w:/g/personal/rodrigo_boxbear_co_uk/EQkHoMX2z_BPl-iKoAPfiRIBmy8yLsv5qyo_pMXwCowbeQ?e=jfn29e");
        }

        [MenuItem("Material Tools/Log", false, 30)]
        public static void LogInNotepad()
        {
            MaterialToolsDebugger.OpenInNotepad(new StringBuilder("Test"));
        }

        #endregion
    }
}