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
}
