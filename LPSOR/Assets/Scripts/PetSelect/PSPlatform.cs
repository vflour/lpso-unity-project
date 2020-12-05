using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PetSelect
{
    public class PSPlatform : MonoBehaviour
    {
        public PetSelectHandler petSelectHandler;
        public int slot;
        public void OnMouseDown()
        {
            petSelectHandler.SelectSlot(slot);
        }


    }
}

