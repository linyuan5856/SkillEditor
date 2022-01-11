////////////////////////////////////////////////
// Copyright: Evil-Mind 2016
// --------------------------
// ButtonDisabler
// --------------------------
// Example class that switch the sprite of a button from enabled to disabled and viceversa.
// Use the method "SetEnabled()" to switch from code.
////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HUDGUIMedieval
{
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(Button))]
	public class ButtonDisabler : MonoBehaviour 
	{
        private bool _isEnabled;
        public bool IsEnabled { get { return _isEnabled; } set { SetEnabled(value); } }

        [Header("Images")]
		public Sprite EnabledImage;
		public Sprite DisabledImage;
		
		private Image _image;
		private Button _button;
		
		void Awake()
		{
			_image = GetComponent<Image>();
			_button = GetComponent<Button>();
		}

        public void SetEnabled(bool value)
        {
            _isEnabled = value;
            _button.interactable = _isEnabled;
            _image.sprite = (_isEnabled ? EnabledImage : DisabledImage);
        }

#if UNITY_EDITOR
        [ContextMenu ("Enable Button")]
        public void EnableButton()
        {
            SetEnabled(true);
        }

        [ContextMenu ("Disable Button")]
        public void DisableButton()
        {
            SetEnabled(false);
        }
#endif
	}
}