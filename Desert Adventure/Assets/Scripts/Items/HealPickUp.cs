using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPickUp : ItemPickUp
{
    [Range(1, 6)]
    public int m_healAmount = 2;

    public override void OnPickUp(PlayerController _player)
    {
        // TODO: Heal the player
        //       Delete this object
    }

}
