using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace V
{
    [CustomEditor(typeof(InventoryUISetting))]
    public class InventoryUISettingEditor : Editor
    {
        InventoryUISetting invUiSetting;

        //use SerializedProperty for multiple InventoryUISetting script access individually 
        SerializedProperty slotRow;
        SerializedProperty slotColumn;
        SerializedProperty slotSize;
        SerializedProperty iconSize;
        SerializedProperty stackable;
        SerializedProperty mainInventory;

        SerializedProperty slotsPaddingBetweenX;
        SerializedProperty slotsPaddingBetweenY;
        SerializedProperty slotsPaddingLeft;
        SerializedProperty slotsPaddingRight;
        SerializedProperty slotsPaddingTop;
        SerializedProperty slotsPaddingBottom;
        SerializedProperty positionNumberX;
        SerializedProperty positionNumberY;

        private bool showStackableItems;

        public bool showInventorySettings = true;
        public bool showInventoryPadding = false;
        private bool showStackableItemsSettings = false;

        private int itemID;
        private int itemValue = 1;
        //private int imageTypeIndex;

        //This function is called when the object is loaded.
        private void OnEnable()
        {
            invUiSetting = target as InventoryUISetting;
            //here serializedObject is Representing the object or objects being inspected.
            slotRow = serializedObject.FindProperty("row");
            slotColumn = serializedObject.FindProperty("column");
            slotSize = serializedObject.FindProperty("slotSize");
            iconSize = serializedObject.FindProperty("iconSize");
            slotsPaddingBetweenX = serializedObject.FindProperty("paddingBetweenX");
            slotsPaddingBetweenY = serializedObject.FindProperty("paddingBetweenY");
            slotsPaddingLeft = serializedObject.FindProperty("paddingLeft");
            slotsPaddingRight = serializedObject.FindProperty("paddingRight");
            slotsPaddingTop = serializedObject.FindProperty("paddingTop");
            slotsPaddingBottom = serializedObject.FindProperty("paddingBottom");
            mainInventory = serializedObject.FindProperty("mainInventory");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginVertical("Box");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mainInventory, new GUIContent("Player Inventory"));
            if (EditorGUI.EndChangeCheck())
            {
                invUiSetting.SetAsMain(); //if anything between Begin and End changed. then EditorGUI.EndChangeCheck() will return true.
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel++;

            GUILayout.BeginVertical("Box");
            showInventorySettings = EditorGUILayout.Foldout(showInventorySettings, "Inventory Settings");
            if (showInventorySettings)
            {
                ResetInventoryProporty();
            }
            GUILayout.EndVertical();


            if (invUiSetting.uiType != InventoryUISetting.UIType.EquipmentSystem)
            {
                GUILayout.BeginVertical("Box");
                showStackableItemsSettings = EditorGUILayout.Foldout(showStackableItemsSettings, "Stacking/Splitting");
                if (showStackableItemsSettings)
                {
                    StackableItemsSettings();
                    GUILayout.Space(20);
                }
                GUILayout.EndVertical();
            }

            EditorGUI.indentLevel--;
            GUILayout.EndVertical();

            SceneView.RepaintAll();
            GUILayout.BeginVertical("Box");
            AddItemGUI();
            GUILayout.EndVertical();
        }

        void AddItemGUI()
        {
            if (invUiSetting.uiType != InventoryUISetting.UIType.EquipmentSystem)
            {

                GUILayout.Label("Add a Item");

                EditorGUILayout.BeginHorizontal();
                ItemDataBaseList inventoryItemList = (ItemDataBaseList)Resources.Load("ItemDataBase");
                string[] items = new string[inventoryItemList.itemList.Count];
                for (int i = 0; i < items.Length; i++)
                {
                    items[i] = inventoryItemList.itemList[i].itemName;
                }
                itemID = EditorGUILayout.Popup("", itemID, items, EditorStyles.popup);
                itemValue = EditorGUILayout.IntField("", itemValue, GUILayout.Width(40));
                GUI.color = Color.green;

                if (GUILayout.Button("Add Item"))
                {
                    invUiSetting.addItemToPanel(itemID);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        void StackableItemsSettings()
        {

        }

        void ResetInventoryProporty()
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.IntSlider(slotRow, 1, 10, new GUIContent("Row"));
            EditorGUILayout.IntSlider(slotColumn, 1, 10, new GUIContent("Row"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                invUiSetting.Resize();
                invUiSetting.updateSlotAmount();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.IntSlider(slotSize, 20, 300, new GUIContent("Slot Size"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                invUiSetting.Resize();
                invUiSetting.updateSlotSize();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.IntSlider(iconSize, 20, 300, new GUIContent("Icon Size"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                invUiSetting.UpdateIconSize();
            }

            GUILayout.BeginVertical("Box");
            showInventoryPadding = EditorGUILayout.Foldout(showInventoryPadding, "Padding");
            if (showInventoryPadding)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel++;
                slotsPaddingLeft.intValue = EditorGUILayout.IntField("Left:", slotsPaddingLeft.intValue);
                slotsPaddingRight.intValue = EditorGUILayout.IntField("Right:", slotsPaddingRight.intValue);
                slotsPaddingBetweenX.intValue = EditorGUILayout.IntField("Spacing X:", slotsPaddingBetweenX.intValue);
                slotsPaddingBetweenY.intValue = EditorGUILayout.IntField("Spacing Y:", slotsPaddingBetweenY.intValue);
                slotsPaddingTop.intValue = EditorGUILayout.IntField("Top:", slotsPaddingTop.intValue);
                slotsPaddingBottom.intValue = EditorGUILayout.IntField("Bottom:", slotsPaddingBottom.intValue);
                EditorGUI.indentLevel--;
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    invUiSetting.updatePadding();
                    invUiSetting.Resize();
                }
            }
            GUILayout.EndVertical();

            EditorGUI.indentLevel--;
        }
    }
}
