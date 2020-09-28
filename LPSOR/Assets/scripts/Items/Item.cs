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

    [SerializeField]
    private bool wearable;


    // public Component ItemType;

    public string Name;
    public string description;
    public bool Wearing = false;

    public Sprite Icon
    {
        get { return icon; }
    }

   public int StackSize
    {
        get { return stacksize; }
    }

    public bool Wearable
    {
        get { return wearable; }
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


