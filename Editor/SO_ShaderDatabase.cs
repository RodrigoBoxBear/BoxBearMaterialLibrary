using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoxBearMaterialTools
{
    [CreateAssetMenu(fileName = "MaterialDatabase", menuName = "Material Tools/Database", order = 100)]
    public class SO_ShaderDatabase : ScriptableObject
    {
        public List<ShaderDatabaseItem> SpecialShaders;
    }
}
