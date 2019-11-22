using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotScript : MonoBehaviour, IPointerClickHandler
{
    public Stack<Item> item = new Stack<Item>();
    public Sprite defaulticon;
    private bool empty;
    public Transform character;
    public GameObject dialogbox;
    public ItemDialog dialogt;
    //private Sprite Icon;

    public void SetItem(Stack<Item> citem)
    {
        if (citem.Count > 0)
        {
            item = citem;
            transform.GetComponent<Image>().sprite = item.Peek().Icon;
            SetCounter();
            dialogt = new ItemDialog(item.Peek().description, item.Peek().Name, item.Peek().Icon);
        }
    
    }

    void Hover(bool val)
    {
        character.GetComponent<CursorSet>().SetDialogVis(val, (dialogt as DialogType), dialogbox);
    }

    public void AddItem(Item citem)
    {
        item.Push(citem);
        SetCounter();
    }

    public void UseItem()
    {
        if (item.Peek() is IUseable)
        {
            (item.Peek() as IUseable).Use(character);
            RemoveItem();
            SetCounter();
        }
    }

    public void RemoveItem()
    {
        item.Pop();
        if (IsEmpty) transform.GetComponent<Image>().sprite = defaulticon;
    }

    void SetCounter()
    {
        Text txt = transform.GetChild(0).GetComponent<Text>();

        if (item.Count > 1)
        {
            txt.text = item.Count.ToString();
            txt.gameObject.SetActive(true);
        }
        else txt.gameObject.SetActive(false);
    }

    public void Wipe()
    {
        item = new Stack<Item>();
        transform.GetComponent<Image>().sprite = defaulticon;
    }

    public bool IsEmpty
    {
        get
        {
           
            if (item.Count < 1) return true;
            else return false;
        }
    }

    public void OnPointerClick(PointerEventData eventdata)
    {
        if(eventdata.button == PointerEventData.InputButton.Left)
        {
            if (!IsEmpty)
            {
                UseItem();
                Hover(false);
            }
                
        }
    }

    public void EnterH()
    {
        if (!IsEmpty) Hover(true);
    }

    public void ExistH()
    {
        if (!IsEmpty) Hover(false);
    }


}
