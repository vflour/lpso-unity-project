using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GlobalSort : MonoBehaviour
{

    public Transform character;
    public Transform map;
    Dictionary<SpriteRenderer, int> character_sort = new Dictionary<SpriteRenderer, int>();
    public bool rdy;
    bool act = false;
    List<SpriteRenderer> charpieces;
    // Update is called once per frame

    private void Update()
    {
        if (rdy)
        {
            setPlayerSort();
        }
    }

    public void SortPlr()
    {
        defcharsort();
        setPlayerSort();
        StartCoroutine(SetObjSort());
        rdy = true;
    }

    IEnumerator SetObjSort()
    {
        SpriteRenderer[] renderers = map.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer v in renderers)
        {
            v.sortingOrder = (int)((v.transform.position.y) * -100);
            yield return null;
        }
    }

    public void setPlayerSort()
    {
        foreach (SpriteRenderer v in charpieces)
        {
            if (v == null)
            {
                charpieces.Remove(v);
                break;
            }
            v.sortingOrder = character_sort[v] + (int)((character.position.y) * -100);
        }
    }

    public void RemoveFromSort(Transform Object)
    {
        rdy = false;
        SpriteRenderer Sprite = Object.GetComponent<SpriteRenderer>();
        charpieces.Remove(Sprite);
        rdy = true;
    }

    void defcharsort()
    {
        SpriteRenderer[] renderers = character.GetComponentsInChildren<SpriteRenderer>(true);
        foreach(SpriteRenderer v in renderers)
        {
            character_sort[v] = v.sortingOrder;
        }
        charpieces = renderers.OrderBy(SpriteRenderer => SpriteRenderer.sortingOrder).ToList();

    }

    Transform GetActiveSprite(Transform plr)
    {
        Transform[] children = plr.GetComponentsInChildren<Transform>();
        Transform isactive = null;

        foreach(Transform v in children)
        {
            if (v.gameObject.activeSelf == true) isactive = v;
        }
        return isactive;
    }

    public void AddToPlayerSort(SpriteRenderer newpiece)
    {
        character_sort[newpiece] = newpiece.sortingOrder;
        newpiece.sortingOrder += (int)((character.position.y) * -100);
        act = true;
        StartCoroutine(SetVWhilePossible(newpiece, character_sort[newpiece]));
        List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        foreach (KeyValuePair<SpriteRenderer, int> itemk in character_sort.OrderBy(key => key.Value))
        {
            renderers.Add(itemk.Key);
        }
        charpieces = renderers;
        act = false;
        setPlayerSort();

    }

    IEnumerator SetVWhilePossible(SpriteRenderer newpiece, int sortord)
    {
        while (act)
        {
            newpiece.sortingOrder = sortord + (int)((character.position.y) * -100);
            yield return null;
        }
        
    }
}
