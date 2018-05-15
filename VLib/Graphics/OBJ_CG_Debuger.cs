using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    //This is a Computer Graphics Debuger Tool--------------------
    [ExecuteInEditMode]
    public class OBJ_CG_Debuger : MonoBehaviour
    {
        public GameObject guiObject_root;

        public GameObject headerObj;
        public GameObject ContentObj;
        GameObject previousSelection;
        Vector3 previousCameraPos;
        Vector3 previousObjPos;


        public void Create_Item_Graphics_Info_GUIPanel()
        {
            if (guiObject_root != null) { return; }
            if (GameUIPr.Instance == null) { Debug.LogError("物件不存在GameUIPr not exist"); return; }

            guiObject_root = new GameObject("Item Graphics Info");
            guiObject_root.AddComponent<Image>().color = new Color(0.4f, 0.4f, 0.8f, 0.7f);
            guiObject_root.AddComponent<DragPanel>();
            guiObject_root.transform.SetParent(GameUIPr.Instance.MainCanvasObject.transform);

            RectTransform rectTransform = guiObject_root.GetComponent<RectTransform>();
            rectTransform.Set_DeltaSize_Anchor_Left_Top(new Vector2(400, 350), new Vector2(20, 20));

            Color imageColor = new Color(173 / (float)255, 173 / (float)255, 240 / (float)255);
            V.UIHelper.VAnchorRect rect = new V.UIHelper.VAnchorRect(0.0f, 0.9f, 1.0f, 1.0f);

            //Setting header Icom, Text, toggle Button
            headerObj = GameUIPr.Create_Image(guiObject_root.transform, "Header", imageColor, null, rect);
            GameUIPr.Create_Text(headerObj.transform, "Header", 20, Color.red, null, V.UIHelper.VAnchorRect.Fill);

            rect = new V.UIHelper.VAnchorRect(0.0f, 0.01f, 1.0f, 0.89f);
            ContentObj = GameUIPr.Create_Image(guiObject_root.transform, "Content", imageColor, null, rect);
            GameUIPr.Create_Text(ContentObj.transform, "Content", 20, Color.red, null, V.UIHelper.VAnchorRect.Fill);

        }

#if UNITY_EDITOR
        void Update()
        {
            if (Camera.main == null) { Debug.Log("No main Camera"); return; }

            GameObject currentSelect = UnityEditor.Selection.activeGameObject;
            if (currentSelect == null) { return; }
            if ((currentSelect != null && currentSelect != previousSelection) ||
                currentSelect.transform.position != previousObjPos ||
                    Camera.main.transform.position != previousCameraPos
                )
            {
                SetItemName(currentSelect.name);
                float distance = (currentSelect.transform.position - Camera.main.transform.position).magnitude;
                string content = "物件距离摄像机距离为 ：  " + distance.ToString() + "\n";

                content += currentSelect.vGetMeshVerticesBoundsInfo();
                content += "Bound 距离系数为 ： ";
                content += currentSelect.vGetMeshBoundSize().Divide(distance).ToString();


                SetItemContent(content);

                previousSelection = currentSelect;
                previousObjPos = previousSelection.transform.position;
                previousCameraPos = Camera.main.transform.position;
            }
        }
#endif

        public void Editor_Release()
        {
            DestroyImmediate(guiObject_root);
        }
        public void SetItemName(string headerName)
        {
            Text text = headerObj.GetComponentInChildren<Text>();
            text.text = headerName;
        }
        public void SetItemContent(string content)
        {
            ContentObj.GetComponentInChildren<Text>().text = content;
        }
    }
}
