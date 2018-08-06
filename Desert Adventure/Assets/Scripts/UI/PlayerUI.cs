using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [Header("General")]
    public PlayerStats m_currentViewedStats;
    public PlayerHealthBar m_healthBar;

    [Header("UI Controllers for piece piece")]
    public BoatPieceUI m_boatBase;
    public BoatPieceUI m_boatMast;
    public BoatPieceUI m_boatSail;
    public BoatPieceUI m_boatTiller;

    private void Awake()
    {
        // Link up so PlayerStats know what UI is controlling it
        if (m_currentViewedStats)
            m_currentViewedStats.CurrentPlayerUI = this;
    }

    private void Start()
    {
        if (!m_currentViewedStats)
            return;

        if (m_healthBar)
            m_healthBar.UpdateLife(m_currentViewedStats.Life);

        OnBoatProgressChange();
    }
    public void OnHealthChange()
    {
        if (m_healthBar)
            m_healthBar.UpdateLife(m_currentViewedStats.Life);
    }

    public void OnBoatProgressChange()
    {
        if (!m_currentViewedStats)
            return;

        if (m_boatBase)
            m_boatBase.ChangeSprite(m_currentViewedStats.BoatBase);

        if (m_boatMast)
            m_boatBase.ChangeSprite(m_currentViewedStats.BoatMast);

        if (m_boatSail)
            m_boatBase.ChangeSprite(m_currentViewedStats.BoatSail);

        if (m_boatTiller)
            m_boatBase.ChangeSprite(m_currentViewedStats.BoatTiller);
    }
}
