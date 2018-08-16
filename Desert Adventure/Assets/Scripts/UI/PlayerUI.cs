using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerUI : MonoBehaviour {

    [Header("General")]
    public PlayerStats m_baseViewedStats;
    public PlayerStats m_currentViewedStats;
    public PlayerHealthBar m_healthBar;

    [Header("UI Controllers for piece piece")]
    public BoatPieceUI m_boatBase;
    public BoatPieceUI m_boatMast;
    public BoatPieceUI m_boatSail;
    public BoatPieceUI m_boatTiller;

    private void Start()
    {
        if (!m_currentViewedStats)
            return;

        if (m_healthBar)
        {
            m_healthBar.SetMaxLife(m_baseViewedStats.Life);
            m_healthBar.UpdateLife(m_currentViewedStats.Life);
        }

        UpdateBoatUI();
    }
    public void OnHealthChange()
    {
        if (m_healthBar)
            m_healthBar.UpdateLife(m_currentViewedStats.Life);
    }

    public void OnBoatProgressChange(BoatPiece _boatPiece)
    {
        if (!m_currentViewedStats)
            return;

        if (_boatPiece == BoatPiece.BASE)
        {
            StartCoroutine(DelayedHelperText("Press <sprite=8> to get into your boat!"));

            if (!m_currentViewedStats.BoatTiller && (!m_currentViewedStats.BoatMast || !m_currentViewedStats.BoatSail))
                StartCoroutine(DelayedHelperText("missing some sort of engine though...", 3.8f));
        }

        else if (m_currentViewedStats.BoatBase)
        {
            if (_boatPiece == BoatPiece.TILLER && (!m_currentViewedStats.BoatMast || !m_currentViewedStats.BoatSail))
                StartCoroutine(DelayedHelperText("You now have control over the boat!"));

            else if ((_boatPiece == BoatPiece.MAST || _boatPiece == BoatPiece.SAIL) && m_currentViewedStats.BoatBase
                && m_currentViewedStats.BoatMast && m_currentViewedStats.BoatSail && !m_currentViewedStats.BoatTiller)
                StartCoroutine(DelayedHelperText("You now have control over the boat!"));

            else if (m_currentViewedStats.BoatBase == m_currentViewedStats.BoatMast && m_currentViewedStats.BoatMast == m_currentViewedStats.BoatSail
                && m_currentViewedStats.BoatSail == m_currentViewedStats.BoatTiller && m_currentViewedStats.BoatTiller)
                StartCoroutine(DelayedHelperText("Your boat has been upgraded!"));
        }

        // If got all pieces, tell new objective
        if (m_currentViewedStats.BoatBase && m_currentViewedStats.BoatMast && m_currentViewedStats.BoatSail && m_currentViewedStats.BoatTiller)
            Invoke("NewObjectiveText", 4.0f);

        UpdateBoatUI();
    }

    void NewObjectiveText()
    {
        EffectCanvas.Instance.TitleText("NEW OBJECTIVE: Reach the outlook tower");
    }

    IEnumerator DelayedHelperText(string _text, float _delayTime = 1)
    {
        yield return new WaitForSeconds(_delayTime);
        EffectCanvas.Instance.HelperText(_text);
        yield return null;
    }

    void UpdateBoatUI()
    {
        if (m_boatBase)
            m_boatBase.ChangeSprite(m_currentViewedStats.BoatBase);

        if (m_boatMast)
            m_boatMast.ChangeSprite(m_currentViewedStats.BoatMast);

        if (m_boatSail)
            m_boatSail.ChangeSprite(m_currentViewedStats.BoatSail);

        if (m_boatTiller)
            m_boatTiller.ChangeSprite(m_currentViewedStats.BoatTiller);
    }

    private void OnEnable()
    {
        EventsController.Instance.OnPlayerLifeChange += OnHealthChange;
        EventsController.Instance.OnBoatPieceObtained += OnBoatProgressChange;
    }

    private void OnDisable()
    {
        EventsController.Instance.OnPlayerLifeChange -= OnHealthChange;
        EventsController.Instance.OnBoatPieceObtained -= OnBoatProgressChange;

    }
}
