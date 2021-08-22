using Game.UI;
using UnityEngine;

namespace Game.Map
{
    public class PlayerViewerALPage : ArrowListPage
    {
        public PlayerViewerScreen playerViewer;
        public override void Initialize(GameObject[] items, int itemsPerPage)
        {
            base.Initialize(items, itemsPerPage);
            UpdatePlayerViewer();
        }

        public override void SetPageVisible(int pageNumber, bool visible)
        {
            base.SetPageVisible(pageNumber, visible);
            UpdatePlayerViewer();
        }

        private void UpdatePlayerViewer()
        {
            int max = itemPages[CurrentPage].Count;
            playerViewer.SetEmptySlotButtons(max); 
        }
    }
}