using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private int stacksize;

    [SerializeField]
    private SlotScript slot;


    // public Component ItemType;

    public string Name;
    public string description;


    public Sprite Icon
    {
        get { return icon; }
    }

   public int StackSize
    {
        get { return stacksize; }
    }


    protected SlotScript Slot
    {
        get
        {
            return slot;
        }
        set
        {
            slot = value;
        }
    }


}


