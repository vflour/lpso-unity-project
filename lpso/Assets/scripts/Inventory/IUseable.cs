using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUseable
{
    void Use(Transform player);
}

public interface IWearable
{
    void ChangeSlot(InventoryScript inventoryRef);
}

