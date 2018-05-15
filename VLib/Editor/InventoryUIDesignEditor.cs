using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace V
{
    [CustomEditor(typeof(InventoryUIDesign))]
    public class InventoryUIDesignEditor : Editor
    {
        InventoryUIDesign invUiDesign;
        Image slotDegin;

        bool showTitleDetail = true;
        bool slotDesignFoldOut;
        bool inventoryDesignFoldOut = true;
        bool inventoryBackgroundDesignFoldOut;
        bool inventoryCloseCross;
        bool inventoryCrossPosition;
        bool showInventoryCrossDesign;

        SerializedProperty inventoryTitle;
        SerializedProperty inventoryTitlePosX;
        SerializedProperty inventoryTitlePosY;
        SerializedProperty titleSizeX;
        SerializedProperty titleSizeY;
        SerializedProperty fontSize;
        SerializedProperty lineSpace;
        SerializedProperty supportRichText;
        SerializedProperty textColor;

        SerializedProperty panelSizeX;
        SerializedProperty panelSizeY;
        SerializedProperty inventoryCrossPosX;
        SerializedProperty inventoryCrossPosY;
        SerializedProperty showInventoryCross;

        int fontStyleOfInventory;
        private int imageTypeIndex = 2;
        private int imageTypeIndexForInventory = 2;
        private int imageTypeIndexForInventory2 = 2;

        public SerializedProperty InventoryCrossPosX
        {
            get
            {
                return inventoryCrossPosX;
            }

            set
            {
                inventoryCrossPosX = value;
            }
        }

        public SerializedProperty InventoryCrossPosY
        {
            get
            {
                return inventoryCrossPosY;
            }

            set
            {
                inventoryCrossPosY = value;
            }
        }

        public SerializedProperty ShowInventoryCross
        {
            get
            {
                return showInventoryCross;
            }

            set
            {
                showInventoryCross = value;
            }
        }

        public int ImageTypeIndexForInventory
        {
            get
            {
                return imageTypeIndexForInventory;
            }

            set
            {
                imageTypeIndexForInventory = value;
            }
        }

        public int ImageTypeIndexForInventory2
        {
            get
            {
                return imageTypeIndexForInventory2;
            }

            set
            {
                imageTypeIndexForInventory2 = value;
            }
        }

        //load data
        private void OnEnable()
        {
            invUiDesign = target as InventoryUIDesign;
            invUiDesign.LoadVariables();
            inventoryTitlePosX = serializedObject.FindProperty("inventoryTitlePosX");
            inventoryTitlePosY = serializedObject.FindProperty("inventoryTitlePosY");
            fontSize = serializedObject.FindProperty("fontSize");
            lineSpace = serializedObject.FindProperty("lineSpace");
            supportRichText = serializedObject.FindProperty("supportRichText");
            textColor = serializedObject.FindProperty("textColor");
            titleSizeX = serializedObject.FindProperty("titleSizeX");
            titleSizeY = serializedObject.FindProperty("titleSizeY");

            panelSizeX = serializedObject.FindProperty("panelSizeX");
            panelSizeY = serializedObject.FindProperty("panelSizeY");
            inventoryTitle = serializedObject.FindProperty("inventoryTitle");
            inventoryCrossPosX = serializedObject.FindProperty("inventoryCrossPosX");
            inventoryCrossPosY = serializedObject.FindProperty("inventoryCrossPosY");
            showInventoryCross = serializedObject.FindProperty("showInventoryCross");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (invUiDesign.uiType != InventoryUIDesign.UIType.Hotbar)
            {
                invUiDesign.LoadVariables();
            }

            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel++;
            inventoryDesignFoldOut = EditorGUILayout.Foldout(inventoryDesignFoldOut, "Inventory Design");
            if (inventoryDesignFoldOut)
            {
                showTitleDetail = EditorGUILayout.Foldout(showTitleDetail, "Title Design");
                if (showTitleDetail)
                {
                    EditorGUI.BeginChangeCheck();
                    inventoryTitle.stringValue = EditorGUILayout.TextField("Title", inventoryTitle.stringValue);
                    titleSizeX.intValue = EditorGUILayout.IntSlider("Title Width:", titleSizeX.intValue, 0, panelSizeX.intValue);
                    titleSizeY.intValue = EditorGUILayout.IntSlider("Title Height:", titleSizeY.intValue, 0, panelSizeY.intValue);
                    inventoryTitlePosX.intValue = EditorGUILayout.IntSlider("Position X:", inventoryTitlePosX.intValue, -panelSizeX.intValue / 2, panelSizeX.intValue / 2);
                    inventoryTitlePosY.intValue = EditorGUILayout.IntSlider("Position Y:", inventoryTitlePosY.intValue, -panelSizeY.intValue / 2, panelSizeY.intValue / 2);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        invUiDesign.ReloadPanelTitlePos();
                        invUiDesign.ReLoadPanelTitleText();
                        invUiDesign.ReloadTitleSize();
                    }
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.indentLevel++;
                    EditorGUILayout.TextArea("Character", EditorStyles.boldLabel);
                    invUiDesign.inventoryTitleText.font = (Font)EditorGUILayout.ObjectField("Font", invUiDesign.inventoryTitleText.font, typeof(Font), true);
                    string[] fontTypes = new string[4]; fontTypes[0] = "Normal"; fontTypes[1] = "Bold"; fontTypes[2] = "Italic"; fontTypes[3] = "BlodAndItalic";
                    fontStyleOfInventory = EditorGUILayout.Popup("Font Style", fontStyleOfInventory, fontTypes, EditorStyles.popup);
                    if (fontStyleOfInventory == 0)
                    {
                        invUiDesign.inventoryTitleText.fontStyle = FontStyle.Normal; fontStyleOfInventory = 0;
                    }
                    else if (fontStyleOfInventory == 1)
                    {
                        invUiDesign.inventoryTitleText.fontStyle = FontStyle.Bold; fontStyleOfInventory = 1;
                    }
                    else if (fontStyleOfInventory == 2)
                    {
                        invUiDesign.inventoryTitleText.fontStyle = FontStyle.Italic; fontStyleOfInventory = 2;
                    }
                    else if (fontStyleOfInventory == 3)
                    {
                        invUiDesign.inventoryTitleText.fontStyle = FontStyle.Italic; fontStyleOfInventory = 3;
                    }
                    fontSize.intValue = EditorGUILayout.IntField("Font Size", fontSize.intValue);
                    lineSpace.floatValue = EditorGUILayout.FloatField("Line Spacing", lineSpace.floatValue);
                    supportRichText.boolValue = EditorGUILayout.Toggle("Rich Text", supportRichText.boolValue);
                    textColor.colorValue = EditorGUILayout.ColorField("Color", textColor.colorValue);
                    invUiDesign.inventoryTitleText.material = (Material)EditorGUILayout.ObjectField("Material", invUiDesign.inventoryTitleText.material, typeof(Material), true);
                    EditorGUI.indentLevel--;
                    GUILayout.Space(20);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        invUiDesign.ReloadTitleFontInfo();
                    }
                }

            }
            EditorGUI.indentLevel--;
            GUILayout.EndVertical(); //end of InventoryDesign

            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel++;
            slotDesignFoldOut = EditorGUILayout.Foldout(slotDesignFoldOut, "Slot Design");
            if (slotDesignFoldOut)
            {
                try
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();
                    invUiDesign.slotInstanceImage.sprite = (Sprite)EditorGUILayout.ObjectField("Source Image", invUiDesign.slotInstanceImage.sprite, typeof(Sprite), true);
                    invUiDesign.slotInstanceImage.color = EditorGUILayout.ColorField("Color", invUiDesign.slotInstanceImage.color);
                    invUiDesign.slotInstanceImage.material = (Material)EditorGUILayout.ObjectField("Material", invUiDesign.slotInstanceImage.material, typeof(Material), true);
                    string[] imageTypes = new string[4]; imageTypes[0] = "Filled"; imageTypes[1] = "Simple"; imageTypes[2] = "Sliced"; imageTypes[3] = "Tiled";
                    imageTypeIndex = EditorGUILayout.Popup("Image Type", imageTypeIndex, imageTypes, EditorStyles.popup);
                    if (imageTypeIndex == 0)
                    {
                        invUiDesign.slotInstanceImage.type = Image.Type.Filled; imageTypeIndex = 0;
                    }
                    else if (imageTypeIndex == 1)
                    {
                        invUiDesign.slotInstanceImage.type = Image.Type.Simple; imageTypeIndex = 1;
                    }
                    else if (imageTypeIndex == 2)
                    {
                        invUiDesign.slotInstanceImage.type = Image.Type.Sliced; imageTypeIndex = 2;
                    }
                    else if (imageTypeIndex == 3)
                    {
                        invUiDesign.slotInstanceImage.type = Image.Type.Tiled; imageTypeIndex = 3;
                    }
                    invUiDesign.slotInstanceImage.fillCenter = EditorGUILayout.Toggle("Fill Center", invUiDesign.slotInstanceImage.fillCenter);
                    EditorGUI.indentLevel--;

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        invUiDesign.ReLoadAllSlots();
                    }
                }
                catch
                {

                }
            }

            EditorGUI.indentLevel--;
            GUILayout.EndVertical();

            SceneView.RepaintAll();
        }

    }
}
