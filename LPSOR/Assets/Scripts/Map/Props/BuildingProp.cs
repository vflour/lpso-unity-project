using System;
using System.Collections;
using UnityEngine;

namespace Game.Map
{
    public class BuildingProp : Prop
    {
        private Animator propAnimator;
        public int mapId; // map teleport id
        private void Start()
        {
            propAnimator = GetComponent<Animator>();
        }

        // Buildings will often have a door opening animation
        protected override float PlayPropAnimation(Player player)
        {
            propAnimator.Play("DoorOpen");
            return propAnimator.GetCurrentAnimatorClipInfo(0).Length;
        }
        
        // Add a closing door animation 
        protected override IEnumerator CharacterArrivedAtProp(Player player)
        {
            yield return StartCoroutine(base.CharacterArrivedAtProp(player));
            propAnimator.Play("DoorClose");
            yield return new WaitForSeconds(propAnimator.GetCurrentAnimatorClipInfo(0).Length);
            mapHandler.system.Emit("loadScene", mapId);
        }
    }
}