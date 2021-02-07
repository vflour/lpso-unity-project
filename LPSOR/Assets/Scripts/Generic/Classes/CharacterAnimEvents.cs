using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CharacterAnimEvents : MonoBehaviour
    {
        // handles all the events by having specific dedicated functions to each event trigger
        // mainly used for sprite swapping
        private Character character;
        
        public void Start()
        {
            character = GetComponent<Character>();
        }
        

    }
}
