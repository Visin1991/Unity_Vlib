using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace V
{

    public class GameUIPr : Singleton<GameUIPr>
    {

        protected GameUIPr() { }

        GameObject ___mainCanvasObj;
        public GameObject MainCanvasObject { get { if (___mainCanvasObj == null) { CheckAndInitMainCanvas(); } return ___mainCanvasObj; } }


        //UI Prefabs
        public MainManue PB_MainManue;
        public SettingPanel PB_SettingPanel;
        public LoadSavePanel PB_LoadSPanel;
        public SaveGamePanel PB_SavePanel;
        public LevelTransitionPanel PB_LevelTransitionPanel;
        public InventoryPanel PB_InventoryPanel;
        public ItemInfo PB_ItemInfo;

        static SliderObj SliderHorizontal { get {  return Resources.Load<SliderObj>("UIDefault\\Text_Slider_Horizontal"); } } 
        static SliderObj SliderVertical { get {  return Resources.Load<SliderObj>("UIDefault\\Text_Slider_Vertical"); } } 

        [HideInInspector]
        public GameObject mainManueObj;    //This is the main Manue Object----Press Icon Botton or press Esc key
        GameObject settingPanelObj; //Panel contains the Music sound ..... Button
        GameObject LoadSaveObj;     //Load the save data Panel
        GameObject SaveGameObj;
        GameObject loadingBackGroundPanelObj;
        GameObject LevelTransitionPanelObj;
        GameObject inventoryPanelObj;
        GameObject itemInfoObj;

        List<GameObject> subPanelObjs = new List<GameObject>();

        private void Start()
        {
            CheckAndInitMainCanvas();

            //----------------------------------------------------------
            //Create or Instanciate a Main Manue
            //-----------------------------------------------------------
            MainManue mainManue = GetComponentInChildren<MainManue>();
            if (mainManue == null)
            {
                if (PB_MainManue == null)
                {
                    MakingDefault_MainMenu();
                }
                else
                {
                    mainManueObj = Instantiate(PB_MainManue).gameObject;
                    mainManueObj.transform.SetParent(MainCanvasObject.transform);
                }
            }
            else
            {
                mainManueObj = mainManue.gameObject;
            }


            //----------------------------------------------------------
            //Create or Instanciate a Setting Pannel
            //-----------------------------------------------------------
            SettingPanel settingPanel = GetComponentInChildren<SettingPanel>();
            if (settingPanel == null)
            {
                if (PB_SettingPanel == null)
                {
                    MakingDefault_SettingPanel();
                    subPanelObjs.Add(settingPanelObj);
                }
                else
                {
                    settingPanelObj = Instantiate(PB_SettingPanel).gameObject;
                    settingPanelObj.transform.SetParent(MainCanvasObject.transform);
                    subPanelObjs.Add(settingPanelObj);
                }
            }
            else
            {
                settingPanelObj = settingPanel.gameObject;
                subPanelObjs.Add(settingPanelObj);
            }


            //Disable all ob sub Panel
            foreach (GameObject sub in subPanelObjs)
            {
                sub.SetActive(false);
            }

            mainManueObj.SetActive(false);
        }

        public void MailBox_SYS_UI_Event(SYS_UI_Event sysUIEvent)
        {

        }

        public void SwitchActivePanel(PanelType target)
        {
            //Enable Target
            if (target == PanelType.MainMenue)
            {
                if (mainManueObj != null)
                    mainManueObj.SetActive(true);
            }
            else if (target == PanelType.Setting)
            {
                if (settingPanelObj != null)
                {
                    settingPanelObj.SetActive(true);
                    settingPanelObj.transform.localPosition = Vector3.zero;
                }
            }
            else if (target == PanelType.LoadSave)
            {
                if (LoadSaveObj != null)
                    LoadSaveObj.SetActive(true);
            }
            else if (target == PanelType.SaveGame)
            {
                if (SaveGameObj != null)
                    SaveGameObj.SetActive(true);
            }

        }

        public void OpenLevelPanel()
        {
            if (LevelTransitionPanelObj != null)
            {
                LevelTransitionPanelObj.SetActive(true);
            }
        }

        public void LoadLPlayerDataFromLastIndex()
        {
            if (loadingBackGroundPanelObj != null)
                loadingBackGroundPanelObj.SetActive(false);
        }

        public void MainUI_ESC_Pressed()
        {
            if (mainManueObj != null)
            {
                //----------------------------------------------------
                //Make sure all of the sub-Mainpanel are not active
                //----------------------------------------------------
                bool subActive = false;
                foreach (GameObject obj in subPanelObjs)
                {
                    if (obj.activeSelf)
                    {
                        subActive = true;
                        break;
                    }
                }
                if (subActive) return;
                //----------------------------------------------------
                mainManueObj.SetActive(!mainManueObj.activeSelf);
                if (mainManueObj.activeSelf)
                {
                    mainManueObj.transform.SetAsLastSibling();
                }
                GameCentalPr.Instance.PauseResumeEvent(mainManueObj.activeSelf);
            }
        }

        public void InventoryPanelEvent()
        {
            if (inventoryPanelObj != null)
            {
                inventoryPanelObj.SetActive(!inventoryPanelObj.activeSelf);
                if (inventoryPanelObj.activeSelf)
                {
                    inventoryPanelObj.transform.localPosition = Vector3.zero;
                }
                else
                {
                    itemInfoObj.GetComponent<ItemInfo>().DeactiveItemInfo();
                }
            }
        }

        public void SettingButtonOnClick()
        {
            if (settingPanelObj != null)
            {
                settingPanelObj.SetActive(!settingPanelObj.activeSelf);

                if (settingPanelObj.activeSelf)
                {
                    settingPanelObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
                    Debug.Log(settingPanelObj.GetComponent<RectTransform>().localPosition);
                }
                else
                {
                    GameCentalPr.Instance.PauseResumeEvent(false);
                }
            }
        }

        public void AddItemToInventory(Item item)
        {
            if (inventoryPanelObj != null)
            {
                if (!inventoryPanelObj.activeSelf)
                {
                    inventoryPanelObj.SetActive(true);
                    inventoryPanelObj.transform.localPosition = new Vector3(999, 999, 0);
                    inventoryPanelObj.GetComponent<InventoryPanel>().AddItem(item);
                    inventoryPanelObj.SetActive(false);
                }
                else
                {
                    inventoryPanelObj.GetComponent<InventoryPanel>().AddItem(item);
                }
            }
        }

        public void ResumeFromeSave()
        {
            SaveGameObj.SetActive(false);
            GameCentalPr.Instance.PauseResumeEvent(false);
        }

        public void StartNewGame()
        {
            mainManueObj.SetActive(false);
            Time.timeScale = 1;
            GameCentalPr.Instance.StartNewGame();
        }

        public void OpenSavePanel()
        {
            if (SaveGameObj != null)
            {
                SaveGameObj.SetActive(!SaveGameObj.activeSelf);
                if (SaveGameObj.activeSelf)
                {
                    SaveGameObj.transform.SetAsLastSibling();
                }
                GameCentalPr.Instance.PauseResumeEvent(SaveGameObj.activeSelf);
            }
            else { Debug.Log("No Save Panel Exsit"); }
        }

        public void LoadSave(int index)
        {
            Time.timeScale = 1;
            loadingBackGroundPanelObj.SetActive(true);
            LoadSaveObj.SetActive(false);
            GameCentalPr.Instance.LoadSave_by_Index(index);
        }

        public void SaveGame(int index)
        {
            ResumeFromeSave();
            GameCentalPr.Instance.SaveGame(index);
        }

        public void CloseLoadingPanel()
        {
            if (loadingBackGroundPanelObj != null)
                loadingBackGroundPanelObj.SetActive(false);
        }

        public void NextLevel()
        {
            LevelTransitionPanelObj.SetActive(false);
            loadingBackGroundPanelObj.SetActive(true);
            GameCentalPr.Instance.NextLevel();
        }

        public enum PanelType
        {
            MainMenue,
            Setting,
            LoadSave,
            SaveGame
        }

        void CheckAndInitMainCanvas()
        {
            MainCanvas mainCanvas = FindObjectOfType<MainCanvas>();

            if (mainCanvas != null)
            {
                ___mainCanvasObj = mainCanvas.gameObject;
                bool isChildOfThis = mainCanvas.gameObject.transform.parent == transform;
                if (!isChildOfThis) { Debug.LogError("Main Canvas Not Set the Child Of the GameManager Object"); }
            }
            else
            {
                ___mainCanvasObj = new GameObject();
                ___mainCanvasObj.name = "mainCanvas";
                ___mainCanvasObj.AddComponent<MainCanvas>();
                Canvas canvas = ___mainCanvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                ___mainCanvasObj.AddComponent<CanvasScaler>();
                ___mainCanvasObj.AddComponent<GraphicRaycaster>();
                ___mainCanvasObj.transform.SetParent(transform);
                ___mainCanvasObj.layer = 5;

                GameObject eventObject = new GameObject();
                eventObject.name = "EventSystem";
                eventObject.AddComponent<EventSystem>();
                eventObject.AddComponent<StandaloneInputModule>();
                eventObject.transform.SetParent(transform);
            }
        }

        [ContextMenu("Create Main Menu")]
        void MakingDefault_MainMenu()
        {
            mainManueObj = new GameObject();
            mainManueObj.name = "MainMenu";
            mainManueObj.AddComponent<Image>().color = new Color(0.4f, 0.4f, 0.8f, 0.7f);
            mainManueObj.AddComponent<MainManue>();
            mainManueObj.AddComponent<DragPanel>();
            mainManueObj.transform.SetParent(MainCanvasObject.transform);
            RectTransform rectTransform = mainManueObj.GetComponent<RectTransform>();
            rectTransform.Set_DeltaSize_Anchor_ScreenPos(new Vector2(200, 400), new Vector2(0, 0));

            Color buttonColor = new Color(0.0f, 0.8f, 0.5f, 1.0f);
            V.UIHelper.VAnchorRect rect = new V.UIHelper.VAnchorRect(0.05f, 0.85f, 0.95f, 0.95f);

            Create_Button_TEXT<NewGameButton>(mainManueObj.transform, "New Game", buttonColor, null, rect, 14, Color.red, null);

            rect.MoveVertical(-0.15f);
            SwitchPanelButton switchToLoad = Create_Button_TEXT<SwitchPanelButton>(mainManueObj.transform, "Load", buttonColor, null, rect, 14, Color.red, null);
            switchToLoad.target = PanelType.LoadSave;

            rect.MoveVertical(-0.15f);
            SwitchPanelButton switchToSave = Create_Button_TEXT<SwitchPanelButton>(mainManueObj.transform, "Save", buttonColor, null, rect, 14, Color.red, null);
            switchToSave.target = PanelType.LoadSave;

            rect.MoveVertical(-0.15f);
            SwitchPanelButton switchToSetting = Create_Button_TEXT<SwitchPanelButton>(mainManueObj.transform, "Setting", buttonColor, null, rect, 14, Color.red, null);
            switchToSetting.target = PanelType.Setting;

            rect.MoveVertical(-0.3f);
            rect.ScaleVertical_Mid(0.7f);
            rect.ScaleHorizontal_Mid(0.3f);

            rect.MoveHorizontal(-0.2f);
            Create_Button_TEXT<SysButton>(mainManueObj.transform, "Resume", buttonColor, null, rect, 14, Color.red, null);

            rect.MoveHorizontal(0.4f);
            Create_Button_TEXT<QuitButton>(mainManueObj.transform, "Quit", buttonColor, null, rect, 14, Color.red, null);

        }

        [ContextMenu("Create Setting Panel")]
        void MakingDefault_SettingPanel()
        {
            settingPanelObj = new GameObject();
            settingPanelObj.name = "SettingPanel";
            settingPanelObj.AddComponent<Image>().color = new Color(0.4f, 0.4f, 0.8f, 0.7f);
            settingPanelObj.AddComponent<SettingPanel>();
            settingPanelObj.AddComponent<DragPanel>();
            settingPanelObj.transform.SetParent(MainCanvasObject.transform);
            RectTransform rectTransform = settingPanelObj.GetComponent<RectTransform>();

            //rectTransform.SetDeltaSize_At_ScreenPos(new Vector2(300, 350), new Vector2(0, 0));
            rectTransform.Set_DeltaSize_Anchor_Left_Top(new Vector2(400, 350), new Vector2(20, 20));


            Color imageColor = new Color(173 / (float)255, 173 / (float)255, 240 / (float)255);
            V.UIHelper.VAnchorRect rect = new V.UIHelper.VAnchorRect(0.0f, 0.9f, 1.0f, 1.0f);

            //Setting header Icom, Text, toggle Button
            GameObject imageObj = Create_Image(settingPanelObj.transform, "SettingObj", imageColor, null, rect);
            Create_Text(imageObj.transform, "Setting", 20, Color.red, null, V.UIHelper.VAnchorRect.Fill);
            Create_Button<SettingButton>(imageObj.transform, "SettingButton", Color.green, null, new V.UIHelper.VAnchorRect(0.9f, 0.0f, 1.0f, 1.0f));

            rect.ScaleHorizontal_Mid(0.9f);
            rect.MoveVertical(-0.15f);

            SliderObj masterVolumSlider = Create_Slider_Horizontal(settingPanelObj.transform, "MasterVolumSliderObj", "Master Volum", rect);
            AudioManager.Instance.masterSlider = masterVolumSlider;

            rect.MoveVertical(-0.15f);
            SliderObj sfxVolumSlider = Create_Slider_Horizontal(settingPanelObj.transform, "SFXVolumSliderOBj", "SFX Volum", rect);
            AudioManager.Instance.sfxSlider = sfxVolumSlider;

            rect.MoveVertical(-0.15f);
            SliderObj musicVolumSlider = Create_Slider_Horizontal(settingPanelObj.transform, "MusicVolumSliderOBj", "Music Volum", rect);
            AudioManager.Instance.musicSlider = musicVolumSlider;


        }

        public static T Create_Button_TEXT<T>(Transform parent, string name, Color buttonColor, Sprite buttonSprit, V.UIHelper.VAnchorRect rect, int fontSize, Color textColor, Font font) where T : MonoBehaviour
        {
            T t = Create_Button<T>(parent, name, buttonColor, buttonSprit, rect);
            Create_Text(t.transform, name, fontSize, textColor, font, new V.UIHelper.VAnchorRect(0, 0, 1, 1));
            return t;
        }

        public static T Create_Button<T>(Transform parent, string name, Color buttonColor, Sprite buttonSprit, V.UIHelper.VAnchorRect rect) where T : MonoBehaviour
        {
            GameObject buttonObj = new GameObject();
            buttonObj.name = name;
            Image image = buttonObj.AddComponent<Image>();
            image.color = buttonColor;
            image.sprite = buttonSprit;
            Button button = buttonObj.AddComponent<Button>();
            ColorBlock cb = button.colors;
            cb.normalColor = buttonColor;
            cb.highlightedColor = new Color(buttonColor.r + 0.2f, buttonColor.g + 0.2f, buttonColor.b + 0.2f);

            T t = buttonObj.AddComponent<T>();
            buttonObj.transform.SetParent(parent);
            RectTransform buttonRT = buttonObj.GetComponent<RectTransform>();
            buttonRT.Set_Match_Anchors(rect.min_LowerLeft, rect.max_UpperRight);

            //V.UIHelper.SetAndMatchAnchors(ref buttonRT, rect.min_LowerLeft, rect.max_UpperRight);
            return t;
        }

        public static void Create_Image_TEXT(Transform parent, string name, Color imageTintColor, Sprite imageSprit, V.UIHelper.VAnchorRect rect, int fontSize, Color textColor, Font font, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            GameObject imageObj = Create_Image(parent, name, imageTintColor, imageSprit, rect);
            Create_Text(imageObj.transform, name, fontSize, textColor, font, V.UIHelper.VAnchorRect.Fill);
        }

        public static GameObject Create_Image(Transform parent, string name, Color imageTintColor, Sprite imageSprit, V.UIHelper.VAnchorRect rect)
        {
            GameObject imageObject = new GameObject();
            imageObject.name = name;
            Image image = imageObject.AddComponent<Image>();
            image.color = imageTintColor;
            image.sprite = imageSprit;
            imageObject.transform.SetParent(parent);
            RectTransform imageRT = imageObject.GetComponent<RectTransform>();
            imageRT.Set_Match_Anchors(rect.min_LowerLeft, rect.max_UpperRight);

            //V.UIHelper.SetAndMatchAnchors(ref imageRT, rect.min_LowerLeft, rect.max_UpperRight);
            return imageObject;
        }

        public static void Create_Text(Transform parent, string content, int fontSize, Color textColor, Font font, V.UIHelper.VAnchorRect rect, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            GameObject textObj = new GameObject();
            textObj.name = "Text";
            Text text = textObj.AddComponent<Text>();
            text.fontSize = fontSize;
            text.color = textColor;
            text.alignment = alignment;
            text.text = content;
            if (font == null)
                text.font = Resources.Load<Font>("OpenSansBold");
            textObj.transform.SetParent(parent);
            RectTransform textRT = textObj.GetComponent<RectTransform>();
            textRT.Set_Match_Anchors(rect.min_LowerLeft, rect.max_UpperRight);
            // V.UIHelper.SetAndMatchAnchors(ref textRT, rect.min_LowerLeft, rect.max_UpperRight);
        }

        public static SliderObj Create_Slider_Vertical(Transform parent, string name, string textTitle, V.UIHelper.VAnchorRect rect)
        {
            SliderObj sliderObj = Instantiate(SliderVertical);
            sliderObj.gameObject.name = name;
            sliderObj.ReSetTitle(textTitle);
            sliderObj.transform.SetParent(parent);
            sliderObj.GetComponent<RectTransform>().Set_Match_Anchors(rect.min_LowerLeft, rect.max_UpperRight);
            return sliderObj;
        }

        public static SliderObj Create_Slider_Horizontal(Transform parent, string name, string textTitle, V.UIHelper.VAnchorRect rect)
        {
            SliderObj sliderObj = Instantiate(SliderHorizontal);
            sliderObj.gameObject.name = name;
            sliderObj.ReSetTitle(textTitle);
            sliderObj.transform.SetParent(parent);
            sliderObj.GetComponent<RectTransform>().Set_Match_Anchors(rect.min_LowerLeft, rect.max_UpperRight);
            return sliderObj;
        }

        private void Reset()
        {
            CheckAndInitMainCanvas();
        }
    }

}

