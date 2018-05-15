using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace V
{
    public class VisinButton : MonoBehaviour
    {

        public void AAA_PressSettingButton()
        {

            SYS_UI_Event_PressSettingButton uiEvent = new SYS_UI_Event_PressSettingButton();
            uiEvent.Init();
            GameUIPr.Instance.MailBox_SYS_UI_Event(uiEvent);
        }

        public void AAA_StartGame()
        {
            SYS_UI_Event_StartGame uiEvent = new SYS_UI_Event_StartGame();
            uiEvent.Init();

            GameUIPr.Instance.MailBox_SYS_UI_Event(uiEvent);
        }

        public void AAA_PrintSomeInfo()
        {
            SYS_UI_Event_PrintSomeInfo uiEvent = new SYS_UI_Event_PrintSomeInfo();
            uiEvent.Init();

            uiEvent.mousePosition = Input.mousePosition;
            uiEvent.whatYouWantToSay = "Hi, this is Wei. You are such a dick";

            GameUIPr.Instance.MailBox_SYS_UI_Event(uiEvent);
        }
    }
}

