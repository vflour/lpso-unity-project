using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum CharacterState{Idle,Walking}
    public class Character : MonoBehaviour
    {
        // references to the character handler.
        public CharacterHandler characterHandler;
        public int index;

        // character states are meant to see if a character is idle, moving, etc..
        private CharacterState _characterState; 
        public CharacterState characterState
        {
            get{return _characterState;}
            set{_characterState = value;}
        }

        // basic methods
        private void Start()
        {
            DetermineSpriteOrder();
            characterState = CharacterState.Idle;
        }

        public void Remove()
        {
            characterHandler.RemoveCharacter(index);
        }

#region Character movement
        private Vector3 targetPosition;
        private Vector3 velocity = Vector3.zero;
        private void LateUpdate()
        {
            // moves the character if they're walking
            if(characterState == CharacterState.Walking)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, Time.deltaTime*5);
                SortSprites();
            }
                
        }

        // sets the target position and sets their state to walking until they reach the desired point
        public IEnumerator MoveTo(Vector3 pos)
        {
            targetPosition = pos;
            characterState = CharacterState.Walking;
            yield return new WaitUntil(()=> (transform.position-targetPosition).magnitude < 0.01);
            characterState = CharacterState.Idle;
        }
#endregion

#region Sprite sort order
        // sorting the sprite order of the character
        private List<int> originalSpriteOrder;
        private List<SpriteRenderer> sprites;

        public void SortSprites()
        {
            for(int i = 0; i < sprites.Count; i++)
            {
                sprites[i].sortingOrder = (int)(originalSpriteOrder[i]+(200*transform.position.y));
            }
        }
        public void DetermineSpriteOrder()
        {
            originalSpriteOrder = new List<int>();
            sprites = new List<SpriteRenderer>();

            foreach(SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            {
                originalSpriteOrder.Add(sprite.sortingOrder);
                sprites.Add(sprite);
            }
        } 
#endregion

#region Character Animations 
        public void SetStateAnimation()
        {
            // sets the mask depending on the current state
        }

        public void AngleCharacter()
        {

        }
        public void TriggerAnimation(string trigger)
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