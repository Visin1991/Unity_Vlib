using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class HealthBar : MonoBehaviour
    {

        Image healthImage;

        private void Start()
        {
            healthImage = GetComponent<Image>();
        }

        public void ValueChange(float current, float max)
        {
            if (healthImage != null)
            {
                healthImage.fillAmount = current / max;
            }
        }
    }
}
