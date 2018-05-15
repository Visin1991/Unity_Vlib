using System.Collections;
using UnityEngine;
using UnityEditor;

namespace V
{
    public class CreateItemDataBase
    {
        public static ItemDataBaseList asset;
        public static ItemDataBaseList createItemDatabase()
        {
            asset = ScriptableObject.CreateInstance<ItemDataBaseList>();
            AssetDatabase.CreateAsset(asset, "Assets/C#MyUnityLib/Resources/ItemDatabase.asset");
            AssetDatabase.SaveAssets();
            asset.itemList.Add(new Item());
            return asset;
        }
    }
}
