using UnityEngine;

namespace Game.Inventory
{
    using Map;
    [CreateAssetMenu(fileName = "Wearable", menuName = "Items/Wearable", order = 1)]
    public class Wearable : Item, IUseable
    {
        public CustomSprite[] parts;
        public WearableType type;
        
        public void Use(ItemButton itemButton)
        {
            if (!itemButton.wearing)
            {
                Equip(itemButton);
                RemoveFromInventory(itemButton);
            }
            else
            {
                AddToInventory(itemButton);
                UnEquip(itemButton);
            }
        }

        public void Equip(ItemButton itemButton)
        {
            InventoryHandler inventoryHandler = itemButton.inventoryScreen.inventoryHandler;
            inventoryHandler.AddWearing((int)type,itemButton.data);
            //tell the user to wear the sprite
            inventoryHandler.system.GetHandler<UserHandler>().localPlayer.DressCharacter(id);
            //send to server that you're wearing the part
            string data = "{"+$"\"wSlot\":{(int)type},\"iSlot\":{inventoryHandler.inventoryData.IndexOf(itemButton.data).ToString()}"+"}";
            inventoryHandler.system.ServerDataSend("dress",data);
        }
        public void UnEquip(ItemButton itemButton)
        {
            InventoryHandler inventoryHandler = itemButton.inventoryScreen.inventoryHandler;
            inventoryHandler.RemoveWearing((int)type);
            inventoryHandler.system.GetHandler<UserHandler>().localPlayer.UndressCharacter(id);
            inventoryHandler.system.ServerDataSend("undress",((int)type).ToString());
        }

        private void RemoveFromInventory(ItemButton itemButton)
        {
            itemButton.inventoryScreen.inventoryHandler.RemoveItemData(itemButton.data);
        }
        
        private void AddToInventory(ItemButton itemButton)
        {
            itemButton.inventoryScreen.inventoryHandler.AddItem(itemButton.data);
        }
        
        public override void RaycastDrag(RaycastHit2D hit, ItemButton itemButton)
        {
            // Check if the wearable hit a character
            Character character = hit.transform.GetComponent<Character>();
            if (itemButton.wearing) return;
            if (character != null)
            {
                Character localCharacter = itemButton.inventoryScreen.gameUI.system.GetHandler<UserHandler>().localPlayer.session;
                if (character == localCharacter )
                {
                    Equip(itemButton);
                    RemoveFromInventory(itemButton);
                }
            }
        }
    }
    public enum WearableType{Hat,Face,Top,Bottom,Gloves,Bracelet,Neck,Shoes}
}