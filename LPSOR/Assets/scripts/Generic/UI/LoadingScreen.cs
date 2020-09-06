using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LoadingScreen : GameScreen // note to self: add more booleans to check if each seperate component has loaded ig
    {
#region Private classes
        private bool isLoaded
        {
            get{return (load == 1.00f);}
        }
        private bool isLoading = false;
        private float load;
#endregion

#region Public classes
        [Header("Object references")]
        public Image backgroundImage;
        public Slider loadingBar;

        public Sprite backgroundTexture
        {
            get{return  backgroundImage.sprite;}
            set{backgroundImage.sprite = value;}
        }
#endregion

        private void Start()
        {
            LoadingVisual();
        }


        public void FinishLoading()
        {         
            StartCoroutine(TweenLoadingBar(1.00f));
        }

        public void LoadingVisual()
        {
            StartCoroutine(TweenLoadingBar(0.25f));

        }

        public IEnumerator TweenLoadingBar(float percentage)
        {
            // waits for the past tweening to finish before starting a new one
            if(isLoading) yield return new WaitUntil(()=>!isLoading); 
            isLoading = true;
            float quantity = percentage-load;

            // loops through the number of times you have to reach to arrive at the percentage
            for(float interval = 0; interval <= quantity; interval+=0.01f)
            {
                loadingBar.value = load+interval;
                yield return new WaitForSeconds(1.00f/60.00f);
            }
            // sets the load value
            load = percentage;

            // destroys the gameobject if loading is completed
            if(load == 1.00f) gameUI.RemoveScreen(this.gameObject);
            isLoading = false;
        }

        
    }
}
