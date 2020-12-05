using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.CrAP;
using UnityEngine.UI;

namespace Game.UI.CRaP
{
    public class PartModifierButton : MonoBehaviour
    {
[Header("Text Fields")] 
        public Text numberField;
        // The part value itself
        private int _partValue;
        public int PartValue
        {
            get { return _partValue;}
            set
            {
                _partValue = (int)Mathf.Repeat(value,maxPartValue);
                numberField.text = $"{_partValue+1}/{maxPartValue}";
                sect3Screen.SetPart(partType,_partValue);
            }
        }
        
        //If its active/selected
        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (_selected)
                {
                    transform.localScale = new Vector3(1.25f,1.25f,1.25f);
                    GetComponent<Image>().sprite = activeSprite;
                }
                else
                {
                    transform.localScale = new Vector3(1,1,1);
                    GetComponent<Image>().sprite = inactiveSprite;
                }
                numberField.gameObject.SetActive(_selected);
            }
        }
[Header("References + Part indexes")]       
        public int maxPartValue;
        public int partType;
        public CrAPSect3Screen sect3Screen;
        public Sprite activeSprite;
        public Sprite inactiveSprite;
        
        // Clicking on the button itself
        public void ButtonClick()
        {
            IncrementIndex();
        }

        // Incrementing/Decrementing the index
        public void IncrementIndex()
        {
            PartValue++;
        }
        public void DecrementIndex()
        {
            PartValue--;
        }
    }
}