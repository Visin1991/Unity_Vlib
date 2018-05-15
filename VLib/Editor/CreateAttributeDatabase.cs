using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V
{
    public class CreateAttributeDatabase
    {
        public static ItemAttributeList asset;
        public static ItemAttributeList createItemAttributeDatabase()
        {
            asset = ScriptableObject.CreateInstance<ItemAttributeList>();
            AssetDatabase.CreateAsset(asset, "Assets/C#MyUnityLib/Resources/AttributeDatabase.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }
    }
}
