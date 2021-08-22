using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.UI;
using UnityEngine.UI;


namespace Game
{
    using Inventory;
    public class CharacterHandler : MonoBehaviour, IHandler // Relays commands to a specific character + basic character management
    {
[Header("Pet Sprite Generation")]
        public PetDatabase petDatabase;
        public ItemDatabase itemDatabase;
        public PetSpriteGenerator petGen;

        [Header("Profile Generation")] 
        public Camera profileCamera;
        public RenderTexture profileRenderTexture;
        public int profileResolution = 256;
 #region IHandler properties + methods       
        private GameSystem _system;
        public GameSystem system{get;   set;}
        public void Activate()
        {
            petGen = new PetSpriteGenerator(petDatabase);
            profileRenderTexture = new RenderTexture(profileResolution,profileResolution,24);
            profileCamera.targetTexture = profileRenderTexture;
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
            return AddCharacter(characterId, characterData, true);
        }
        public Character AddCharacter(string characterId, CharacterData characterData, bool wearClothing)
        {
            // instantiate object and add character component
            GameObject charObject = petGen.GenerateCrAPSprite(characterData.palette,characterData.species,characterData.speciesSubtype,characterData.parts);
            Character character = charObject.AddComponent<Character>();
            character.characterHandler = this;
            character.data = characterData;
            character.name = characterId;
            
            // Dress the character
            if (characterData.wearing != null && wearClothing) // Check if character has clothes first
            {
                foreach (ItemData wearing in characterData.wearing)
                    if(wearing!=null)
                        character.AddClothes(wearing.id);
            }

            // Check if character has already been loaded
            if (loadedCharacters.Count != 0)
            {
                if (!loadedCharacters.ContainsKey(characterId))
                {
                    loadedCharacters.Add(characterId,character);
                }
            }else{
                loadedCharacters.Add(characterId,character);
            }
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
        public IEnumerator GenerateProfile(string characterId, CharacterData characterData, bool bodyless, Action<Texture> returnDelegate)
        {
            yield return new WaitForEndOfFrame();
            profileCamera.gameObject.SetActive(true);
            // generate the character
            Character character = AddCharacter(characterId+"_profile", characterData,false);
            
            if(bodyless) DisembodyCharacter(character.transform);
            character.transform.SetParent(profileCamera.transform);
            character.transform.localPosition = new Vector3(0,-0.8f,10);
            SetLayerRecursively(character.transform);
            
            yield return new WaitForEndOfFrame();
            
            // generate the texture
            Texture2D snapshot = new Texture2D(profileResolution,profileResolution,TextureFormat.ARGB32,false);
            profileCamera.Render();
            RenderTexture.active = profileCamera.targetTexture;
            snapshot.ReadPixels(new Rect(0,0,profileResolution,profileResolution),0,0);
            snapshot.LoadRawTextureData(snapshot.GetRawTextureData());
            snapshot.Apply();
            RemoveCharacter(characterId+"_profile");
            
            profileCamera.gameObject.SetActive(false);
            returnDelegate(snapshot);
        }
        
        // Since we're only getting rid of the body for the front view, it'll do only that.
        public void DisembodyCharacter(Transform character)
        {
            Transform frontView = character.Find("F").Find("Body_B");
            string[] partNames = {"Tail_B","LeftLeg1_B","LeftArm1_B","RightLeg1_B","RightArm1_B","Body_Group"};
            Transform[] parts = new Transform[partNames.Length];
            
            //dont ask
            for (int i = 1; i<partNames.Length;i++)
                parts[i]= frontView.Find(partNames[i]);
            
            foreach(Transform part in parts)
                if (part!=null)
                    Destroy(part.gameObject);
        }

        private void SetLayerRecursively(Transform character)
        {
            foreach (Transform child in character.GetComponentsInChildren<Transform>())
                child.gameObject.layer = 11;
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

