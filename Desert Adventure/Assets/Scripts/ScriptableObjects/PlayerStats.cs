using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/PlayerStats")]
public class PlayerStats : ScriptableObject {

    public int Life;
    public bool BoatBase;
    public bool BoatMast;
    public bool BoatSail;
    public bool BoatTiller;

    public void Reset(PlayerStats _defaultStats)
    {
        Life = _defaultStats.Life;
        BoatBase = _defaultStats.BoatBase;
        BoatMast = _defaultStats.BoatMast;
        BoatSail = _defaultStats.BoatSail;
        BoatTiller = _defaultStats.BoatTiller;
    }
}
