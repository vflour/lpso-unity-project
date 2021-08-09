using System.Collections;
using System.Collections.Generic;
using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class MouseHandler : MonoBehaviour, IHandler
    {
        
        #region Initialization
        // IHandler methods
        public GameSystem system {get; set;}
        public bool Display { get; set; }

        public void Activate()
        {
            // disable default cursor and add the new one
            Cursor.visible = false;
            GameObject cursorObject = Instantiate(cursorPrefab);
            cursor = cursorObject.AddComponent<GameCursor>();
            
            // initialize all dialog prompts
            SetDialogPrompts();
        }
        
        public void Update()
        {
            if (enableRaycast){
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    OnMouseRayCast(); 
                }
            }
        }
        #endregion
        #region Raycast functions

        public bool enableRaycast = false;

        public delegate void MouseRaycastFunction(RaycastHit2D raycastHit);
        private Dictionary<string,MouseRaycastFunction> raycastEvents = new Dictionary<string,MouseRaycastFunction>();

        public void RaycastListen(string key, MouseRaycastFunction raycastFunction)
        {
            raycastEvents.Add(key, raycastFunction);
        }

        public void RaycastDisconnect(string key)
        {
            raycastEvents.Remove(key);
        }

        private bool isFiring = false;
        public void OnMouseRayCast()
        {
            if (Input.GetMouseButtonDown(0) && !isFiring)
            {
                isFiring = true;
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray,Mathf.Infinity);

                // If it hits something...
                if (hit.collider != null)
                {
                    foreach (MouseRaycastFunction raycastFunction in raycastEvents.Values)
                        raycastFunction(hit);
                }

                isFiring = false;
            }
        }
        #endregion
        #region Dialog box hovering
        public PrefabDatabase dialogPrefabs;
        private DialogBox currentDialogBox;

        public void SetDialogPrompts()
        {
            DialogPrompt[] dialogPrompts = GameObject.FindObjectsOfType<DialogPrompt>();
            foreach (DialogPrompt prompt in dialogPrompts)
                prompt.mouseHandler = this;
        }
        public void SetDialogPrompts(Transform transform)
        {
            DialogPrompt[] dialogPrompts = transform.GetComponentsInChildren<DialogPrompt>();
            foreach (DialogPrompt prompt in dialogPrompts)
                prompt.mouseHandler = this;
        }

        public void CreateDialog(DialogData data)
        {
            RemoveDialog();
            GameObject dialogPrefab = dialogPrefabs.Data[data.type.ToString()];
            currentDialogBox = system.GetHandler<GameUI>().InstantiateScreen("DialogBox",dialogPrefab).GetComponent<DialogBox>();
            currentDialogBox.SetData(data);
            SetCursorParticles(true);
        }

        public void RemoveDialog()
        {
            if (system.GetHandler<GameUI>().HasScreen("DialogBox"))
            {
                system.GetHandler<GameUI>().RemoveScreen("DialogBox");
                currentDialogBox = null;
            }
            SetCursorParticles(false);
        }
        
        #endregion
        #region Cursor
        public enum CursorIcons {None,Loading,Walk,Deny}
        private GameCursor cursor;
        public GameObject cursorPrefab;
        public PrefabDatabase iconPrefabs;
        
        public void SetCursorIcon(CursorIcons icon)
        {
            if (icon != CursorIcons.None)
                cursor.SetIcon(iconPrefabs.Data[icon.ToString()]);
            else
                cursor.RemoveIcon();
        }

        public void SetCursorParticles(bool toggle)
        {
            cursor.particles = toggle;
        }
        #endregion
    }   
}

