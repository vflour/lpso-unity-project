using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    using UI;
    public abstract class Item : ScriptableObject
    {
        private int _id;
    
        [SerializeField]
        private Sprite _icon;

        [SerializeField]
        private int _stackSize;
    
        public string Name;
        public string description;
        public ItemType itemType;

        public int id
        {
            get => _id;
            set => _id = value;
        }

        public Sprite icon
        {
            get => _icon;
        }
        public int stackSize
        {
            get => _stackSize;
        }
        public virtual void RaycastDrag(RaycastHit2D hit, ItemButton itemButton)
        {
        
        }
    }
    public enum ItemType{Clothes,Food,Toys,Furniture}
}



