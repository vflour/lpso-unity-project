using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.PetSelect;
using UnityEngine.UI;

namespace Game.UI
{
    public class PSArrowList : ArrowList
    {
        public GameObject resetButtonPrefab;
        public PetSelectHandler petSelectHandler;
        public int totalCount;
        private List<ArrowListButton> currentArrows = new List<ArrowListButton>();

        public void Initialize(int totalCount)
        {
            petSelectHandler = GameObject.Find("PetSelectHandler").GetComponent<PetSelectHandler>();
            this.totalCount = totalCount;
            SetPage(0); // setting default page to 0
            
            if(totalCount > maxCount) // continue only if the maxcount is smaller than the actual button count
            {
                AddButton(resetButtonPrefab,-1,2); // instantiates the topmost button
                AddButton(arrowButtonPrefab,-1,1); // instatiates the previous button

                pageCount = (int)Mathf.Clamp(Mathf.Ceil((float)totalCount/(float)maxCount),0,4); // page count is clamped to the maximum amount of buttons
                
                for(int count = 0; count<pageCount; count++) // cycle through all pages
                {
                    ArrowListButton b = AddButton(pageButtonPrefab,count,0); 
                    currentArrows.Add(b);
                }
                AddButton(arrowButtonPrefab,1,1); // instatiates the next button
                AddButton(resetButtonPrefab,1,2); // instantiates the bottommost button
                SetPageButtonsSize();
            }
        }

        public override void ChangePage(int adder)
        {
            petSelectHandler.IncrementSlotGroup(adder);
            base.ChangePage(adder);
        }

        public override void SetPage(int page)
        {
            sortArrows(page);
            petSelectHandler.SetSlotGroup(page);
            currentPage = page;


        }

        // reorders the arrow lists so that they correspond to the correct index
        private void sortArrows(int page)
        {
            // scale basically sets the index order of each page
            int scale = page >= 2? page - 2 : 5 + page;
            foreach(ArrowListButton button in currentArrows)
            {
                scale++;
                if(scale>=pageCount) scale = 0;
                button.value = scale;
                button.GetComponent<Text>().text = scale.ToString();
            }
        }
    }
}