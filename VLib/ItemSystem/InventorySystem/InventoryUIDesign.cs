using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class InventoryUIDesign : MonoBehaviour
    {

        public enum UIType { Storage, EquipmentSystem, Hotbar, Store }
        public UIType uiType;

        //UIDesign
        [SerializeField]
        public Image slotInstanceImage;
        [SerializeField]
        public Image slotImage;
        [SerializeField]
        public Image panelImage;
        [SerializeField]
        public bool showInventoryCross;
        [SerializeField]
        public Image inventoryCrossImage;
        [SerializeField]
        public RectTransform inventoryCrossRectTransform;
        [SerializeField]
        public int inventoryCrossPosX;
        [SerializeField]
        public int inventoryCrossPosY;
        [SerializeField]
        public string inventoryTitle;
        [SerializeField]
        public Text inventoryTitleText;
        [SerializeField]
        public int inventoryTitlePosX;
        [SerializeField]
        public int inventoryTitlePosY;
        [SerializeField]
        public int panelSizeX;
        [SerializeField]
        public int panelSizeY;

        public int fontSize;
        public float lineSpace;
        public bool supportRichText;
        public Color textColor;
        public int titleSizeX;
        public int titleSizeY;

        public void LoadVariables()
        {
            inventoryTitlePosX = (int)transform.GetChild(0).GetComponent<RectTransform>().localPosition.x;
            inventoryTitlePosY = (int)transform.GetChild(0).GetComponent<RectTransform>().localPosition.y;
            panelSizeX = (int)GetComponent<RectTransform>().sizeDelta.x;
            panelSizeY = (int)GetComponent<RectTransform>().sizeDelta.y;
            inventoryTitle = transform.GetChild(0).GetComponent<Text>().text;
            inventoryTitleText = transform.GetChild(0).GetComponent<Text>();

            fontSize = transform.GetChild(0).GetComponent<Text>().fontSize;
            lineSpace = transform.GetChild(0).GetComponent<Text>().lineSpacing;
            supportRichText = transform.GetChild(0).GetComponent<Text>().supportRichText;
            textColor = transform.GetChild(0).GetComponent<Text>().color;
            titleSizeX = (int)transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x;
            titleSizeY = (int)transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;

            if (uiType != UIType.Hotbar)
            {
                inventoryCrossRectTransform = transform.GetChild(2).GetComponent<RectTransform>();
                inventoryCrossImage = transform.GetChild(2).GetComponent<Image>();
            }
            panelImage = GetComponent<Image>();
            slotImage = transform.GetChild(1).GetChild(0).GetComponent<Image>(); //get the first slotImage as reference
            slotInstanceImage = slotImage;
            slotInstanceImage.sprite = slotImage.sprite;
            slotInstanceImage.color = slotImage.color;
            slotInstanceImage.material = slotImage.material;
            slotInstanceImage.type = slotImage.type;
            slotInstanceImage.fillCenter = slotImage.fillCenter;
        }

        public void ReLoadPanelTitleText()
        {
            transform.GetChild(0).GetComponent<Text>().text = inventoryTitle;
        }
        public void ReloadTitleFontInfo()
        {
            transform.GetChild(0).GetComponent<Text>().fontSize = fontSize;
            transform.GetChild(0).GetComponent<Text>().lineSpacing = lineSpace;
            transform.GetChild(0).GetComponent<Text>().supportRichText = supportRichText;
            transform.GetChild(0).GetComponent<Text>().color = textColor;
        }
        public void ReloadPanelTitlePos()
        {
            transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(inventoryTitlePosX, inventoryTitlePosY, 0);
        }
        public void ReloadTitleSize()
        {
            transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(titleSizeX, titleSizeY);
        }
        public void ReLoadAllSlots()
        {
            Image slot = null;
            for (int i = 0; i < transform.GetChild(1).childCount; i++)
            {
                slot = transform.GetChild(1).GetChild(i).GetComponent<Image>();
                slot.sprite = slotInstanceImage.sprite;
                slot.color = slotInstanceImage.color;
                slot.material = slotInstanceImage.material;
                slot.type = slotInstanceImage.type;
                slot.fillCenter = slotInstanceImage.fillCenter;
            }
        }
    }
}
