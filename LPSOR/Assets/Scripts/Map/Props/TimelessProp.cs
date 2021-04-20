using System.Collections;
using UnityEngine;

namespace Game.Map
{
    // Timeless props are essentially props that don't wait for an animation to finish
    // However that means the user isn't occupied and they can move freely to exit
    public class TimelessProp : StructureProp
    {
        // This method fires once the player has arrived at the prop's start location
        protected override IEnumerator CharacterArrivedAtProp(Player player)
        {
            occupant = player;
            // init character
            player.session.characterState = CharacterState.Action;
            player.session.transform.position = transform.position;

            // play animation and get animation time
            float propAnimationTime = PlayPropAnimation(player);
            float characterAnimationTime = PlayCharacterAnimation(player);
            
            // wait until the player is no longer in action state
            yield return new WaitUntil(()=> player.session.characterState != CharacterState.Action);
            
            // set the character back to normal
            player.session.tilePosition = endLocation;
            
            occupant = null;
        }
    }
}