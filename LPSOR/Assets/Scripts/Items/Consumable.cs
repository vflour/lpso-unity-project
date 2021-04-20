using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    using Map;
    [CreateAssetMenu(fileName = "Consumable", menuName = "Items/Consumable", order = 1)]
    public class Consumable : Item, IUseable
    {
        public int FeedAmount;

        public void Use(ItemButton itemButton)
        {
            itemButton.inventoryScreen.inventoryHandler.RemoveItem(itemButton.data);
        }
        
        public override void RaycastDrag(RaycastHit2D hit, ItemButton itemButton)
        {
            // Check if the wearable hit a character
            Character character = hit.transform.GetComponent<Character>();
            if (character != null)
            {
                Character localCharacter = itemButton.inventoryScreen.gameUI.system.GetHandler<UserHandler>().localPlayer.session;
                if (character == localCharacter)
                    Use(itemButton);
            }
        }
    } 
}

