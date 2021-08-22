using System;
using System.Collections;
using System.Collections.Generic;
using Game.Map;
using Game.UI;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game
{
    using Inventory;
    public enum CharacterState{Idle, Walking, Running, Action}

    public class Character : MonoBehaviour
    {
        // references to the character handler.
        public CharacterHandler characterHandler;
        public CharacterData data;
        // character states are meant to see if a character is idle, moving, etc..
        private CharacterState _characterState;

        public CharacterState characterState
        {
            get { return _characterState; }
            set
            {
                _characterState = value;
                if (value!= CharacterState.Action)
                    PlayAnimation(value.ToString());
            }
        }

        // angle determining the character's direction, updates anim when set
        private int _angle;

        public int angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                PlayAnimation(characterState.ToString());
            }
        }

        // basic methods
        private void Start()
        {
            animator = GetComponent<Animator>();
            sortGroup = GetComponent<SortingGroup>(); // get sorting group
            characterState = CharacterState.Idle;
            
        }

        public void Remove()
        {
            characterHandler.RemoveCharacter(this.name);
        }

        #region Character movement
        [Header("Character Position")]
        // general vector3 positioning
        public Vector3 targetPosition;
        
        // tile positioning
        public Vector2Int tilePosition = new Vector2Int();
        public TileNode targetTile;
        // Late Update function for moving the character to a desired point
        private void LateUpdate()
        {
            // moves the character if they're walking
            if (characterState == CharacterState.Walking || characterState == CharacterState.Running)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 3);
                SortSprites();
            }
        }
        // sets the target position and sets their state to walking until they reach the desired point
        public void MoveTo(Vector3 pos, CharacterState characterState)
        {
            targetPosition = pos;
            this.characterState = characterState;
        }

        public void MoveTo(Vector3 pos)
        {
            MoveTo(pos, CharacterState.Walking);
        }

        public IEnumerator MoveToTile(List<TileNode> path, Vector3 tileOffset, Action callback)
        {
            CharacterState walkType = path.Count < 3? CharacterState.Walking: CharacterState.Running;
            TileNode targetTile = path[path.Count-1];
            this.targetTile = targetTile;
            foreach (TileNode node in path)
            {
                if (targetTile != this.targetTile)
                    yield break;
                tilePosition = new Vector2Int(node.x,node.y);
                Vector3 targetPosition = node.transform.position + tileOffset;
                
                // Set angle and move
                angle = CalculateAngle(transform.position, targetPosition);
                MoveTo(targetPosition,walkType);
                yield return new WaitUntil(() => transform.position==targetPosition);
            }
            characterState = CharacterState.Idle;
            callback();
        }
        
        public int CalculateAngle(Vector3 p1, Vector3 p2)
        {
            // get the unit of p2-p1
            Vector3 unitDirection = (p2 - p1).normalized;
            // get the angle no through the y axis of the unit direction
            int direction = Mathf.RoundToInt((4 * Mathf.Asin(unitDirection.y)) / Mathf.PI) + 2;
            // negative direction is set whether or not the player is pointing left or right
            // if unit.x < 0, it's left and vice-versa. negative direction means the player is pointing right
            direction = (int) -Mathf.Sign(unitDirection.x) * direction;
            return direction;
        }
        #endregion
        #region Sprite sort order
        private SortingGroup sortGroup;
        public void SortSprites()
        {
            sortGroup.sortingOrder = (int) -transform.position.y*10;
        }

        #endregion
        
        #region Character Animations

        private Animator animator;
        public void PlayAnimation(string animName)
        {
            animator.SetFloat("Angle",Mathf.Abs(angle));
            animator.Play(animName);
        }
        
        // triggers an event 
        public void AnimationTrigger(string trigger)
        {
            // triggers an animation
        }
        #endregion

        #region Character Clothing
        private List<GameObject>[] clothingSprites = new List<GameObject>[8];
        
        public void AddClothes(int id)
        {
            if (characterHandler.itemDatabase.data[id].itemType != ItemType.Clothes) return;
            Wearable wearable = characterHandler.itemDatabase.data[id] as Wearable;

            // Cycle through each sprite in the parts
            foreach(CustomSprite sprite in wearable.parts)
            {
                Transform angle = transform.Find(sprite.Angle); // Finds the corresponding angle
                Transform boneParent = PetSpriteGenerator.FindParentByName(angle, sprite.PartName);// Finds the bone
                GameObject spritePart = Instantiate(sprite.Sprite,boneParent); // Instantiates the sprite
                spritePart.name =  $"{sprite.Sprite.name}_Clothing_{id}";
            }
        }
        public void RemoveClothes(int id)
        {
            if (characterHandler.itemDatabase.data[id].itemType != ItemType.Clothes) return;
            Wearable wearable = characterHandler.itemDatabase.data[id] as Wearable;

            // Cycle through each sprite in the parts and find its equivalent
            foreach(CustomSprite sprite in wearable.parts)
            {
                Transform angle = transform.Find(sprite.Angle); // Finds the corresponding angle
                Transform boneParent = PetSpriteGenerator.FindParentByName(angle, sprite.PartName);// Finds the bone
                
                // Finds a sprite with the specified name and id
                Transform find = boneParent.Find($"{sprite.Sprite.name}_Clothing_{id}");
                if(find!=null)
                    Destroy(find.gameObject);
            }
        }
        #endregion

        #region Descriptor
        public void AddDialogDescriptor(string[] text, Sprite icon)
        {
            // Add the dialog descriptor prompt
            DialogPrompt prompt = gameObject.AddComponent<DialogPrompt>();
            prompt.data = new DialogData()
            {
                type=UI.DialogType.Information,
                text=text,
                icon=icon,
            };
            prompt.mouseHandler = characterHandler.system.GetHandler<MouseHandler>();
        }
        #endregion
        
    }
}