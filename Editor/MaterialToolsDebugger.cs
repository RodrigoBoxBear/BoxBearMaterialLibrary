using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace BoxBearMaterialTools
{
    public class MaterialToolsDebugger : MonoBehaviour
    {
        /// <summary>
        /// Opens up Notepad in the user's computer, showing the message in the parameter.
        /// </summary>
        public static void OpenInNotepad(StringBuilder _text)
        {
            try
            {
                string tempFilePath = Path.GetTempFileName();       // Create a temporary file
                File.WriteAllText(tempFilePath, _text.ToString());  // Write the text into the temporary file.
                Process.Start("notepad.exe", tempFilePath);         // Open the temporary file.
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"{e.Message}");
            }
        }
    }
}
