using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public enum ArrowListButtonType{PageButton,PreviousArrow,NextArrow,MinButton,MaxButton}
    
    public class ArrowList : MonoBehaviour
    { 
[Header("Arrow List Buttons")]        
        public ArrowListButtonType[] buttonOrder;
        public GameObject[] buttonSprites;
        public List<ArrowListButton> activeButtons = new List<ArrowListButton>();
[Header("Page Data")]         
        public int itemsPerPage;
        public ArrowListPage pageObject;
        public bool showListIfOne;
        
#region Initialization
        public void Initialize(GameObject[] itemList)
        {
            int itemCount = itemList.Length;
            maxPageCount = itemCount / itemsPerPage + 1;
            
            // Init buttons if theres more than one pages
            if (maxPageCount > 1 || showListIfOne)
            {
                InitializeButtons();
                CurrentPage = 0;
            }

            InitializePageObject(itemList);
        }
        // Initialize a page object if there is one
        private void InitializePageObject(GameObject[] itemList)
        {
            pageObject = GetComponent<ArrowListPage>();
            if (pageObject != null)
                pageObject.Initialize(itemList,itemsPerPage);
        }
        // initialize the buttons
        private void InitializeButtons()
        {
            // loop through the button order
            for(int i = 0; i<buttonOrder.Length;i++)
            {
                ArrowListButtonType buttonType = buttonOrder[i];
                switch (buttonType)
                {
                    case ArrowListButtonType.PreviousArrow:
                        InstantiateButton(-1, buttonType);
                        break;
                    case ArrowListButtonType.PageButton:
                        InstantiateNPages();
                        break;
                    case ArrowListButtonType.NextArrow:
                        InstantiateButton(1, buttonType);
                        break;
                    case ArrowListButtonType.MinButton:
                        InstantiateButton(0, buttonType);
                        break;
                    case ArrowListButtonType.MaxButton:
                        InstantiateButton(maxPageCount-1, buttonType);
                        break;
                }
            }
        }
        // Instantiates a set amount of pages
        protected virtual void InstantiateNPages()
        {
            // add n amount of pages, where n = maxPageCount
            for (int pageIndex = 0; pageIndex < maxPageCount; pageIndex++)
                InstantiateButton(pageIndex, ArrowListButtonType.PageButton);
            
        }
        // Adding a button to the active button list
        protected virtual ArrowListButton InstantiateButton(int buttonValue, ArrowListButtonType buttonType)
        {
            // Instantiate button based on buttontype
            GameObject button = GameObject.Instantiate(buttonSprites[(int) buttonType], transform);
            // initialize arrowlistbutton component
            ArrowListButton buttonComponent = button.GetComponent<ArrowListButton>();
            buttonComponent.Value = buttonValue;
            buttonComponent.buttonType = buttonType;
            buttonComponent.arrowList = this;
            
            // add to active buttons
            activeButtons.Add(buttonComponent);
            button.transform.SetAsLastSibling();
            return buttonComponent;
        }
#endregion
#region Page Count
        // Page count properties
        protected int maxPageCount;
        protected int _currentPage;

        public virtual int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = (int)Mathf.Repeat(value,maxPageCount-1);
                SetActiveButton();
                SetVisiblePage();
            }
        }
        
        // Adding or subtracting from the currentPage. Used for button functionality
        public void AddToPage(int increment)
        {
            CurrentPage += increment;
        }
#endregion
#region Page Modification       
        // Loop through each active button and set their size based on the currentPage
        public void SetActiveButton()
        {
            // Set button active based on whether or not it's the correct page and value
            foreach (ArrowListButton button in activeButtons)
                button.Active = (button.buttonType == ArrowListButtonType.PageButton && button.Value == CurrentPage);
        }
        // If there's a pageObject, tell it to set the current page
        public void SetVisiblePage()
        {
            if (pageObject != null)
                pageObject.CurrentPage = CurrentPage;
            
        }
 #endregion        
    }
}

