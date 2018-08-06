using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPickUp : ItemPickUp
{
    [Range(1, 6)]
    public int m_healAmount = 2;

    public override void OnPickUp()
    {
        // TODO: Tell player they have healed
    }

}
