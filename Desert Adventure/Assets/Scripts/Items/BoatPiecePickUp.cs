using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatPiecePickUp : ItemPickUp {

    enum ShipPiece
    {
        BASE,
        MAST,
        SAIL,
        TILLER
    }

    public override void OnPickUp(PlayerController _player)
    {
        // TODO: Get player's PlayerStats
        //       Set piece they picked up to be true in PlayerStats

        EventsController.Instance.TriggerBoatPieceObtained();
    }
}
