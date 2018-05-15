using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Threading;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace V
{
    //
    // Set Up the Tutorial System..........
    //
    // The Singleton patern make sure all of the publc variables of the GameCentalPr
    // can be access by  for example GameCentalPr.Instance.pause
    [RequireComponent(typeof(GameDataManager))]
    [RequireComponent(typeof(SceneInitCommandQueue))]
    [RequireComponent(typeof(SoundLib))]
    [RequireComponent(typeof(AudioManager))]
    [RequireComponent(typeof(MusicManager))]
    [RequireComponent(typeof(CursorManager))]
    [RequireComponent(typeof(GameUIPr))]
    public class GameCentalPr : Singleton<GameCentalPr>
    {

        protected GameCentalPr() { } // guarantee this will be always a singleton only - can't use the constructor!

        public System.Action<bool> Adapter_Pause;
        public System.Action Adapter_GameOver;

        public int currentLevel = 0;
        //bool loadSaveData;

        public TargetLE DefaultLETarget_PF;

        GameObject targetLEObject;


        public static BaseSerializableData[] allBuildInDatas;


        void Start()
        {
            DontDestroyOnLoad(this);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameUIPr.Instance.MainUI_ESC_Pressed();
            }

        }

        void InitalTarget()
        {
            targetLEObject = FindObjectOfType<TargetLE>().gameObject;
        }

        void InitTarget_Default()
        {
            if (DefaultLETarget_PF != null)
            {
                TargetLE target = Instantiate<TargetLE>(DefaultLETarget_PF);
                GameObject targetLEObj = target.gameObject;
            }
        }

        void OnApplicationPause(bool pause)
        {

        }

        void OnApplicationQuit()
        {

        }

        public void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        //============================================================

        public void StartNewGame()
        {
            currentLevel = 1;

            SceneManager.LoadScene(1, LoadSceneMode.Single);
            SceneInitCommandQueue.Enqueue(new SceneInitCommandInfo(Default_Init, null));
        }

        public void LoadSave_by_Index(int index)
        {
            //GameDataManager.RequestData(Generate_Big_Data, Print_Big_Data);

            string path = Path.Combine(Application.persistentDataPath, "Save" + index.ToString() + ".vSave");
            if (File.Exists(path))
            {

                //With the use.... when code get out of the scope......automatically Close.
                using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
                {
                    //1.) Read the Scene Name
                    string sceneName = reader.ReadString();

                    //2. push a load data command to command Queue. After we Change Scene, We will Load the Data
                    SceneInitCommandQueue.Enqueue(new SceneInitCommandInfo(LoadSave, path));

                    //3.) Load the Scene         
                    SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                }
            }
            else
            {
                Debug.Log("The Save is Invalidate");
            }
        }

        public void LoadSave_by_FilePath(string path)
        {

            BinaryReader reader = new BinaryReader(File.OpenRead(path));
            reader.ReadString();                       //Reader The sceneName again

            //3. Find all build in SerialiazableDatas
            allBuildInDatas = FindObjectsOfType<BaseSerializableData>();

            //4.) Deserialize All Data
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                string uniqueID = reader.ReadString();
                if (uniqueID == "runTimeSpawn")
                {
                    CreateAndLoad_RTS_Object(reader);
                }
                else
                {
                    LoadEditorTimeBuildInObject(reader, uniqueID);
                }
            }

            reader.Close();
        }

        //Runtime Spawn Object
        void CreateAndLoad_RTS_Object(BinaryReader reader)
        {
            System.Type t = System.Type.GetType(reader.ReadString());
            GameDataManager.Instance.InstantiatePrefeb_By_Type(t).DeSerializeData(reader);
        }

        void LoadEditorTimeBuildInObject(BinaryReader reader, string uniqueID)
        {
            //Because The Build In Object already in the scene, So we do not need to 
            //Creae a Instance of the Object, Just find the Data Object, and Serialize it Directly.

            //Find the Editor Time BuildIn Object, then DeSerialize it.
            foreach (BaseSerializableData data in allBuildInDatas)
            {
                if (data.uniqueId == uniqueID)
                {
                    data.DeSerializeData(reader);
                }
            }
        }

        public void LoadTargetCharacter()
        {
            targetLEObject.SetActive(true);
        }

        //=============================================================
        //  SceneInitCommandExecuter Delegate........
        //-------------------------------------------------------------
        public void Default_Init(object a)
        {
            GameUIPr.Instance.CloseLoadingPanel();
            LoadTargetCharacter();
        }

        public void LoadSave(object path)
        {
            Default_Init(null);
            currentLevel = SceneManager.GetActiveScene().buildIndex;
            LoadSave_by_FilePath((string)path);
        }

        //=============================================================

        public void SaveGame(int index)
        {
            string path = Path.Combine(Application.persistentDataPath, "Save" + index.ToString() + ".vSave");
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(SceneManager.GetActiveScene().name);

                BaseSerializableData[] allData = FindObjectsOfType<BaseSerializableData>();
                foreach (BaseSerializableData data in allData)
                {
                    data.SerializeData(writer);
                }
            }
        }

        public void PauseResumeEvent(bool pause)
        {
            Time.timeScale = pause ? 0 : 1;
            if (Adapter_Pause != null)
                Adapter_Pause.Invoke(pause);
        }

        public void NextLevel()
        {
            currentLevel++;
            SceneInitCommandQueue.Enqueue(new SceneInitCommandInfo(Default_Init, true));
            SceneManager.LoadScene(currentLevel, LoadSceneMode.Single);
        }

        public Transform GetTargetLETransform()
        {
            if (targetLEObject != null)
                return targetLEObject.transform;
            else
                return null;
        }

        //========================================
        //Multi-Thread Test
        //========================================
        //This function will be called in a seperate Thread
        public List<int> Generate_Big_Data()
        {
            List<int> data = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                data.Add(i);
                Debug.Log(i);
            }
            return data;
        }

        //This function will be called inside the Unity Main Thread
        public void Print_Big_Data(object data)
        {
            data = (List<int>)data;
            Debug.Log(((List<int>)data).Count);
        }

        //========================================

    }
}



