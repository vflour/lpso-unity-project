using System.Collections;
using UnityEngine;

namespace Game.Map
{
    // Structure props are props but with special animations for the player and prop itself. Think of swings or bouncepads.
    public class StructureProp : Prop
    {
        private Animator propAnimator;
        // Props might have custom animation strings inside them
        public string propAnimation;
        public string characterAnimation;
        private void Start()
        {
            propAnimator = GetComponent<Animator>();
        }
        
        protected override float PlayPropAnimation(Player player)
        {
            if (propAnimation == "") return 0;
            
            propAnimator.Play(propAnimation);
            return propAnimator.GetComponent<Animation>().clip.length;
        }

        protected override float PlayCharacterAnimation(Player player)
        {
            if (characterAnimation == "") return 0;

            Animator characterAnimator = player.session.GetComponent<Animator>();
            characterAnimator.Play(characterAnimation);
            
            return characterAnimator.GetComponent<Animation>().clip.length;
        }
        
    }
}