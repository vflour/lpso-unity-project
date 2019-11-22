using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "Items/Consumable", order = 1)]
public class Consumable : Item, IUseable
{
    public int FeedAmount;
    
    /// 1 = f
    /// 2 = fl
    /// 3 = L
    /// 4 = bl
    /// 5 = b

    public void Use(Transform character)
    {
    }
}
