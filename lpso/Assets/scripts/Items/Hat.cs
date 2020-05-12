using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hat", menuName = "Items/Hat", order = 1)]
public class Hat : Item, IUseable, IWearable
{
    [SerializeField]
    private List<Sprite> spriteorder;

    public List<int> spritesort;
    public string parent;
    public int WearingSlot = 0;

    private List<string> angles = new List<string>(){
        "f",
        "fL",
        "L",
        "bL",
        "b"
        };

    /// 1 = f
    /// 2 = fl
    /// 3 = L
    /// 4 = bl
    /// 5 = b

    public void Use(Transform character)
    {
        if (Wearing != true)
        {
            int i = 0;
            foreach (string v in angles)
            {
                GameObject items = new GameObject(Name);
   
                items.transform.SetParent(character.Find(v).Find(parent));
                SpriteRenderer item = items.AddComponent<SpriteRenderer>();
                item.sprite = spriteorder[i];
                item.sortingOrder = spritesort[i];
                items.transform.localPosition = new Vector3(0, 0, 0);
                items.transform.localScale = new Vector3(1, 1, 1);

                character.gameObject.GetComponent<GlobalSort>().AddToPlayerSort(item);
                character.gameObject.GetComponent<GlobalSort>().setPlayerSort();
                i++;

            }

        }
        else
        {
            foreach (string v in angles)
            {
                Transform Parent = character.Find(v).Find(parent);
                Transform Sprite = Parent.Find(Name);
                character.gameObject.GetComponent<GlobalSort>().RemoveFromSort(Sprite);
                Destroy(Sprite.gameObject);
            }
        }

    }

    public void ChangeSlot(InventoryScript inventoryRef)
    {
        if (Wearing) inventoryRef.AddItem(this, 1);
        else inventoryRef.SetWearing(WearingSlot, this);
        Wearing = !Wearing;
    }
}
