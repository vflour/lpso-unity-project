using System;
using System.Collections;
using System.Collections.Generic;
using Game.Map;
using Game.UI;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.Inventory
{
    public class ItemButton : MonoBehaviour
    {
        [Header("Item Data")]
        public Item _item;
        public ItemData _data;

        public ItemData data
        {
            get => _data;
            set
            {
                SetCounter(value.count);
                _data = value;
            }
        }

        public Item item
        {
            get => _item;
            set
            {
                image.sprite = value.icon;
                DialogData dialogData = new DialogData();
                dialogData.icon = value.icon;
                dialogData.text = new[] {value.Name, value.description};
                dialogData.type = UI.DialogType.Item;
                prompt.data = dialogData;
                _item = value;
            }
        }
        
        public bool wearing;
        
        [Header("References")] 
        public InventoryScreen inventoryScreen;
        private CanvasGroup canvasGroup;
        
        [Header("GUI Elements")] 
        public Image image;
        public Text counter;
        
        public DialogPrompt prompt;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnClick()
        {
            // check modes
            if (inventoryScreen.mode == InventoryScreen.Mode.Dragging) return;
            if (inventoryScreen.mode == InventoryScreen.Mode.Deleting)
            {
                if (!wearing)
                {
                    inventoryScreen.inventoryHandler.RemoveItem(data);
                    SetCounter(data.count);
                }
                return;
            }
                
            // use item
            if (item is IUseable)
                (item as IUseable).Use(this);
            SetCounter(data.count);
        }
        
        public void OnDragStarted()
        {
            if (inventoryScreen.mode != InventoryScreen.Mode.Normal) return;
            canvasGroup.blocksRaycasts = false;
            inventoryScreen.draggingButton = this;
            
            inventoryScreen.mode = InventoryScreen.Mode.Dragging;
            inventoryScreen.draggingClone = Instantiate(this.gameObject, inventoryScreen.transform).GetComponent<ItemButton>();
            inventoryScreen.draggingClone.name = this.name + "_dragging";
        }

        public void OnDragEnded()
        {
            if (inventoryScreen.mode != InventoryScreen.Mode.Dragging) return;
            canvasGroup.blocksRaycasts = true;
            inventoryScreen.RemoveDraggingButtons();
            
            // raycast under mouse and send to item to process
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray,Mathf.Infinity);
            item.RaycastDrag(hit, this);
            SetCounter(data.count);
        }
        public void OnDrag()
        {
            Vector2 canvasPosition = Input.mousePosition;
            Transform canvas = inventoryScreen.gameUI.uiSpace;
            RectTransformUtility.ScreenPointToLocalPointInRectangle( canvas as RectTransform, Input.mousePosition, Camera.main, out canvasPosition);
            inventoryScreen.draggingClone.transform.position = canvas.transform.TransformPoint(canvasPosition);
        }

        public void OnDrop()
        {
            if (wearing)
                inventoryScreen.DropIntoWearing();
            else
                inventoryScreen.DropIntoInventory(inventoryScreen.loadedPage.IndexOf(this));
        }

        private void SetCounter(int value)
        {
            bool showCounter = value > 1;
            if (showCounter)
                counter.text = value.ToString();
            counter.gameObject.SetActive(showCounter);
        }
    }  
}

