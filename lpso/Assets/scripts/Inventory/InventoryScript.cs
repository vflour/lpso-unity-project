using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    public Item[] WearingData = new Item[8];
    public List<List<Stack<Item>>> InventoryData = new List<List<Stack<Item>>>();

    public Transform InventoryGui;
    public Transform WearingGui;

    public GameObject gui;
    public int MaxSlotsPerPage = 12;
    public int page = 0;
    public ItemDatabase database;
    public Transform player;


    public void Summon(List<List<InventorySlot_s>> saves, Transform character)
    {
        player = character;
        database.InitDatabase();
        int i = 0;
        foreach(List<InventorySlot_s> v in saves)
        {
            i = 0;
            InventoryData.Add(new List<Stack<Item>>());
            foreach(InventorySlot_s vv in v)
            {
                
                Stack<Item> itemstack = new Stack<Item>();
                InventoryData[i].Add(itemstack);
                for (int ii = 0; ii < vv.stack; ii++)
                {
                    Item itemb = database.ItemBase[vv.id];
                    itemb.Wearing = false;
                    itemstack.Push(itemb);
                }
                
            }
            i++;
        }
        WearingData = new Item[8];

    }

    public void AddItem(Item item,int Count)
    {

        foreach (List<Stack<Item>> Page in InventoryData)
        {
            foreach (Stack<Item> itemstack in Page)
            {
                int StackSize = itemstack.Peek().StackSize;
                int StackCount = itemstack.Count;
                if (Count > 0 && StackCount < StackSize && item.name == itemstack.Peek().name) 
                {
                    int StackRemain = StackSize - StackCount;

                    for (int i = 0; i < StackRemain; i++)
                    {
                        itemstack.Push(item);
                        Count--;
                    }
                }
            }
        }

        if(Count > 0) { 
            foreach (List<Stack<Item>> Page in InventoryData)
            {
                if(Page.Count < MaxSlotsPerPage)
                {
                    Stack<Item> itemstack = new Stack<Item>();
                    Page.Add(itemstack);
                    for (int i = 0; i < Count; i++)
                    {
                        itemstack.Push(item);
                    }
                    break;
                }
            }
        }
        ResetInventorySlots();
        SetInventorySlots();
    }

    public void RemoveEntry(int Page, Stack<Item> ItemStack)
    {
        if(Page != -1)
        {
            int Chosen = 0;
            foreach (Stack<Item> Slot in InventoryData[Page])
            {
                if (Slot.Count > 0)
                {
                    if (Slot.Peek().name == Slot.Peek().name && Slot.Count < 1) break;
                }
                else break;
                Chosen++;
            }
            InventoryData[Page].RemoveAt(Chosen);
        }
        else
        {
            int Chosen = 0;
            foreach (Item Slot in WearingData)
            {
                if (ItemStack.Count > 0)
                {
                    if (Slot.Name == ItemStack.Peek().Name) break;
                    Chosen++;
                }
                else break;
            }
            WearingData[Chosen] = null;
        }

        ResetInventorySlots();
        SetInventorySlots();
    }

    public void SetWearing(int WearingSlot, Item item)
    {
        WearingData[WearingSlot] = item;

        ResetInventorySlots();
        SetInventorySlots();
    }

    public void SummonWearing(List<InventorySlot_s> saves, Transform character)
    {
        player = character;
        int i = 0;
        foreach (InventorySlot_s v in saves)
        {
            Item itemb = database.ItemBase[v.id];
            WearingData[i] = itemb;
            i++;
        }
    }

    void ResetInventorySlots()
    {
        for( int i = 0; i < 12; i++)
        {
            
            SlotScript slot = InventoryGui.GetChild(i).GetComponent<SlotScript>();
            slot.transform.GetChild(0).gameObject.SetActive(false);
            slot.Wipe();
        }
        for (int i = 0; i < 8; i++)
        {
            SlotScript slot = WearingGui.GetChild(i).GetComponent<SlotScript>();
            slot.Wipe();
        }
    }

    public void OpenInventory()
    {
        if (!gui.activeSelf)
        {
            gui.SetActive(true);
            ResetInventorySlots();
            SetInventorySlots();
        }
        else CloseInventory();

    }

    public void CloseInventory()
    {
        gui.SetActive(false);
    }

    private bool WearingOpen = false;
    public Transform WearingArrow;

    public void OpenWearing()
    {
        WearingOpen = !WearingOpen;

        int ArrowSize = WearingOpen == true ? -1 : 1;
        WearingArrow.localScale = new Vector3(ArrowSize, 1, 1);
        gui.GetComponentInChildren<Animator>().SetBool("Open", WearingOpen);
        gui.GetComponentInChildren<Animator>().SetTrigger("OpenBox");
    }

    void SetInventorySlots()
    {
        int i = 0;
        foreach (Stack<Item> v in InventoryData[page]) 
        {
            SlotScript slot = InventoryGui.GetChild(i).GetComponent<SlotScript>();
            slot.SetItem(v);
            slot.Inven_Ref = this;
            slot.character = player;
            slot.Page = page;
            i++;
        }
        i = 0;
       foreach (Item v in WearingData)
        {
            if (v != null)
            {
                SlotScript slot = WearingGui.GetChild(i).GetComponent<SlotScript>();
                Stack<Item> itmstack = new Stack<Item>();
                itmstack.Push(v);

                slot.Page = -1;
                slot.SetItem(itmstack);
                slot.Inven_Ref = this;
                slot.character = player;

                i++;
            }
        }

    }


}
