using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Game.Inventory;
using Game.Map;
using Game.UI;
using UnityEngine;
using UnityEngine.Events;

public class SpeechPathMenu : MonoBehaviour
{
    [Header("References")]
    public SpeechPathData speechData; // reference to the speech data object
    public GameObject bubbleMenuPrefab;

    [Header("Variables")] 
    public Transform tailRelativeTo;
    public Action<string> speechPathFound;
    private List<BubbleMenu> activeMenus = new List<BubbleMenu>();
    private int highest = 0;
    private Regex regex;
    
    public void OpenMenu()
    {
        regex = new Regex("\\[\\.\\.\\.\\]");
        BubbleMenu firstMenu = Draw(speechData.contains,0);
        if (firstMenu)
            firstMenu.MoveTail(tailRelativeTo.position.y);
    }
    
    // generates a bubblemenu based on the contents of a speechpath
    public BubbleMenu Draw(SpeechPath path,int order)
    {
        // if there's too many menus then leave
        if (activeMenus.Count > order) return null;
        
        BubbleMenu menu = Instantiate(bubbleMenuPrefab, transform).GetComponent<BubbleMenu>();
        activeMenus.Add(menu);
        // create the items array for the menu
        BubbleMenuItem[] items = new BubbleMenuItem[path.contains.Length];
        menu.options = items;
        //set highest
        if (path.contains.Length > highest) highest = path.contains.Length;
        //transform.GetComponent<RectTransform>().sizeDelta = new Vector2(175,-11 * (highest-1));
        menu.transform.position = transform.position;
        menu.transform.localPosition += new Vector3(150*order,5 * (highest-1));
            
        // add each item
        for(int i = 0; i < path.contains.Length; i++)
        {
            int index = i;
            SpeechPath subPath = path.contains[i];
            BubbleMenuItem itemData = new BubbleMenuItem() {itemData=subPath.data, itemEvent = new UnityEvent()};
            itemData.itemEvent.AddListener(() => SelectPath(subPath,order+1,path,index));
            items[i] = itemData;
        }
        menu.Draw();
        return menu;
    }
    // clears all bubbles
    public void Clear(int start)
    {
        for (int i = start; i < activeMenus.Count; i++)
            Destroy(activeMenus[i].gameObject);
        activeMenus.RemoveRange(start,activeMenus.Count-start);
    }
    // selects the specified path
    private void SelectPath(SpeechPath path,int nextOrder,SpeechPath parentPath,int pathIndex)
    {
        if (path.contains.Length > 0)
        {
            if(activeMenus.Count > nextOrder)
                Clear(nextOrder);
            BubbleMenu nextMenu = Draw(path, nextOrder);
            if (nextMenu)
                nextMenu.MoveTail(activeMenus[nextOrder-1].loadedButtons[pathIndex].transform.position.y);
        }
        else
        {
            string sentence = path.data;
            Clear(0);
            if (regex.IsMatch(parentPath.data)) // replace with regex for sentences having [...]
                sentence = regex.Replace(parentPath.data, sentence);
            highest = 0;
            speechPathFound(sentence);
        }
    }
}
