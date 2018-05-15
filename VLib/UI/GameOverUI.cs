using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class GameOverUI : MonoBehaviour
    {

        //Make a singleTon for playerManager RegestPlayer();
        static private GameOverUI instance = null;
        void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
            }
            instance = this;
        }
        public static GameOverUI GetInstance()
        {
            return instance;
        }
        //----------------------------------------------

        public Image fadePlane;
        public GameObject gameOverUI;

        public void RegisterPlayer(GameObject p)
        {
            // p.GetComponent<PlayerProperties>().event_Non_OnDeath += OnGameOver;
            //p.GetComponent<PlayerProperties>().event_Non_OnDeath += OnGameOver2;
        }
        void OnGameOver2(object sender, System.EventArgs e)
        {
            Debug.Log("OnGameOver2 called");
        }
        void OnGameOver(object sender, System.EventArgs e)
        {
            StartCoroutine(Fade(Color.clear, Color.black, 1));
            gameOverUI.SetActive(true);
        }

        IEnumerator Fade(Color from, Color to, float time)
        {
            float speed = 1 / time;
            float percent = 0.0f;
            while (percent < 1)
            {
                percent += Time.deltaTime * speed;
                fadePlane.color = Color.Lerp(from, to, percent);
                yield return null;
            }
        }

        public void StartNewGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level1");
        }
    }
}
