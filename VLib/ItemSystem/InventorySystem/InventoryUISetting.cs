using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace V
{
    public class InventoryUISetting : MonoBehaviour
    {
        public enum UIType { Storage, EquipmentSystem, Hotbar, Store }
        public UIType uiType;

        public bool mainInventory;
        Vector2 currentResolution;
        public float panelWidth;
        public float panelHeight;

        public int column;
        public int row;
        public int slotSize;
        public int iconSize;
        public int paddingBetweenX;
        public int paddingBetweenY;
        public int paddingTop;
        public int paddingBottom;
        public int paddingLeft;
        public int paddingRight;

        RectTransform mainPanelRTF;

        GameObject title;
        RectTransform titleRTF;

        GameObject slotsGroup;
        RectTransform slotsGroupRTF;
        GridLayoutGroup slotGridLayout;

        GameObject button;
        RectTransform buttonRTF;

        GameObject prefabSlot;
        GameObject prefabItemObj;

        [SerializeField]
        public List<Item> itemsInPanel = new List<Item>();

        [SerializeField]
        private ItemDataBaseList itemDatabase;

        public RectTransform TitleRTF
        {
            get
            {
                return titleRTF;
            }

            set
            {
                titleRTF = value;
            }
        }

        public RectTransform ButtonRTF
        {
            get
            {
                return buttonRTF;
            }

            set
            {
                buttonRTF = value;
            }
        }

        public void SetAsMain()
        {
            if (mainInventory)
                this.gameObject.tag = "Untagged";
            else if (!mainInventory)
                this.gameObject.tag = "MainInventory";
        }

        public void ResetUILayout()
        {

            GetAndSetData();

            UITypeProcess();

            Resize();
            updateSlotAmount();
            updateSlotSize();
            updatePadding();
        }

        public void GetAndSetData()
        {
            mainPanelRTF = GetComponent<RectTransform>();
            if (mainPanelRTF == null)
            {
                Debug.Log(4);
            }
            title = transform.GetChild(0).gameObject;
            titleRTF = title.GetComponent<RectTransform>();

            slotsGroup = transform.GetChild(1).gameObject;
            slotsGroupRTF = slotsGroup.GetComponent<RectTransform>();
            slotGridLayout = slotsGroup.GetComponent<GridLayoutGroup>();

            button = transform.GetChild(2).gameObject;
            buttonRTF = button.GetComponent<RectTransform>();

            LoadResources();
        }

        public void UITypeProcess()
        {
            if (uiType == UIType.Hotbar)
            {
                mainPanelRTF.localPosition = new Vector3(0.0f, -350f, 0.0f);
                panelWidth = mainPanelRTF.sizeDelta.x;
                panelHeight = mainPanelRTF.sizeDelta.y;
                title.SetActive(false);
                button.SetActive(false);
                SetDefaultLayoutHotbar();
            }
            else if (uiType == UIType.Storage)
            {
                mainPanelRTF.localPosition = new Vector3(0.0f, 0f, 0.0f);
                SetDefaultStorage();
            }
        }

        public void SetDefaultStorage()
        {
            column = 9;
            row = 9;

            slotSize = 70;
            iconSize = 70;
            paddingBetweenX = 5;
            paddingBetweenY = 5;
            paddingTop = 50;
            paddingBottom = 10;
            paddingLeft = 10;
            paddingRight = 10;
        }

        public void SetDefaultLayoutHotbar()
        {
            column = 9;
            row = 1;

            slotSize = 100;
            iconSize = 100;
            paddingBetweenX = 5;
            paddingBetweenY = 5;
            paddingTop = 10;
            paddingBottom = 10;
            paddingLeft = 10;
            paddingRight = 10;
        }

        public void Resize()
        {
            int x = (((column * slotSize) + ((column - 1) * paddingBetweenX)) + paddingLeft + paddingRight);
            // Debug.Log(x);
            int y = (((row * slotSize) + ((row - 1) * paddingBetweenY)) + paddingTop + paddingBottom);
            GetAndSetData();
            mainPanelRTF.sizeDelta = new Vector2(x, y);
            slotsGroupRTF.sizeDelta = new Vector2(x, y);
        }

        public void updatePadding()
        {
            slotGridLayout.spacing = new Vector2(paddingBetweenX, paddingBetweenY);
            slotGridLayout.padding.top = paddingTop;
            slotGridLayout.padding.bottom = paddingBottom;
            slotGridLayout.padding.left = paddingLeft;
            slotGridLayout.padding.right = paddingRight;
        }

        public void updateSlotAmount()
        {

            slotsGroupRTF.localPosition = Vector3.zero;

            List<GameObject> slotList = new List<GameObject>();

            //get all Slot 
            foreach (Transform child in slotsGroup.transform)
            {
                if (child.tag == "Slot") { slotList.Add(child.gameObject); }
            }

            //protect the overflowing items
            List<Item> itemsToMove = new List<Item>();
            while (slotList.Count > column * row)
            {
                GameObject go = slotList[slotList.Count - 1];
                ItemHandleOnGUI itemInSlot = go.GetComponentInChildren<ItemHandleOnGUI>();
                if (itemInSlot != null)
                {
                    itemsToMove.Add(itemInSlot.item);
                    itemsInPanel.Remove(itemInSlot.item);
                }
                slotList.Remove(go);
                DestroyImmediate(go);
            }

            //Add more Slot
            if (slotList.Count < column * row)
            {
                for (int i = slotList.Count; i < (column * row); i++)
                {
                    GameObject slot = (GameObject)Instantiate(prefabSlot);
                    slot.name = "Slot " + (slotList.Count + 1).ToString();
                    slot.transform.SetParent(slotsGroup.transform);
                    slotList.Add(slot);
                }
            }

            if (itemsToMove != null && itemsInPanel.Count < column * row)
            {
                foreach (Item i in itemsToMove)
                {
                    addItemToPanel(i.itemID);
                }
            }
        }

        public void updateSlotSize()
        {
            slotGridLayout.cellSize = new Vector2(slotSize, slotSize);
            UpdateItemSize();
        }

        public void UpdateIconSize()
        {
            for (int i = 0; i < slotsGroup.transform.childCount; i++)
            {
                //is there a item i the slot
                if (slotsGroup.transform.GetChild(i).childCount > 0)
                {
                    //slotGroup-->slot-->item-->iconObj
                    slotsGroup.transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(iconSize, iconSize);
                }
            }
        }

        void UpdateItemSize()
        {
            //check each slot
            for (int i = 0; i < slotsGroup.transform.childCount; i++)
            {
                //if there is a item in the slot?
                if (slotsGroup.transform.GetChild(i).childCount > 0)
                {
                    slotsGroup.transform.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(slotSize, slotSize);
                }
            }
        }

        public void addItemToPanel(int id)
        {
            if (slotsGroup == null)
            {
                slotsGroup = transform.GetChild(1).gameObject;
            }
            for (int i = 0; i < slotsGroup.transform.childCount; i++)
            {
                //if the slot is empty
                if (slotsGroup.transform.GetChild(i).childCount == 0)
                {
                    if (prefabItemObj == null) { prefabItemObj = Resources.Load("Prefabs/ItemObj") as GameObject; }
                    GameObject itemObj = (GameObject)Instantiate(prefabItemObj);
                    itemObj.GetComponent<ItemHandleOnGUI>().item = itemDatabase.getItemCopyByID(id);
                    itemObj.transform.SetParent(slotsGroup.transform.GetChild(i));                  // set the slot as parent
                    itemObj.GetComponent<RectTransform>().localPosition = Vector3.zero;             // set the Obj to the center of the slot
                    itemObj.transform.GetChild(0).GetComponent<Image>().sprite = itemObj.GetComponent<ItemHandleOnGUI>().item.itemIcon;
                    itemObj.GetComponent<ItemHandleOnGUI>().item.indexItemInList = itemsInPanel.Count - 1;
                    break;
                }
            }
            updateItemList();
        }

        public void updateItemList()
        {
            itemsInPanel.Clear();
            if (slotsGroup == null)
            {
                slotsGroup = transform.GetChild(1).gameObject;
            }
            for (int i = 0; i < slotsGroup.transform.childCount; i++)
            {
                Transform slotTrans = slotsGroup.transform.GetChild(i);
                //if the there is a ItemObj in side the Slot
                if (slotTrans.childCount != 0)
                {
                    itemsInPanel.Add(slotTrans.GetChild(0).GetComponent<ItemHandleOnGUI>().item);
                }
            }
        }

        public void statckableSettings()
        {
            for (int i = 0; i < slotsGroup.transform.childCount; i++)
            {
                if (slotsGroup.transform.GetChild(i).childCount > 0)
                {
                    ItemHandleOnGUI itemHandle = slotsGroup.transform.GetChild(i).GetChild(0).GetComponent<ItemHandleOnGUI>();
                    if (itemHandle.item.maxStack > 1)
                    {

                    }
                }
            }
        }

        public void LoadResources()
        {
            if (prefabItemObj == null)
            {
                prefabItemObj = Resources.Load("Prefabs/ItemObj") as GameObject;
            }
            if (itemDatabase == null)
            {
                itemDatabase = (ItemDataBaseList)Resources.Load("ItemDataBase");
            }
            if (prefabSlot == null)
            {
                prefabSlot = Resources.Load("Prefabs/Slot") as GameObject;
            }
        }

        public void OnUpdateItemList()
        {
            updateItemList();
        }
    }
}
