using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogType : ScriptableObject
{
}


public class ItemDialog: DialogType, IDialog
{
    string Desc;
    string Name;
    Sprite Icon;

    public ItemDialog(string Desc, string Name, Sprite Icon)
    {
        this.Desc = Desc;
        this.Name = Name;
        this.Icon = Icon;
    }

    public GameObject NewDialog(GameObject dialogbox, Transform canvas)
    {
        GameObject dialogv = Instantiate(dialogbox, canvas);
        dialogv.transform.Find("Name").GetComponent<Text>().text = Name;
        dialogv.transform.Find("Desc").GetComponent<Text>().text = Desc;
        dialogv.transform.Find("Icon").GetComponent<Image>().sprite = Icon;

        return dialogv;
    }

}

public class PropDialog : DialogType, IDialog
{
    string msg;

    public PropDialog(string msg)
    {
        this.msg = msg;
    }

    public GameObject NewDialog(GameObject dialogbox, Transform canvas)
    {
        GameObject dialogv = Instantiate(dialogbox, canvas);
        dialogv.GetComponentInChildren<Text>().text = msg;

        return dialogv;
    }
}

public interface IDialog
{
    GameObject NewDialog(GameObject dialogbox, Transform canvas);
}
