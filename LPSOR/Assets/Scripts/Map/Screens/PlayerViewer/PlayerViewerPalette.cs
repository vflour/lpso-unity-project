using UnityEngine;

namespace Game.Map
{
    [CreateAssetMenu(fileName = "PlayerViewerPalette", menuName = "Databases/PVPalette", order = 0)]
    public class PlayerViewerPalette : ScriptableObject
    {
        public Sprite[] petButtonSprites; // these are used for the 12 buttons
        public Sprite emptyBoxSprite; // used for unfilled boxes
        public Sprite fieldSprite; // the field used for displaying text
        public Color textColor; // the text color itself
        public IconDictionary mainBoxes;// main box sprites
        public PlayerViewerScreen.ViewerButtons[] buttons;
    }
}