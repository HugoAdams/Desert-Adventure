using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    SHIP_BASE,
    SHIP_MAST,
    SHIP_SAIL,
    SHIP_TILLER
}

public abstract class ItemPickUp : MonoBehaviour {

    public abstract void OnPickUp(PlayerController _player);
}
