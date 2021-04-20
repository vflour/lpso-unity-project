using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Map
{
    public class ChatCanvas : MonoBehaviour
    {
        public List<ChatBubble> bubbles = new List<ChatBubble>();

        #region Adding and Removing bubbles
        public void AddBubble(ChatBubble bubble)
        {
            if (bubbles.Count >= 3) throw new Exception("Too many chat bubbles."); 
            bubbles.Add(bubble);
            bubble.canvas = this;
        }

        public void RemoveBubble(ChatBubble bubble)
        {
            bubbles.Remove(bubble);
            Destroy(bubble.gameObject);
        }
        #endregion

        #region Handling Bubble positions

        // update the bubble position each frame
        // this is so that bubbles can overlap on eachother
        Vector3 velocity = Vector3.up;
        private void Update()
        {
            for(int i = 0; i < bubbles.Count; i++)
            {
                RectTransform bubbleTransform = bubbles[i].GetComponent<RectTransform>();
                int positionIndex = bubbles.Count - 1 - i;
                Vector3 position = new Vector3(0 ,45, 0)+new Vector3(0,60,0)*positionIndex;
                bubbleTransform.localPosition =
                    Vector3.SmoothDamp(bubbleTransform.localPosition, position, ref velocity, 0.03f);
            }
        }

        #endregion
    }
}
