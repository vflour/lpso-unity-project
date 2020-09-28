using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CrAP
{
    public class CrAPSystem : GameSystem
    {
 
#region Initialization

        // adds a couple of new recievers specific to this system
        protected override void DeclareRecievers()
        {
            base.DeclareRecievers();

            // connection recievers
            Recieve("speciesSection",(obj)=>
            {
                gameUI.RemoveAllScreens();
                gameUI.InstantiateScreen(1);
            }
            );
            Recieve("subSpeciesSection",(obj)=>
            {
                gameUI.RemoveAllScreens();
                gameUI.InstantiateScreen(2);
            }
            );
            Recieve("characterModSection",(obj)=>
            {
                gameUI.RemoveAllScreens();
                gameUI.InstantiateScreen(3);
            });
            Recieve("savePet",(obj)=>{});
        
        }
#endregion       
    }
}