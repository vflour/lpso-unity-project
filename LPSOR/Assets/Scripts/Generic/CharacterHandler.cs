using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    using Inventory;
    public class CharacterHandler : MonoBehaviour, IHandler // Relays commands to a specific character + basic character management
    {
[Header("Pet Sprite Generation")]
        public PetDatabase petDatabase;
        public ItemDatabase itemDatabase;
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
        public Dictionary<string,Character> loadedCharacters = new Dictionary<string,Character>();
        // why do i need to get the index every time i can just use the object itself sfkjdkfjlksdjflsjflkdf
        public Character AddCharacter(string characterId, CharacterData characterData)
        {
            // instantiate object and add character component
            GameObject charObject = petGen.GenerateCrAPSprite(characterData.palette,characterData.species,characterData.speciesSubtype,characterData.parts);
            Character character = charObject.AddComponent<Character>();
            character.characterHandler = this;
            character.data = characterData;
            
            // Dress the character
            if (characterData.wearing != null) // Check if character has clothes first
            {
                foreach (ItemData wearing in characterData.wearing)
                    if(wearing!=null)
                        character.AddClothes(wearing.id);
            }

            loadedCharacters.Add(characterId,character);
            return character;
        }
        public void RemoveCharacter(string characterName)
        {
            GameObject.Destroy(loadedCharacters[characterName].gameObject);
            loadedCharacters.Remove(characterName);
        }

        public bool HasCharacter(string characterName)
        {
            return loadedCharacters.ContainsKey(characterName);
        }
#endregion
        public void MoveCharacter(string characterName, Vector3 position)
        {
            loadedCharacters[characterName].MoveTo(position);
        }
        public void AnimateCharacter(string characterName, string animationName)
        {
            loadedCharacters[characterName].PlayAnimation(animationName);
        }

        public void AddClothes(string characterName, int slot)
        {
            loadedCharacters[characterName].AddClothes(slot);
        }
        public void RemoveClothes(string characterName, int slot)
        {
            loadedCharacters[characterName].RemoveClothes(slot);
        }

        public PaletteColor[] GetPalette(int species, int[] paletteData)
        {
            return petGen.GetPalette(species,paletteData);

        }
        public void SetPalette(string characterName, PaletteColor[] palette)
        {
            GameObject characterObject = loadedCharacters[characterName].gameObject;
            petGen.SetPalette(characterObject,palette);
    
       }
    }
}

