using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BubbleMenu : MonoBehaviour
{
    [Header("Menu Options")]
    public BubbleMenuItem[] options;

    [Header("Menu Template")] 
    public int viewportMax;
    public Transform template;
    public Transform tail;
    private float tailOffset;
    
    [Header("Buttons")]
    public Button upArrow;
    public Button downArrow;

    private Transform loadedMenu; // current loaded Template menu
    public List<Button> loadedButtons = new List<Button>();
    private int currentIndex = 0; // the current index, used for hiding

    private int maxIndex
    {
        get { return options.Length - (viewportMax - 1); }
    }

    public void OnEnable()
    {
        tailOffset = transform.position.y;
        Draw();
    }

    public void ScrollUp()
    {
        Scroll(-1);
    }

    public void ScrollDown()
    {
        Scroll(1);
    }

    public void Scroll(int add)
    {
        currentIndex = Mathf.Clamp(currentIndex+add, 0, maxIndex-1);

        if (currentIndex > 0) 
        {
            loadedButtons[currentIndex-1].gameObject.SetActive(false); // upper edge exits viewport
            loadedButtons[currentIndex + viewportMax-1].gameObject.SetActive(true); // lower edge enters viewport
        }
        if (currentIndex < maxIndex-1)
        {
            loadedButtons[currentIndex + viewportMax].gameObject.SetActive(false); // lower edge exits viewport
            loadedButtons[currentIndex].gameObject.SetActive(true); // upper edge enters viewport
        }
        UpdateArrows();
    }

    public void UpdateArrows()
    {
        upArrow.interactable = currentIndex > 0;
        downArrow.interactable = currentIndex < maxIndex - 1;
    }
    
    // generates the buttons from the options list
    public void Draw()
    {
        Clear();
        //clone template
        loadedMenu = Instantiate(template, template.parent);
        loadedMenu.gameObject.SetActive(true);
        
        // get button
        Transform button = loadedMenu.Find("Content").Find("Item");
        button.gameObject.SetActive(false);
        
        //enable/disable arrows
        bool arrowsVisible = options.Length > viewportMax;
        upArrow.gameObject.SetActive(arrowsVisible);
        downArrow.gameObject.SetActive(arrowsVisible);
        UpdateArrows();
        
        // gen buttons
        DrawButtons(button);
        upArrow.transform.SetAsFirstSibling();
        downArrow.transform.SetAsLastSibling();
        
    }

    public void MoveTail(float y)
    {
        tail.position = new Vector3(tail.position.x, y, tail.position.z);
    }

    private void Update()
    {
        //tail.localPosition = new Vector3(tail.localPosition.x, tailOffset, tail.localPosition.z); 
    }

    // Draws each button and adds a listener
    public void DrawButtons(Transform button)
    {
        for (int i=0; i<options.Length;i++)
        {
            BubbleMenuItem option = options[i];
            Transform item = Instantiate(button, button.parent);
            item.gameObject.name = $"{i}:{option.itemData}";
            item.Find("Item Label").GetComponent<Text>().text = option.itemData;
            
            Button itemButton = item.GetComponent<Button>();
            itemButton.onClick.AddListener(()=> option.itemEvent?.Invoke());
            itemButton.gameObject.SetActive(i < viewportMax);
            loadedButtons.Add(itemButton);
        }
    }

    // Clears the loaded menu
    public void Clear()
    {
        if (loadedMenu!=null)
        {
            Destroy(loadedMenu.gameObject);
            loadedMenu = null;
        }    
    }

    private void OnDestroy()
    {
        Destroy(tail.gameObject);
    }
}
