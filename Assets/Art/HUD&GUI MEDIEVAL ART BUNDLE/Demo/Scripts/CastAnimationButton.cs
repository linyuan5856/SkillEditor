////////////////////////////////////////////////
// Copyright: Evil-Mind 2016
// --------------------------
// CastAnimationButton
// --------------------------
// Example class that disables a button and enables it again after some time has passed.
// While it is disabled, it creates an animation modifying the fillAmount of an image to tell the user
// how much time remains for the button to be enabled again.
////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HUDGUIMedieval
{
    [RequireComponent(typeof(Button))]
	public class CastAnimationButton : MonoBehaviour 
	{
        /// <summary>
        /// Time for the button to be enabled again
        /// </summary>
		public float CastTime = 0f;
		
		private float _currentAnimTime = 0f;
		private bool _animating = false;
		private Image _fillingImage = null;
        private Button _button = null;
		
		void Awake()
		{
            _button = GetComponent<Button>();
			_fillingImage = transform.GetChild(0).GetComponent<Image>();
		}
		
		public void StartAnimation()
		{
			if(_animating)
				return;

            StartCoroutine(AnimationPlaying());
		}
		
		IEnumerator AnimationPlaying()
		{
            // Set initial values
			_animating = true;
			_currentAnimTime = CastTime;
            _button.interactable = false;


			while(_currentAnimTime > 0f)
			{
                // Update the fillAmount
				_currentAnimTime -= Time.deltaTime;
                _fillingImage.fillAmount = (CastTime - _currentAnimTime) / CastTime;
				yield return null;
			}

            // Enable the button again
            _button.interactable = true;
			_animating = false;
		}
	}
}