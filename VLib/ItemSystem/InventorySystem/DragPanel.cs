using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace V
{
    public class DragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler
    {

        private Vector2 pointerOffset;
        private RectTransform canvasRectTransform;
        private RectTransform panelRectTransform;

        private void Start()
        {
            canvasRectTransform = transform.parent.transform as RectTransform;
            panelRectTransform = GetComponent<RectTransform>();

        }

        public void OnPointerDown(PointerEventData data)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out pointerOffset);
            //Debug.Log(canvasRectTransform.sizeDelta);
            //Debug.LogFormat("Offset :{0}",pointerOffset);
        }

        public void OnDrag(PointerEventData data)
        {
            if (panelRectTransform == null)
            {
                return;
            }
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, data.pressEventCamera, out localPointerPosition))
            {
                panelRectTransform.localPosition = localPointerPosition - pointerOffset;
                //Debug.LogFormat("Local Position :{0}, Offset :{1}", localPointerPosition, pointerOffset);
            }
        }
    }
}
