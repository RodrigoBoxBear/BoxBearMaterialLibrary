using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace BoxBearMaterialTools
{
    public class MaterialToolsDebugger : MonoBehaviour
    {
        public static void OpenInNotepad(StringBuilder _text)
        {
            try
            {
                string tempFilePath = Path.GetTempFileName(); // Create a temporary file

                File.WriteAllText(tempFilePath, _text.ToString());

                Process.Start("notepad.exe", tempFilePath);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"{e.Message}");
            }
        }
    }
}
