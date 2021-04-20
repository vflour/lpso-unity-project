using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.PetSelect;
using UnityEngine.UI;

namespace Game.UI
{
    public class PSArrowList : ArrowList
    {
[Header("Pet Select Values")] 
        public int maxPageButtons;
        private List<ArrowListButton> activePageButtons = new List<ArrowListButton>();
        
        // Instantiates a set amount of pages
        protected override void InstantiateNPages()
        {
            // add n amount of pages, where n = maxPageCount
            for (int pageIndex = 0; pageIndex < maxPageButtons; pageIndex++)
            {
                InstantiateButton(pageIndex, ArrowListButtonType.PageButton);
            }
        }
        // Override the button instantiation to include active Page buttons
        protected override ArrowListButton InstantiateButton(int buttonValue, ArrowListButtonType buttonType)
        {
            ArrowListButton button = base.InstantiateButton(buttonValue,buttonType);
            if(buttonType == ArrowListButtonType.PageButton) activePageButtons.Add(button);
            return button;
        }
        // Override the CurrentPage property to reset the page values
        public override int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = (int)Mathf.Repeat(value,maxPageCount);
                ResetPages();
            }
        }
        // Set page numbers to be accurate to the current page
        private void ResetPages()
        {
            int pageButtonRange = maxPageButtons / 2;
            for (int pageNumber = -pageButtonRange; pageNumber <= pageButtonRange; pageNumber++)
                activePageButtons[pageNumber+pageButtonRange].Value = (int)Mathf.Repeat(CurrentPage + pageNumber,maxPageCount-1);
        }
    }
}