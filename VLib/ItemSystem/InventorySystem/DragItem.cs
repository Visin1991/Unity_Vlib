using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace V
{

    public class DragItem : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler
    {

        private Vector2 pointerOffset;//??????
        private RectTransform itemObjRectTransform;
        private RectTransform dragSlotRectTransform;
        private CanvasGroup canvasGroup;
        private GameObject oldSlot;
        private InventoryUISetting inventory;
        private Transform draggedItemBox;
        private Transform previewsSlot;
        private readonly Vector3 slotCenter = new Vector3(0, 0, 0);
        private WeiClickManager clickManager;

        public delegate void ItemDelegate();
        public static event ItemDelegate UpdateInventoryList;

        private void Start()
        {
            clickManager = new WeiClickManager();
            itemObjRectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>(); //used to modify the state of children elements  Alpha, Raycasting, Enabled....
            dragSlotRectTransform = GameObject.FindGameObjectWithTag("DraggingItem").GetComponent<RectTransform>();
            inventory = transform.parent.parent.parent.GetComponent<InventoryUISetting>();
            draggedItemBox = GameObject.FindGameObjectWithTag("DraggingItem").transform;
        }

        //interface of IDragHandler
        public void OnDrag(PointerEventData data)
        {
            if (itemObjRectTransform == null)
                return;
            if (data.button == PointerEventData.InputButton.Left)
            {
                //itemObjRectTransform.SetAsLastSibling(); //Move the transform to the end of the local transform list.
                transform.SetParent(draggedItemBox);
                Vector2 localPointerPosition;
                canvasGroup.blocksRaycasts = false; //set false in OnDrag. So when we End Drag we can get the information of new position's rectTransform
                                                    ///https://docs.unity3d.com/ScriptReference/RectTransformUtility.ScreenPointToLocalPointInRectangle.html
                                                    ///use this to Out put the position related to the dragSlotObject
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(dragSlotRectTransform, Input.mousePosition, data.pressEventCamera, out localPointerPosition))
                {
                    itemObjRectTransform.localPosition = localPointerPosition;// - pointerOffset;// the position related to the dragSlotObject
                                                                              //Debug.Log(itemObjRectTransform.localPosition);
                }
                inventory.OnUpdateItemList();
            }
        }
        //interface of IPointerDownHandler
        public void OnPointerDown(PointerEventData data)
        {
            if (data.button == PointerEventData.InputButton.Left)
            {
                if (clickManager.DoubleClick())
                {
                    GetComponent<ItemHandleOnGUI>().DoubleClick();
                }
                ///https://docs.unity3d.com/ScriptReference/RectTransformUtility.ScreenPointToLocalPointInRectangle.html
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(itemObjRectTransform, data.position, data.pressEventCamera, out pointerOffset);
                oldSlot = transform.parent.gameObject;
            }
            if (UpdateInventoryList != null)
            {
                UpdateInventoryList();
            }
        }
        //interface of IEndDragHandler
        public void OnEndDrag(PointerEventData data)
        {
            //1. check MouseClick
            if (data.button == PointerEventData.InputButton.Left)
            {
                Transform releaseRTF = null;
                //1.1 check if Raycast hit a UI element
                if (data.pointerEnter != null)
                {
                    releaseRTF = data.pointerEnter.transform;

                    GameObject otherItemObj = null;
                    //1.1.1 check if it is a ItemObject UI
                    if (releaseRTF.parent.GetComponent<ItemHandleOnGUI>() != null)
                    {
                        otherItemObj = releaseRTF.parent.gameObject;
                        Transform otherItemObjSlot = otherItemObj.transform.parent;
                        //Put other ItemObj to draggedItemBox
                        otherItemObj.transform.SetParent(oldSlot.transform);
                        otherItemObj.transform.localPosition = slotCenter;
                        //Set current ItemObj to otherItemObjtSlot
                        transform.SetParent(otherItemObjSlot);
                        transform.localPosition = slotCenter;

                    }//1.1.2 check if it is a empty Slot UI
                    else if (releaseRTF.tag == "Slot")
                    {
                        if (releaseRTF.childCount > 0)
                        {
                            otherItemObj = releaseRTF.GetChild(0).gameObject;
                            otherItemObj.transform.SetParent(oldSlot.transform);
                            otherItemObj.transform.localPosition = slotCenter;
                        }
                        transform.SetParent(releaseRTF);
                        transform.localPosition = slotCenter;
                    }

                    else
                    {
                        transform.SetParent(oldSlot.transform);
                        transform.localPosition = slotCenter;
                    }
                }//end Hit Raycast hit a UI element
                else
                {//if we didn't hit any UI element
                    transform.SetParent(oldSlot.transform);
                    transform.localPosition = slotCenter;
                    GetComponent<ItemHandleOnGUI>().Clean();
                }
                canvasGroup.blocksRaycasts = true; //when we EndDrag, we need to reset it to true. So we can Drag it next time.
            }//end of Left Button
        }
    }
}
