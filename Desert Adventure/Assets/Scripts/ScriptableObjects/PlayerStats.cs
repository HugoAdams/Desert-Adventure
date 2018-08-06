using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom")]
public class PlayerStats : ScriptableObject {

    public int Life;
    public bool BoatBase;
    public bool BoatMast;
    public bool BoatSail;
    public bool BoatTiller;

    public PlayerUI CurrentPlayerUI;

    public void HitPlayer(int _damage)
    {
        Life -= _damage;

        if (CurrentPlayerUI)
            CurrentPlayerUI.OnHealthChange();

    }

    public void UpdatePlayerProgress()
    {
        if (CurrentPlayerUI)
            CurrentPlayerUI.OnBoatProgressChange();
    }
}
