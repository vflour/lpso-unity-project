using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    public class CharacterHandler : MonoBehaviour, IHandler // Relays commands to a specific character + basic character management
    {
[Header("Pet Sprite Generation")]
        public PetDatabase petDatabase;
        public PetSpriteGenerator petGen;

 #region IHandler properties + methods       
        private GameSystem _system;
        public GameSystem system{get;   set;}
        public void Activate()
        {
            petGen = new PetSpriteGenerator(petDatabase);
        }
        private bool _display;
        public bool Display
        {
            get { return _display;}
            set { _display = value; }
        }
#endregion

#region Character instantiation
        public List<Character> loadedCharacters = new List<Character>();

        public Character AddCharacter(CharacterData characterData)
        {
            // instantiate object and add character component
            GameObject charObject = petGen.GenerateCrAPSprite(characterData.palette,characterData.species,characterData.speciesSubtype,characterData.parts);
            Character character = charObject.AddComponent<Character>();
            
            loadedCharacters.Add(character);
            // sets the character fields
            character.characterHandler = this;

            return character;
        }
        public void RemoveCharacter(int character)
        {
            GameObject.Destroy(loadedCharacters[character].gameObject);
            loadedCharacters.RemoveAt(character);
        }
#endregion
        public void MoveCharacter(int character, Vector3 position)
        {
            loadedCharacters[character].MoveTo(position);
        }
        public void AnimateCharacter(int character, string animationName)
        {
            loadedCharacters[character].PlayAnimation(animationName);
        }

        public void AddClothes(int character, int slot)
        {
            loadedCharacters[character].AddClothes(slot);
        }
        public void RemoveClothes(int character, int slot)
        {
            loadedCharacters[character].RemoveClothes(slot);
        }

        public Color[] GetPalette(int species, int[] paletteData)
        {
            return petGen.GetPalette(species,paletteData);

        }
        public int GetIndex(Character character)
        {
            return loadedCharacters.IndexOf(character);
        }      
        public void SetPalette(int character, Color[] palette)
        {
            GameObject characterObject = loadedCharacters[character].gameObject;
            petGen.SetPalette(characterObject,palette);
    
       }
    }
}

