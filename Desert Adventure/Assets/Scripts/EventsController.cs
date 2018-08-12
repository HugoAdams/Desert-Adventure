using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsController : MonoBehaviour {

    public static EventsController Instance
    { get; private set; }

    public delegate void PlayerLifeChange();
    public delegate void BoatPieceObtained(BoatPiece _boatPiece);

    public event PlayerLifeChange OnPlayerLifeChange;
    public event BoatPieceObtained OnBoatPieceObtained;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Set singleton
        Instance = this;
    }

    public void TriggerPlayerLifeChange()
    {
        if (OnPlayerLifeChange != null)
            OnPlayerLifeChange();
    }

    public void TriggerBoatPieceObtained(BoatPiece _boatPiece)
    {
        if (OnBoatPieceObtained != null)
            OnBoatPieceObtained(_boatPiece);
    }
}
