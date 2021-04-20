using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Inventory
{
    public class InventoryHandler : MonoBehaviour, IHandler
    {
        [Header("Item Data")]
        public ItemDatabase itemDatabase;
        public List<ItemData> inventoryData;
        public ItemData[] wearingData;
        
        [Header("References")] 
        public InventoryScreen inventoryScreen;
        
        #region Initialization
        public bool Display { get; set; }
        public GameSystem system { get; set; }
        public void Activate()
        {
            InitializeDatabase();
            InitializeInventories();
        }

        private void InitializeInventories()
        {
            // if inventory or wearing is null, then create a new list/array for them
            if (system.gameData.playerData.inventory == null)
                system.gameData.playerData.inventory = new List<ItemData>();
            if(system.gameData.sessionData.wearing==null)
                system.gameData.sessionData.wearing = new ItemData[8];
            
            // if any references are null, replace and add new ones
            if (system.gameData.sessionData.wearing.Length < 8)
            {
                wearingData = new ItemData[8];
                for (int i = 0; i < system.gameData.sessionData.wearing.Length; i++)
                    if (system.gameData.sessionData.wearing[i] != null)
                        wearingData[i] = system.gameData.sessionData.wearing[i];
                system.gameData.sessionData.wearing = wearingData;
            }  
          
            // set data
            inventoryData = system.gameData.playerData.inventory;
            wearingData = system.gameData.sessionData.wearing;
        }
        // sets the ids according to the order
        private void InitializeDatabase()
        {
            for (int i = 0; i < itemDatabase.data.Length; i++)
                itemDatabase.data[i].id = i;


                
        }
        #endregion
        #region Modifying the inventory
        
        // Gets an item through it's id
        public void AddItem(ItemData item)
        {
            string location;
            int index;
            if (inventoryData.Contains(item))
            {
                location = "Inventory";
                index = inventoryData.IndexOf(item);
                inventoryData.Remove(item);
            }
            else
            {
                location = "Wearing";
                index = 0;
                for (int i = 0; i<wearingData.Length;i++)
                    if(wearingData[i]!=null)
                        if (wearingData[i].id == item.id)
                            index = i;
            }
            
            inventoryData.Add(item);
            RefreshScreen();
            
            string data = "{"+$"\"location\":\"{location}\",\"index\":{index}"+"}";
            system.ServerDataSend("addItemData", data);
        }

        // Removes 1 item at a specific slot. First decreases the count then removes it if necessary
        public void RemoveItem(ItemData item)
        {
            if (inventoryData.Contains(item))
            {
                item.count--;
                if (item.count <= 0)
                {
                    RemoveItemData(item);
                    RefreshScreen();
                }
                else
                    system.ServerDataSend("removeItem", inventoryData.IndexOf(item).ToString());
            }

        }
        
        // Places an item in between slots
        public void InsertBetween(int index, ItemData item)
        {
            int oIndex = inventoryData.IndexOf(item);
            inventoryData.Remove(item);
            inventoryData.Insert(index, item);
            string data = "{"+$"\"nIndex\":{index},\"oIndex\":{oIndex}"+"}";

            system.ServerDataSend("insertItem",data);
            RefreshScreen();
        }
        
        // Removes an item's entry completely
        public void RemoveItemData(ItemData item)
        {
            if (inventoryData.Contains(item))
            {
                inventoryData.Remove(item);
                system.ServerDataSend("removeItemData",inventoryData.IndexOf(item).ToString());
                RefreshScreen();
            }
        }
        #endregion
        
        #region Wearing data
        // Adds a clothing to the data
        public void AddWearing(int slot, ItemData data)
        {
            if (wearingData[slot] != null)
            {
                ItemButton wearingButton = inventoryScreen.loadedWearing[slot];
                (wearingButton.item as Wearable).Use(wearingButton);
            }
            
            wearingData[slot] = data;
            RefreshScreen();
        }

        // Removes clothing from data
        public void RemoveWearing(int slot)
        {
            wearingData[slot] = null;
            RefreshScreen();
        }
        #endregion

        #region Refresh Inventory screen
        public void RefreshScreen()
        {
            if (inventoryScreen != null)
            {
                inventoryScreen.CreateWearing();
                inventoryScreen.Create();
            }    
        }

        public void RemoveScreen()
        {
            inventoryScreen = null;
        }
        #endregion
    }
}