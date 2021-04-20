using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.CrAP;
using UnityEngine.UI;

namespace  Game.UI.CRaP
{
    public class PaletteModifierButton : MonoBehaviour
    {
        public int Index;
        public int Value;

        public GameObject dotSprite;
        private bool _selected;

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                dotSprite.SetActive(_selected);
            }
        }

        public CrAPSect3Screen sect3Screen;

        public void SetPalette()
        {
            sect3Screen.SetPaletteSection(Index, Value);
        }
    }
}