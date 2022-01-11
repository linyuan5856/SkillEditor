////////////////////////////////////////////////
// Copyright: Evil-Mind 2016
// --------------------------
// FillBySlider
// --------------------------
// Example class that sets a fill amount value depending on the slider value.
// To use this, add this component to the image. Then on the slider, add an 
// "OnValueChanged" action, call to the "SetValue()" method of this class
// and attach the slider itself.
////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HUDGUIMedieval
{
    [RequireComponent(typeof(Image))]
    public class FillBySlider : MonoBehaviour
    {
        private Image _image;

        void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetValue(Slider slider)
        {
            _image.fillAmount = slider.value;
        }
    }
}