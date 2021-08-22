using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class ArrowListPage : MonoBehaviour
    {
        // Create an array of lists, where each list represents a page holding a set amount of items
        protected List<GameObject>[] itemPages;
        // Initializing the page objects
        public virtual void Initialize(GameObject[] items, int itemsPerPage)
        {
            int itemCount = items.Length;
            int pageCount = itemCount / itemsPerPage + 1;
            
            // exit code if theres only one page
           // if (pageCount == 1) return;
            
            // Allocate n amount of List arrays, where n=pageCount
            itemPages = new List<GameObject>[pageCount];
            for(int pageIndex = 0; pageIndex < pageCount; pageIndex++)
                    itemPages[pageIndex] = new List<GameObject>();
            
            // Assign an item to a page
            for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
            {
                int pageIndex = itemIndex / itemsPerPage;
                // add the item to the page list
                itemPages[pageIndex].Add(items[itemIndex]);
            }
            CurrentPage = 0;
        }
        // Property to getting/setting the current visible page
        private int _currentPage;
        public int CurrentPage
        {
            get {return _currentPage; }
            set
            {
                _currentPage = value;
                SetAllPagesVisible(false);
                SetPageVisible(_currentPage,true);
            }
        }
        // Setting the page visibility
        public void SetAllPagesVisible(bool visible)
        {
            for(int pageNumber = 0; pageNumber < itemPages.Length; pageNumber++)
                SetPageVisible(pageNumber,visible);
        }
        public virtual void SetPageVisible(int pageNumber, bool visible)
        {
            foreach(GameObject item in itemPages[pageNumber])
                item.SetActive(visible);
        }
    }
}
