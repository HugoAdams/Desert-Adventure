﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoatPiece
{
    BASE,
    MAST,
    SAIL,
    TILLER
}

public class BoatPiecePickUp : ItemPickUp {

    public BoatPiece m_boatPiece;

    public override void OnPickUp(PlayerController _player)
    {
        // Update player stats, then despawn this object
        switch (m_boatPiece)
        {
            case BoatPiece.BASE:
                _player.m_currentStats.BoatBase = true;
                EffectCanvas.Instance.TitleText("OBTAINED: BOAT HULL <sprite=3>");
                break;
            case BoatPiece.MAST:
                _player.m_currentStats.BoatMast = true;
                EffectCanvas.Instance.TitleText("OBTAINED: BOAT MAST <sprite=4>");
                break;
            case BoatPiece.SAIL:
                _player.m_currentStats.BoatSail = true;
                EffectCanvas.Instance.TitleText("OBTAINED: BOAT SAIL <sprite=5>");
                break;
            case BoatPiece.TILLER:
                _player.m_currentStats.BoatTiller = true;
                EffectCanvas.Instance.TitleText("OBTAINED: BOAT TILLER <sprite=6>");
                break;
            default:
                break;
        }

        if (!transform.parent)
            Destroy(gameObject);
        else
            Destroy(transform.parent.gameObject);

        EventsController.Instance.TriggerBoatPieceObtained(m_boatPiece);
    }

    private void OnTriggerEnter(Collider other)
    {
        // ASSUMING THE OTHER IS PLAYER, ONLY PLAYER CAN BE HIT
        // player has player only collider on child
        PlayerController currentStats = other.GetComponentInParent<PlayerController>();
        OnPickUp(currentStats);
    }
}
