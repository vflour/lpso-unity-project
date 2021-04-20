using UnityEngine;
using System.Collections;
namespace Game.Map
{
    public class Prop : MonoBehaviour
    {
        public int id;
        public Vector2Int startLocation;
        public Vector2Int endLocation;
        public bool mirrorCharacter;
        public Player occupant;
        public SpriteRenderer selection;

        public bool isOccupied
        {
            get => occupant != null;
        }

        public MapHandler mapHandler;
    
        // Send the character to the start tile
        public void Interact(Player player)
        {
            if (isOccupied) return;
            player.MoveCharacter(startLocation.x,startLocation.y, () => StartCoroutine(CharacterArrivedAtProp(player)));
        }

        // This method fires once the player has arrived at the prop's start location
        protected virtual IEnumerator CharacterArrivedAtProp(Player player)
        {
            occupant = player;
            
            // init character
            player.session.characterState = CharacterState.Action;
            player.session.transform.position = transform.position;
            SetUserOccupied(player, true);
            // play animation and get animation time
            SetMirror(player);
            float propAnimationTime = PlayPropAnimation(player);
            float characterAnimationTime = PlayCharacterAnimation(player);
            
            // wait for the character animation to end
            yield return new WaitForSeconds(characterAnimationTime);
            
            // set the character back to normal
            player.session.tilePosition = endLocation;
            player.session.characterState = CharacterState.Idle;
            SetUserOccupied(player, false);
            occupant = null;
        }

        protected void SetUserOccupied(Player player, bool value)
        {
            UserHandler user = mapHandler.system.GetHandler<UserHandler>();
            if (player == user.localPlayer)
                user.occupied = value;
        }

        // Plays a prop animation and returns the time it takes for the prop animation to play
        protected virtual float PlayPropAnimation(Player player)
        {
            return 0; 
        }

        // Plays a character animation and returns the time it takes for the animation to play
        protected virtual float PlayCharacterAnimation(Player player)
        {
            return 0;
        }

        // Set the character's "mirrored" sprite if applicable
        public void SetMirror(Player player)
        {
            if (mirrorCharacter)
                player.session.angle = -Mathf.Abs(player.session.angle);

        }
        
        // Apply an outline when the mouse hovers over the object
        protected virtual void OnMouseEnter()
        {
            UserHandler user = mapHandler.system.GetHandler<UserHandler>();
            if (user.occupied) return;
            selection.material = mapHandler.propOutline;
        }

        protected virtual void OnMouseExit()
        {
            UserHandler user = mapHandler.system.GetHandler<UserHandler>();
            if (user.occupied) return;
            selection.material = mapHandler.propNormal;
        }
        
    }
}
