////////////////////////////////////////////////
// Copyright: Evil-Mind 2016
// --------------------------
// ButtonImageOnClick
// --------------------------
// Example class that switches a sprite depending 
// if the user is clicking on the element or not.
////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HUDGUIMedieval
{
    [RequireComponent(typeof(Image))]
    public class ButtonImageOnClick : MonoBehaviour
    {
        public Sprite SelectedSprite;
        private Sprite _normalSprite;
        private Image _image;

        void Awake()
        {
            _image = GetComponent<Image>();
            _normalSprite = _image.sprite;
        }

        public void OnPointerDown()
        {
            _image.sprite = SelectedSprite;
        }

        public void OnPointerUp()
        {
            _image.sprite = _normalSprite;
        }
    }
}