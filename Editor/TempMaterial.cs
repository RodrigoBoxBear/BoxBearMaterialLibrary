using System;
using System.Text;
using UnityEngine;

namespace BoxBearMaterialTools
{
    /// <summary>
    /// This asset holds information of property values/texture maps temporarily.
    /// The information stored in this class is to be used for holding the material properties
    /// of the 'Universal Render Pipeline/Lit original shader to the new shader, whatever it is.
    /// </summary>
    public class TempMaterial
    {
        public string materialName;
        public Color colorTint;
        public Texture2D basecolorTexture;
        public float roughnessValue;
        public Texture2D roughnessTexture;
        public Texture2D normalTexture;

        /// <summary> From what material was this TemAssetPBR generated from </summary>
        public Material origin;

        /// <summary> Simple constructor that uses color only. </summary>
        public TempMaterial(Color _color)
        {
            materialName = "New Material from color";
            colorTint = _color;
            basecolorTexture = Texture2D.whiteTexture;
            roughnessValue = 0.5f;
            roughnessTexture = Texture2D.grayTexture;
            normalTexture = Texture2D.normalTexture;
        }

        /// <summary> Constructor that takes a material and converts it into a TempAssetPBR. </summary>
        public TempMaterial(Material _material)
        {
            materialName = _material.name;
            colorTint = _material.GetColor("_Color");
            basecolorTexture = (Texture2D)_material.GetTexture("_MainTex");
            roughnessValue = 1f - _material.GetFloat("_Smoothness");    // Applies One Minus logic to convert Glossiness (Smoothness) to Roughness
        }

        /// <summary>
        /// Generates a formated text with information about this TempMaterial instance.
        /// </summary>
        public string GenerateReport()
        {
            StringBuilder sb = new StringBuilder($"Console Report for TempAssetPBR '{materialName}'");
            sb.AppendLine();
            sb.Append("Please open this log message to see the report.");
            sb.AppendLine();
            sb.AppendLine($"Material Name: {materialName}");
            sb.AppendLine($"Color Tint: {colorTint.ToString()}");
            sb.AppendLine($"Roughness Value: {roughnessValue}");

            return sb.ToString();
        }

        /// <summary>
        /// Converts a TempMaterial back to a material.
        /// </summary>
        public Material CreateMaterialFromTemp(String _name, Shader _shader)
        {
            Material result = new(_shader);
            result.color = colorTint;
            return result;
        }
    }
}
