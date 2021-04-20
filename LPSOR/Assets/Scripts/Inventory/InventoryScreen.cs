using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Inventory
{ 
    using UI;
    public class InventoryScreen : GameScreen
    {    
        public enum Mode{Normal,Dragging,Deleting}
        
        [Header("Item Data")]
        public List<ItemButton> loadedPage;
        public ItemButton[] loadedWearing = new ItemButton[8];
        
        [Header("Page Data")]
        public int maxItemsPerPage = 12;
        private string _filterType = "All";

        public string filterType
        {
            get => _filterType;
            set
            {
                _filterType = value;
                Create();
            }
        }

        private int _currentPage;
        
        [Header("Screen Data")]
        public Mode mode;
        private bool wearingOpen;
        
        [Header("Prefabs and Transforms")] 
        public GameObject itemButtonPrefab;
        public Transform[] itemButtonSpace;
        public Transform[] wearingButtonSpace;

        [Header("GUI Instances")] 
        public Dropdown categoryDropdown;
        public Text currentPageText;
        public Text kibbleCoinsText;
        public ItemButton draggingButton;
        public ItemButton draggingClone;
        
        [Header("References")] 
        public Animator inventoryAnimator;
        public InventoryHandler inventoryHandler;
        
        public int currentPage
        {
            get => _currentPage;
            set
            {
                int maxPages = inventoryHandler.inventoryData.Count / (maxItemsPerPage+1);
                _currentPage = (int)Mathf.Repeat(value,maxPages+1);
                SetCurrentPageText(value);
                Create();
            }
        }

        #region Initialization
        // On start, generate the "All" page
        private void Start()
        {
            inventoryHandler = gameUI.system.GetHandler<InventoryHandler>();
            inventoryHandler.inventoryScreen = this;
            inventoryAnimator = GetComponent<Animator>();
            Create();
            CreateWearing();
        }
        #endregion

        #region Page Generation

        // Generates the currentPage
        public void Create()
        {
            // clear page
            Clear();
            loadedPage = new List<ItemButton>();
            // get page data and instantiate buttons
            int count = inventoryHandler.inventoryData.Count;
            int startIndex = currentPage * maxItemsPerPage;
            
            for (int i = startIndex; i < startIndex + maxItemsPerPage; i++) // first, insert each button in the loaded pages
            {
                if (i < count) // if the button is within range, set data
                {
                    ItemData itemStorageData = inventoryHandler.inventoryData[i]; // get item information
                    Item itemData = inventoryHandler.itemDatabase.data[itemStorageData.id];
                    
                    if (itemData.itemType.ToString() == filterType || filterType == "All") // check if it respects the chosen filter
                    {
                        ItemButton itemButton = Instantiate(itemButtonPrefab).GetComponent<ItemButton>();
                        itemButton.inventoryScreen = this;
                        itemButton.data = itemStorageData;
                        itemButton.item = itemData;
                        loadedPage.Add(itemButton);
                    }
                }
            }
            // then, set the parent of each itembutton 
            for (int i = 0; i < loadedPage.Count; i++)
            {
                loadedPage[i].transform.SetParent(itemButtonSpace[i]);
                loadedPage[i].transform.localPosition = Vector3.zero;
                loadedPage[i].transform.localScale = Vector3.one;
            }
            SetCurrentPageText(currentPage);
            gameUI.system.GetHandler<MouseHandler>().SetDialogPrompts(transform);
        }

        // Clears the page
        public void Clear()
        {
            for (int i = 0; i < loadedPage.Count; i++)
            {
                Destroy(loadedPage[i].gameObject);
                loadedPage.RemoveAt(i);
            }
                
            
        }
        #endregion

        
        #region Wearing Clothing
        
        // Loads the clothing buttons
        public void CreateWearing()
        {
            ClearWearing();
            for (int i = 0; i < inventoryHandler.wearingData.Length; i++)
            {
                if (inventoryHandler.wearingData[i] != null)
                {
                    ItemButton itemButton = Instantiate(itemButtonPrefab,wearingButtonSpace[i]).GetComponent<ItemButton>();
                    itemButton.inventoryScreen = this;
                    itemButton.data = inventoryHandler.wearingData[i];
                    itemButton.item = inventoryHandler.itemDatabase.data[itemButton.data.id];
                    itemButton.wearing = true;
                    loadedWearing[i] = itemButton;
                }
            }
            gameUI.system.GetHandler<MouseHandler>().SetDialogPrompts(transform);
        }

        // Clears clothing buttons
        public void ClearWearing()
        {
            for(int i = 0; i < loadedWearing.Length; i++)
                if (loadedWearing[i] != null)
                {
                    Destroy(loadedWearing[i].gameObject);
                    loadedWearing[i] = null;
                }
                    
            loadedWearing = new ItemButton[8];
        }
        #endregion
        #region Button Functions
        public void NextPage()
        {
            currentPage++;
        }
        
        public void PreviousPage()
        {
            currentPage++;
        }

        public void ToggleWearingBox()
        {
            wearingOpen = !wearingOpen;
            inventoryAnimator.SetBool("Open",wearingOpen);
            inventoryAnimator.SetTrigger("OpenBox");
        }

        public void SetCategory()
        {
            filterType = categoryDropdown.options[categoryDropdown.value].text;
            Create();
        }

        public void SetCurrentPageText(int value)
        {
            int maxPages = inventoryHandler.inventoryData.Count / maxItemsPerPage;
            currentPageText.text = $"{value+1}/{maxPages+1}";
        }
        
        public void Exit()
        {
            gameUI.RemoveScreen("Inventory");
        }

        public void DeleteMode()
        {
            // delete mode can only be accessed from normal mode
            if (mode == Mode.Normal)
                mode = Mode.Deleting;
            else if (mode == Mode.Deleting)
                mode = Mode.Normal;
        }

        public void OnDestroy()
        {
            inventoryHandler.RemoveScreen();
        }
        #endregion
        
        #region
        public void DropIntoInventory(int index)
        {
            if (draggingButton == null) return;
            if (index >= loadedPage.Count-1)
                DropInNew();
            else
                DropInBetween(inventoryHandler.inventoryData.IndexOf(loadedPage[index].data));
            
            if (draggingButton.item is Wearable)
                if (draggingButton.wearing)
                    (draggingButton.item as Wearable).UnEquip(draggingButton);
            
            RemoveDraggingButtons();
        }

        public void DropInNew()
        {
            inventoryHandler.AddItem(draggingButton.data);
        }

        public void DropInBetween(int index)
        {
            inventoryHandler.InsertBetween(index,draggingButton.data);
        }

        public void DropIntoWearing()
        {
            if (draggingButton == null) return;
            if (!(draggingButton.item is Wearable)) return;
            (draggingButton.item as Wearable).Equip(draggingButton);
            inventoryHandler.RemoveItemData(draggingButton.data);
            RemoveDraggingButtons();
            Create();
            CreateWearing();
        }

        public void RemoveDraggingButtons()
        {
            if (draggingClone!=null)
            {
                Destroy(draggingClone.gameObject);
                draggingClone = null;
                draggingButton = null;
                mode = Mode.Normal;
            }
        }
        #endregion
    } 
}

