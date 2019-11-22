using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBars : MonoBehaviour
{
    public GameObject chatbar;
    public GameObject menubar;

    public void OpenChatBar()
    {
        if (!chatbar.activeSelf) chatbar.SetActive(true);
        else chatbar.SetActive(false); 
    }

    public void OpenMenuBar()
    {
        if (!menubar.activeSelf) menubar.SetActive(true);
        else menubar.SetActive(false);
    }
}
