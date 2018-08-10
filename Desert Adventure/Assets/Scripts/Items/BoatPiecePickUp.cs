using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatPiecePickUp : ItemPickUp {

    public enum ShipPiece
    {
        BASE,
        MAST,
        SAIL,
        TILLER
    }

    public ShipPiece m_boatPiece;

    public override void OnPickUp(PlayerController _player)
    {
        // Update player stats, then despawn this object
        switch (m_boatPiece)
        {
            case ShipPiece.BASE:
                _player.m_currentStats.BoatBase = true;
                break;
            case ShipPiece.MAST:
                _player.m_currentStats.BoatMast = true;
                break;
            case ShipPiece.SAIL:
                _player.m_currentStats.BoatSail = true;
                break;
            case ShipPiece.TILLER:
                _player.m_currentStats.BoatTiller = true;
                break;
            default:
                break;
        }

        Destroy(gameObject);

        EventsController.Instance.TriggerBoatPieceObtained();
    }
}
