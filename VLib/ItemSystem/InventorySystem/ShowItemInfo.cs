using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace V
{
    public class ShowItemInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public ItemInfo itemInfo;
        public GameObject itemInfoGameObject;
        public RectTransform canvasRectTransform;
        public RectTransform itemInfoRectTransform;
        private Item item;
        // Use this for initialization
        void Start()
        {
            if (GameObject.FindGameObjectWithTag("ItemInfo") != null)
            {
                itemInfo = GameObject.FindGameObjectWithTag("ItemInfo").GetComponent<ItemInfo>();
                itemInfoGameObject = GameObject.FindGameObjectWithTag("ItemInfo");
                itemInfoRectTransform = itemInfoGameObject.GetComponent<RectTransform>() as RectTransform;
            }
            canvasRectTransform = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>() as RectTransform;
        }

        public void OnPointerEnter(PointerEventData data)
        {

            if (itemInfo != null)
            {
                item = GetComponent<ItemHandleOnGUI>().item;
                itemInfo.item = item;
                itemInfo.ActivateItemInfo();
                if (canvasRectTransform == null)
                {
                    return;
                }

                Vector3[] slotCorners = new Vector3[4];
                GetComponent<RectTransform>().GetWorldCorners(slotCorners);
                //Because we set the LeftUp corner as the reference Origin Pivot for ItemInfo RectTransform.
                //So we put the ItemInfo position to the Right Bottom by set ItemInfo's RectTransform to slotCorners[3]
                Vector2 localPointerPosition;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, slotCorners[3], data.pressEventCamera, out localPointerPosition))
                {
                    itemInfoRectTransform.localPosition = localPointerPosition;
                    itemInfoRectTransform.localPosition = new Vector3(localPointerPosition.x, localPointerPosition.y + itemInfo.itemInfoHeight);

                }
            }//end itemInfo != null
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (itemInfo != null)
            {
                itemInfo.DeactiveItemInfo();
            }
        }
    }
}
