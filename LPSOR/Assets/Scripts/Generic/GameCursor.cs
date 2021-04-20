using System;
using UnityEngine;

namespace Game
{
    public class GameCursor : MonoBehaviour
    {
        public GameObject sideIcon;
        private ParticleSystem particleSystem;

        public bool particles
        {
            set
            {
                if(value)
                    particleSystem.Play();
                else
                    particleSystem.Stop();
            }
        }
        public void SetIcon(GameObject icon)
        {
            RemoveIcon();
            sideIcon = Instantiate(icon, transform);
        }
        public void RemoveIcon()
        {
            if(sideIcon!=null)
                Destroy(sideIcon);
        }
        private void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }
        private void Update()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition;
            transform.position -= Vector3.forward; // set the z value to -1
        }
    }
}