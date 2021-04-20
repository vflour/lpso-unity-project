using System;
using UnityEngine;

namespace Game.UI
{
    public enum DialogType{Information,Item}
    public class DialogBox : GameScreen
    {
        public RectTransform container;
        private RectTransform rectTransform;
        private Camera currentCamera;
        private void Start()
        {
            rectTransform = this.GetComponent<RectTransform>();
            currentCamera = Camera.main;
        }

        private void Update()
        {
            Vector2 canvasPosition = Input.mousePosition;
            Transform canvas = gameUI.uiSpace;
            RectTransformUtility.ScreenPointToLocalPointInRectangle( canvas as RectTransform, Input.mousePosition, currentCamera, out canvasPosition);
            rectTransform.position = canvas.transform.TransformPoint(canvasPosition);
        }
        
        public virtual void SetData(DialogData data)
        {
            if(data.size.x!=0 && data.size.y!=0)
                container.sizeDelta = data.size;
            if(data.offset.x!=0 && data.offset.y!=0)
                container.localPosition = data.offset;
        }
        
    }
}