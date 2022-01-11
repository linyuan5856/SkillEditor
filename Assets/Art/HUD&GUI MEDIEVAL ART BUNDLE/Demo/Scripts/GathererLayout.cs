////////////////////////////////////////////////
// Copyright: Evil-Mind 2016
// --------------------------
// FillBySlider
// --------------------------
// Example class that draws a specific number of sprites ("TotalElements") inside a layout and
// sets them enabled or disabled depending on another number ("CurrentQuantity") representing
// the quantity enabled.
//
// To modify this values by code, use the methods "SetTotalElements()" and "SetCurrentQuantity()".
////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace HUDGUIMedieval
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class GathererLayout : MonoBehaviour
    {
        [Header("Values")]
        public int TotalElements = 0;
        public int CurrentQuantity = 0;

        [Header("Images")]
        public Sprite EnabledSprite;
        public Sprite DisabledSprite;

        private HorizontalLayoutGroup _layout = null;
        private List<Image> _elements = null;

        // The width of the sprite, used to calculate the width of the Layout
        private float _imageWidth = 0f;

        void Awake()
        {
            _layout = GetComponent<HorizontalLayoutGroup>();
            _elements = new List<Image>();
        }

        void Start()
        {
            _layout.childForceExpandWidth = false;
            _layout.childForceExpandHeight = false;

            _imageWidth = EnabledSprite.texture.width;

            Recalculate();
        }

        /// <summary>
        /// Changes the total elements to draw
        /// </summary>
        /// <param name="newValue">The number of elements that will be drawn</param>
        public void SetTotalElements(int newValue)
        {
            TotalElements = newValue;
            Recalculate();
        }

        /// <summary>
        /// Changes the number of elements that are enabled
        /// </summary>
        /// <param name="newValue">New quantity of elements that are enabled</param>
        public void SetCurrentQuantity(int newValue)
        {
            CurrentQuantity = newValue;
            ReFill();
        }

        /// <summary>
        /// Changes the size of the layout and adds or deletes new elements, depending
        /// on the new value of "TotalElements". It also calls a "ReFill()" to recalulate the enabled sprites.
        /// </summary>
        void Recalculate()
        {
            // Make sure the value is not negative
            TotalElements = Mathf.Clamp(TotalElements, 0, int.MaxValue);

            // Create or destroy
            if (TotalElements > _elements.Count)
            {
                CreateNewElements();
            }
            else if (TotalElements < _elements.Count)
            {
                RemoveElements();
            }

            // Resize the layout
            ((RectTransform)_layout.transform).sizeDelta = new Vector2(TotalElements * (_imageWidth + _layout.spacing), 
                ((RectTransform)_layout.transform).sizeDelta.y);

            ReFill();
        }

        /// <summary>
        /// Creates new elements on the layout
        /// </summary>
        void CreateNewElements()
        {
            int elementsToCreate = TotalElements - _elements.Count;

            for (int i = 0; i < elementsToCreate; ++i)
            {
                GameObject go = new GameObject();
                go.transform.SetParent(transform, false);
                go.transform.name = "Element";
                _elements.Add(go.AddComponent<Image>());
            }
        }

        /// <summary>
        /// Removes elements from the layout
        /// </summary>
        void RemoveElements()
        {
            while (_elements.Count > TotalElements)
            {
                int index = _elements.Count - 1;
                GameObject go = _elements[index].gameObject;
                _elements.RemoveAt(_elements.Count - 1);
                Destroy(go);
            }
        }

        /// <summary>
        /// Updates the status of each elements
        /// </summary>
        void ReFill()
        {
            CurrentQuantity = Mathf.Clamp(CurrentQuantity, 0, TotalElements);

            for (int i = 0; i < _elements.Count; ++i)
            {
                _elements[i].sprite = (i + 1 <= CurrentQuantity ? EnabledSprite : DisabledSprite);
            }
        }
    }
}