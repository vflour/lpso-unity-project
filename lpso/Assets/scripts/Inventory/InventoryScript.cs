using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    public List<List<Stack<Item>>> InventoryData = new List<List<Stack<Item>>>();
    public Transform InventoryGui;
    public GameObject gui;
    public int page = 0;
    public ItemDatabase database;
    Transform player;


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
                    itemstack.Push(itemb);
                }
                
            }
            i++;
        }
        
    }

    void ResetInventorySlots()
    {
        for( int i = 0; i < 12; i++)
        {
            SlotScript slot = InventoryGui.GetChild(i).GetComponent<SlotScript>();
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

    void SetInventorySlots()
    {
        int i = 0;
        foreach (Stack<Item> v in InventoryData[page])
        {
            SlotScript slot = InventoryGui.GetChild(i).GetComponent<SlotScript>();
            slot.SetItem(v);
            slot.character = player;
            i++;
        }
    }


}
