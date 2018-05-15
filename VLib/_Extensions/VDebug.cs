using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public static class VDebug
    {
        static int tickCounter = 0;
        static bool _lock = false;
        static string log = "";

        public static void Reset()
        {
            tickCounter = 0;
            _lock = false;
        }

        public static void StartRecord()
        {
            if (!_lock)
            {
                _lock = true;
                log += "VDebug： 开始 \n";
            }
        }

        public static void Tick(string logInfor)
        {
            tickCounter++;
            string content = "Tick Index : <" + tickCounter.ToString() + "> : " + logInfor + "\n";
            log += content;
        }

        public static void EndRecord(EndType endType)
        {
            if (log != null && log.Length > 0)
            {
                string[] lines = log.Split(System.Environment.NewLine.ToCharArray());
                if (endType == EndType.PrintLines)
                {
                    foreach (string s in lines)
                    {
                        Debug.Log(s);
                    }
                }
                else
                {
                    //......
                } 
            }
            log = "";
            tickCounter = 0;
            _lock = false;
        }

        public enum EndType
        {
            PrintLines,
            WriteToFile
        }
    }
}
