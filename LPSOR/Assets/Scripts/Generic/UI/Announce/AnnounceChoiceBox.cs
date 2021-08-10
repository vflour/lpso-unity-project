using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    public class AnnounceChoiceBox : AnnounceBox
    {

        private Action denyCallback;
        private Action acceptCallback;
        
        protected override IEnumerator SetBoxTimer()
        {
            yield break;
        }

        public void SetCallbacks(Action denyCallback, Action acceptCallback)
        {
            this.denyCallback = denyCallback;
            this.acceptCallback = acceptCallback;
        }

        public virtual void AcceptButton()
        {
            acceptCallback();
            Remove();
        }
        
        public virtual void DenyButton()
        {
            denyCallback();
            Remove();
        }


    }
}
