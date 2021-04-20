using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class AnnounceBox : GameScreen
    {
        public Text message;
        public Image icon;
        
        private Animator animator;
        private float boxTime = 30; 
        // On start, get the animator
        void Start()
        {
            animator = GetComponent<Animator>();
        }
        
        protected virtual IEnumerator SetBoxTimer()
        {
            yield return new WaitForSeconds(boxTime);
            yield return PlayExitAnimation();
            Remove();
        }

        protected virtual IEnumerator PlayExitAnimation()
        {
            animator.SetTrigger("Exit");
            yield return new WaitForSeconds(1);
        }
        protected virtual void Remove()
        {
            gameUI.RemoveScreen(name);
        }
    }
    public enum AnnounceBoxType{Normal, Choice}
    public enum AnnounceBoxIcon{Buddy, Map, Adventure}
}
