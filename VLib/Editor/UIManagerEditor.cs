using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace V
{
    public class UIManagerEditor : EditorWindow
    {
        static Texture headTexture;
        static Texture skypeTexture;
        static Texture emailTexture;
        static Texture folderIcon;

        static ItemDataBaseList itemDatabaseList = null;
        static ItemAttributeList itemAttributeList = null;

        [MenuItem("Tools/V/Game Play/Inventory System/UIManager")]
        static void Init()
        {
            EditorWindow.GetWindow(typeof(UIManagerEditor));
            headTexture = Resources.Load<Texture>("EditorWindowTextures/headTexture");
            skypeTexture = Resources.Load<Texture>("EditorWindowTextures/skypeIcon");
            emailTexture = Resources.Load<Texture>("EditorWindowTextures/emailIcon");
            folderIcon = Resources.Load<Texture>("EditorWindowTextures/folderIcon");

            Object itemDatabase = Resources.Load("ItemDataBase");
            if (itemDatabase == null)
            {
                itemDatabaseList = CreateItemDataBase.createItemDatabase();
            }
            else
            {
                itemDatabaseList = (ItemDataBaseList)Resources.Load("ItemDatabase");
            }

            Object attributeDatabase = Resources.Load("AttributeDatabase");
            if (attributeDatabase == null)
            {
                itemAttributeList = CreateAttributeDatabase.createItemAttributeDatabase();
            }
            else
            {
                itemAttributeList = (ItemAttributeList)Resources.Load("AttributeDatabase");
            }

            //Object inputManager = Resources.Load("InputManager");

        }

        bool showInputManager;
        bool showItemDataBase;
        bool showBluePrintDataBase;

        List<bool> manageItem = new List<bool>();
        Vector2 scrollPosition;
        static KeyCode test;

        bool showItemAttributes;
        string addAttributeName = "";
        int attributeAmount = 0;
        int newAttributeIndex = 0;
        int newAttributeValue = 0;
        int[] attributeName;
        int[] attributeValue;

        int[] attributeNamesManage = new int[100];
        int[] attributeValueManage = new int[100];
        int attributeAmountManage;
        bool showItem;

        public int toolbarInt = 0;
        public string[] toolbarStrings = new string[] { "Create Item", "Manage Items" };

        void OnGUI()
        {
            Header();
            if (GUILayout.Button("Input Manager"))
            {
                showInputManager = !showInputManager;
                showItemDataBase = false;
                showBluePrintDataBase = false;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Itemdatabase"))
            {
                showInputManager = false;
                showItemDataBase = !showItemDataBase;
                showBluePrintDataBase = false;
            }
            if (GUILayout.Button("Blueprintdatabase"))
            {
                showInputManager = false;
                showItemDataBase = false;
                showBluePrintDataBase = !showBluePrintDataBase;
            }
            EditorGUILayout.EndHorizontal();

            if (showItemDataBase)
                ItemDataBase();
        }

        void Header()
        {
            //S.1
            GUILayout.BeginHorizontal();
            {
                if (headTexture == null || emailTexture == null || skypeTexture == null || folderIcon == null)
                {
                    headTexture = Resources.Load<Texture>("EditorWindowTextures/headTexture");
                    if (headTexture == null) { Debug.Log("No headTexture"); }
                    skypeTexture = Resources.Load<Texture>("EditorWindowTextures/skypeIcon");
                    if (skypeTexture == null) { Debug.Log("No skypeTexture"); }
                    emailTexture = Resources.Load<Texture>("EditorWindowTextures/emailIcon");
                    if (emailTexture == null) { Debug.Log("No emailTexture"); }
                    folderIcon = Resources.Load<Texture>("EditorWindowTextures/folderIcon");
                    if (folderIcon == null) { Debug.Log("No folderIcon"); }
                }

                GUI.DrawTexture(new Rect(10, 10, 75, 75), headTexture);
                GUILayout.Space(90);

                GUILayout.BeginVertical();
                {
                    GUILayout.Space(10);

                    GUILayout.BeginVertical("Box");
                    {
                        GUILayout.Label("「Informations」", EditorStyles.boldLabel);

                        GUILayout.BeginHorizontal();
                        {
                            GUI.DrawTexture(new Rect(95, 35, 15, 15), emailTexture);
                            GUILayout.Space(25);
                            GUILayout.Label("zhuzhanhao1991@Gmail.com");
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        {
                            GUI.DrawTexture(new Rect(95, 52, 15, 15), skypeTexture);
                            GUILayout.Space(25);
                            GUILayout.Label("Wei,Zhu");
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        {
                            GUI.DrawTexture(new Rect(95, 71, 15, 15), folderIcon);
                            GUILayout.Space(25);

                            GUILayout.BeginHorizontal();
                            {
                                //if (GUILayout.Button("Documentation and API",GUIStyle.none)){
                                if (GUILayout.Button("Documentation and API"))
                                {
                                    Application.OpenURL("https://docs.unity3d.com/ScriptReference/");
                                }
                                if (GUILayout.Button("Unity Assets Store"))
                                {
                                    Application.OpenURL("https://www.assetstore.unity3d.com/en/");
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndHorizontal();
                    }
                    //E.1.1.1
                    GUILayout.EndVertical();

                }
                //E.1.1
                GUILayout.EndVertical();
            }
            //E.1
            GUILayout.EndHorizontal();
        }

        void ItemDataBase()
        {
            EditorGUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings, GUILayout.Width(position.width - 18));
            }
            GUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                GUILayout.Space(10);

                if (toolbarInt == 0) //Create Item
                {
                    GUI.color = Color.green;
                    ///https://docs.unity3d.com/ScriptReference/GUILayout.Width.html
                    if (GUILayout.Button("Add Item", GUILayout.Width(position.width - 23))) //position.width : the width of the rectangle, measured from the X position
                    {
                        AddItem();
                        showItem = true;
                    }
                    if (showItem)
                    {
                        GUI.color = Color.white;

                        GUILayout.BeginVertical("Box", GUILayout.Width(position.width - 23));
                        try
                        {
                            itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].itemName = EditorGUILayout.TextField("Item Name", itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].itemName, GUILayout.Width(position.width - 30));
                            itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].itemID = itemDatabaseList.itemList.Count - 1;

                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("Item Description");
                                GUILayout.Space(47);
                                itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].itemDesc = EditorGUILayout.TextArea(itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].itemDesc, GUILayout.Width(position.width - 180), GUILayout.Height(70));
                            }
                            GUILayout.EndHorizontal();

                            itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].itemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].itemIcon, typeof(Sprite), false, GUILayout.Width(position.width - 33));
                            itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].itemPrefab = (GameObject)EditorGUILayout.ObjectField("Item Prefab", itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].itemPrefab, typeof(GameObject), false, GUILayout.Width(position.width - 33));
                            itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].itemType, GUILayout.Width(position.width - 33));
                            itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].rarity = EditorGUILayout.IntSlider("Rarity", itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].rarity, 0, 100);

                            GUILayout.BeginVertical("Box", GUILayout.Width(position.width - 33));
                            {
                                showItemAttributes = EditorGUILayout.Foldout(showItemAttributes, "Item attributes");
                                if (showItemAttributes)
                                {

                                    GUILayout.BeginHorizontal();
                                    {
                                        addAttributeName = EditorGUILayout.TextField("Name", addAttributeName);
                                        GUI.color = Color.green;
                                        if (GUILayout.Button("Add"))
                                            AddAttribute();
                                    }
                                    GUILayout.EndHorizontal();

                                    GUILayout.Space(10);
                                    GUI.color = Color.white;

                                    {
                                        EditorGUI.BeginChangeCheck();
                                        attributeAmount = EditorGUILayout.IntSlider("Amount", attributeAmount, 0, 50);
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            attributeName = new int[attributeAmount];
                                            attributeValue = new int[attributeAmount];
                                        }
                                    }

                                    string[] attributes = new string[itemAttributeList.itemAttributeList.Count];
                                    for (int i = 0; i < attributes.Length; i++)
                                    {
                                        attributes[i] = itemAttributeList.itemAttributeList[i].attributeName;
                                    }

                                    for (int k = 0; k < attributeAmount; k++)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        attributeName[k] = EditorGUILayout.Popup("Attribute " + (k + 1), attributeName[k], attributes, EditorStyles.popup);
                                        attributeValue[k] = EditorGUILayout.IntField("Value", attributeValue[k]);
                                        EditorGUILayout.EndHorizontal();
                                    }

                                    if (GUILayout.Button("Save"))
                                    {
                                        List<ItemAttribute> iA = new List<ItemAttribute>();
                                        for (int i = 0; i < attributeAmount; i++)
                                        {
                                            iA.Add(new ItemAttribute(attributes[attributeName[i]], attributeValue[i]));
                                        }
                                        itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].itemAttributes = iA;
                                    }
                                }
                            }
                            GUILayout.EndVertical();
                            itemDatabaseList.itemList[itemDatabaseList.itemList.Count - 1].indexItemInList = 999;
                        }
                        catch { }
                        GUILayout.EndVertical();
                    }
                }

                if (toolbarInt == 1) //manage Item
                {
                    if (itemDatabaseList == null)
                    {
                        itemDatabaseList = (ItemDataBaseList)Resources.Load("ItemDatabase");
                    }
                    if (itemDatabaseList.itemList.Count < 1)
                    {
                        GUILayout.Label("There is no Item in the Database!");
                    }
                    else
                    {
                        GUILayout.BeginVertical();
                        {
                            for (int i = 0; i < itemDatabaseList.itemList.Count; i++)
                            {
                                try
                                {
                                    manageItem.Add(false);

                                    GUILayout.BeginVertical("Box");
                                    {
                                        manageItem[i] = EditorGUILayout.Foldout(manageItem[i], "" + itemDatabaseList.itemList[i].itemName);
                                        if (manageItem[i])
                                        {
                                            EditorUtility.SetDirty(itemDatabaseList);

                                            GUI.color = Color.red;
                                            if (GUILayout.Button("Delete Item"))
                                            {
                                                itemDatabaseList.itemList.RemoveAt(i);
                                                EditorUtility.SetDirty(itemDatabaseList);
                                            }

                                            GUI.color = Color.white;
                                            itemDatabaseList.itemList[i].itemName = EditorGUILayout.TextField("Item Name", itemDatabaseList.itemList[i].itemName, GUILayout.Width(position.width - 45));
                                            itemDatabaseList.itemList[i].itemID = i;

                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.Label("Item ID");
                                                GUILayout.Label(" " + i);
                                            }
                                            GUILayout.EndHorizontal();

                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.Label("Item Description");
                                                GUILayout.Space(47);
                                                itemDatabaseList.itemList[i].itemDesc = EditorGUILayout.TextArea(itemDatabaseList.itemList[i].itemDesc, GUILayout.Width(position.width - 195), GUILayout.Height(70));
                                            }
                                            GUILayout.EndHorizontal();

                                            itemDatabaseList.itemList[i].itemIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", itemDatabaseList.itemList[i].itemIcon, typeof(Sprite), false, GUILayout.Width(position.width - 45));
                                            itemDatabaseList.itemList[i].itemPrefab = (GameObject)EditorGUILayout.ObjectField("Item Model", itemDatabaseList.itemList[i].itemPrefab, typeof(GameObject), false, GUILayout.Width(position.width - 45));
                                            itemDatabaseList.itemList[i].itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", itemDatabaseList.itemList[i].itemType, GUILayout.Width(position.width - 45));
                                            itemDatabaseList.itemList[i].rarity = EditorGUILayout.IntSlider("Rarity", itemDatabaseList.itemList[i].rarity, 0, 100);

                                            GUILayout.BeginVertical("Box", GUILayout.Width(position.width - 45));
                                            {
                                                showItemAttributes = EditorGUILayout.Foldout(showItemAttributes, "Item Attributes");
                                                if (showItemAttributes)
                                                {
                                                    int iaCount = itemDatabaseList.itemList[i].itemAttributes.Count;
                                                    string[] attributes = new string[iaCount];

                                                    string[] allAtributes = new string[itemAttributeList.itemAttributeList.Count];
                                                    for (int ii = 0; ii < itemAttributeList.itemAttributeList.Count; ii++)
                                                    {
                                                        allAtributes[ii] = itemAttributeList.itemAttributeList[ii].attributeName;
                                                    }

                                                    for (int t = 0; t < attributes.Length; t++)
                                                    {
                                                        attributes[t] = itemDatabaseList.itemList[i].itemAttributes[t].attributeName;
                                                        attributeNamesManage[t] = t;
                                                        attributeValueManage[t] = itemDatabaseList.itemList[i].itemAttributes[t].attributeValue;
                                                    }

                                                    for (int z = 0; z < iaCount; z++)
                                                    {
                                                        EditorGUILayout.BeginHorizontal();
                                                        {
                                                            GUI.color = Color.red;
                                                            if (GUILayout.Button("-"))
                                                            {
                                                                itemDatabaseList.itemList[i].itemAttributes.RemoveAt(z);
                                                            }
                                                            GUI.color = Color.white;
                                                            attributeNamesManage[z] = EditorGUILayout.Popup(attributeNamesManage[z], attributes, EditorStyles.popup);
                                                            itemDatabaseList.itemList[i].itemAttributes[z].attributeValue = EditorGUILayout.IntField("Value", itemDatabaseList.itemList[i].itemAttributes[z].attributeValue);
                                                        }
                                                        EditorGUILayout.EndHorizontal();
                                                    }

                                                    newAttributeIndex = EditorGUILayout.Popup(newAttributeIndex, allAtributes, EditorStyles.popup);
                                                    newAttributeValue = EditorGUILayout.IntField("Value:", newAttributeValue);

                                                    GUI.color = Color.green;
                                                    if (GUILayout.Button("+"))
                                                    {
                                                        string newAttributeName = itemAttributeList.itemAttributeList[newAttributeIndex].attributeName;
                                                        itemDatabaseList.itemList[i].itemAttributes.Add(new ItemAttribute(newAttributeName, newAttributeValue));
                                                    }

                                                    GUI.color = Color.white;
                                                    if (GUILayout.Button("Save"))
                                                    {
                                                        List<ItemAttribute> iA = new List<ItemAttribute>();
                                                        for (int k = 0; k < itemDatabaseList.itemList[i].itemAttributes.Count; k++)
                                                        {
                                                            iA.Add(new ItemAttribute(attributes[attributeNamesManage[k]], attributeValueManage[k]));
                                                        }
                                                        itemDatabaseList.itemList[i].itemAttributes = iA;
                                                        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
                                                        for (int z = 0; z < items.Length; z++)
                                                        {
                                                            ItemHandleOnGUI itemhandle = items[z].GetComponent<ItemHandleOnGUI>();
                                                            if (itemhandle.item.itemID == itemDatabaseList.itemList[i].itemID)
                                                            {
                                                                itemhandle.item = itemDatabaseList.itemList[i];
                                                                Debug.Log("Reset");
                                                            }
                                                        }
                                                        manageItem[i] = false;
                                                    }
                                                }
                                            }
                                            GUILayout.EndVertical();
                                            EditorUtility.SetDirty(itemDatabaseList);
                                        }
                                    }
                                    GUILayout.EndVertical();

                                }
                                catch { }
                            }
                        }
                        GUILayout.EndVertical();
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        void AddItem()
        {
            EditorUtility.SetDirty(itemDatabaseList);
            Item newItem = new Item();
            newItem.itemName = "New Item";
            itemDatabaseList.itemList.Add(newItem);
            EditorUtility.SetDirty(itemDatabaseList);
        }
        void AddAttribute()
        {
            EditorUtility.SetDirty(itemAttributeList);
            ItemAttribute newAttribute = new ItemAttribute();
            newAttribute.attributeName = addAttributeName;
            itemAttributeList.itemAttributeList.Add(newAttribute);
            addAttributeName = "";
            EditorUtility.SetDirty(itemAttributeList);
        }
    }
}
