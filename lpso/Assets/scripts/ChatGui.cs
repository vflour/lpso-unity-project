using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatGui : MonoBehaviour
{
    public InputField chatgui;
    public Transform plrgui;
    public GameObject chat_bubble;
    public Button okbutton;
    public LoadIntoMap loadintomap;
    public int messagequeue = 2;
    public bool focused = false;

    private void Awake()
    {
        okbutton.onClick.AddListener(() => {
            SendMsg();
            });
    }

    private void Update()
    {
        if (chatgui.isFocused && !focused) StartCoroutine(delayfocus());
        if (Input.GetKeyUp(KeyCode.Return) && focused == true) SendMsg();
        if (loadintomap.loaded) plrgui.position = loadintomap.character.transform.position;

    }

    IEnumerator delayfocus()
    {
        focused = true;
        yield return new WaitForSeconds(0.5f);
        focused = false;
    }

    public void SendMsg()
    {
        string text = chatgui.text;
        if (messagequeue > 0 && text.Length > 0) 
        {
            if (messagequeue == 1)
            {
               StartCoroutine(movebubbleup(plrgui.GetChild(0)));
            }
            messagequeue--;
            chatgui.text = "";

            GameObject bubble = Instantiate(chat_bubble, new Vector3(0, 0, 0), Quaternion.identity, plrgui);
            bubble.GetComponent<RectTransform>().localPosition = new Vector3(-10, 40, 0);
            bubble.GetComponentInChildren<Text>().text = text;
            Animator animator = bubble.GetComponentInChildren<Animator>();
            StartCoroutine(setbubbleanim(animator,bubble));
        }
    }

    IEnumerator setbubbleanim(Animator animator,GameObject bubble)
    {
        animator.SetBool("isopen", true);
        yield return new WaitForSeconds(5);
        animator.SetBool("isopen", false);
        yield return new WaitForSeconds(.4f);
        Destroy(bubble);
        messagequeue++;
    }
    IEnumerator movebubbleup(Transform bubble)
    {
        Vector3 targetpos = new Vector3(-10, 90, 0);
        Vector3 velocity = Vector3.zero;
        while (messagequeue < 2 && bubble!= null)
        {

            bubble.GetComponent<RectTransform>().localPosition = Vector3.SmoothDamp(bubble.GetComponent<RectTransform>().localPosition, targetpos, ref velocity, 0.075f); ;

            yield return null;
        }
    }


}


