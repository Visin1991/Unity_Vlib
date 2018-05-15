using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace V
{

    public static class WeiInputHelper
    {

        public const double MAX_TIME_TO_CLICK = 0.5;
        public const double MIN_TIME_TO_CLOCK = 0.05;
        private static TimeSpan maxDuration = TimeSpan.FromSeconds(MAX_TIME_TO_CLICK);
        private static TimeSpan minDuration = TimeSpan.FromSeconds(MIN_TIME_TO_CLOCK);

        private static System.Diagnostics.Stopwatch timer;
        private static bool ClickedOnce = false;

        public static bool DoubleClick()
        {
            if (!ClickedOnce)
            {
                timer = System.Diagnostics.Stopwatch.StartNew();
                ClickedOnce = true;
            }
            else if (ClickedOnce)
            {
                if (timer.Elapsed > minDuration && timer.Elapsed < maxDuration)
                {
                    ClickedOnce = false;
                    return true;
                }
                else if (timer.Elapsed > maxDuration)
                {
                    ClickedOnce = false;
                    return false;
                }
            }
            return true;
        }
    }

    [System.Serializable]
    public class WeiClickManager
    {
        //properties
        public float MaxTimeToClick { get { return _maxTimeToClick; } set { _maxTimeToClick = value; } }
        public float MinTimeToClick { get { return _minTimeToClick; } set { _minTimeToClick = value; } }

        //property variables
        private float _maxTimeToClick = 0.60f;
        private float _minTimeToClick = 0.05f;

        //private variables to keep track
        private float _minCurrentTime = 0;
        private float _maxCurrentTime = 0;

        public bool DoubleClick()
        {
            //When we open the Main Manue, this function will always return false. bacause time scale is equal to 0 now.
            if (Time.time >= _minCurrentTime && Time.time <= _maxCurrentTime)
            {
                return true;
            }
            _minCurrentTime = Time.time + MinTimeToClick;
            _maxCurrentTime = Time.time + MaxTimeToClick;
            return false;
        }
    }
}
