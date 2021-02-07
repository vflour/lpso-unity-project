using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game
{
    public enum CharacterState{Idle, Walking, Running}

    public class Character : MonoBehaviour
    {
        // references to the character handler.
        public CharacterHandler characterHandler;

        // character states are meant to see if a character is idle, moving, etc..
        private CharacterState _characterState;

        public CharacterState characterState
        {
            get { return _characterState; }
            set
            {
                _characterState = value;
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
            sortGroup = GetComponent<SortingGroup>(); // get sorting group
            characterState = CharacterState.Idle;
        }

        public void Remove()
        {
            characterHandler.RemoveCharacter(characterHandler.GetIndex(this));
        }

        #region Character movement
        [Header("Character Position")]
        // general vector3 positioning
        public Vector3 targetPosition;
        private Vector3 velocity = Vector3.zero;
        // tile positioning
        public Vector2Int tilePosition = new Vector2Int();
        // Late Update function for moving the character to a desired point
        private void LateUpdate()
        {
            // moves the character if they're walking
            if (characterState == CharacterState.Walking)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, Time.deltaTime * 5);
                SortSprites();
            }
        }
        // sets the target position and sets their state to walking until they reach the desired point
        public void MoveTo(Vector3 pos)
        {
            targetPosition = pos;
            characterState = CharacterState.Walking;
        }
        #endregion
        #region Sprite sort order
        private SortingGroup sortGroup;
        public void SortSprites()
        {
            sortGroup.sortingOrder = (int) transform.position.y;
        }

        #endregion
        
        #region Character Animations

        private Animator animator;
        public void PlayAnimation(string animName)
        {
            animator.SetFloat("Angle",angle);
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
        
        public void AddClothes(int slot)
        {
            
        }
        public void RemoveClothes(int slot)
        {

        }
        #endregion
    }
}