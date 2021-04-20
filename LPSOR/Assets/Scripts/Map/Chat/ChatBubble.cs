using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Map
{
    public class ChatBubble : MonoBehaviour
    {
        public Text messageBox;
        public ChatCanvas canvas;
        public Animator animator;
        private string _message;
        private bool _opened;
        private float time = 5;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                messageBox.text = value;
            }
        }
        public bool Opened
        {
            get => _opened;
            set
            {
                _opened = value;
                animator.SetBool("isopen", value);
            }
        }

        public void Start()
        {
            _opened = true;
        }
        public void Update()
        {
            if (Opened)
            {
                time -= Time.deltaTime;
                if (time <= 0)
                    StartCoroutine(Remove());
            }
        }

        public IEnumerator Remove()
        {
            Opened = false;
            yield return new WaitForSeconds(0.4f);
            canvas.RemoveBubble(this);
        }
    }
}
