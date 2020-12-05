using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.CRaP
{
    public class CrAPScreen : GameScreen
    {
        private int _currentSection;
        public int CurrentSection
        {
            get { return _currentSection; }
            set
            {
                _currentSection = value;
                setCurrentSection();
            }
        }

[Header("Sprites + Icons")]
        public GameObject[] sizeableIcons;
        public Sprite activeSprite;
        public Sprite inactiveSprite;

        // set the active icon's sprite + size
        private void setCurrentSection()
        {
            // reset each numbered icon's size
            for (int index = 0; index<3; index++)
                setIcon(index, new Vector3(1,1,1), inactiveSprite);
            // set the size + icon of the numbered icon
            setIcon(CurrentSection, new Vector3(1.25f,1.25f,1.25f), activeSprite);
        }
        
        // set the icon's size + sprite
        private void setIcon(int index, Vector3 scale, Sprite sprite)
        {
            GameObject icon = sizeableIcons[index];
            icon.transform.localScale = scale;
            icon.GetComponent<Image>().sprite = sprite;            
        }
    }
}