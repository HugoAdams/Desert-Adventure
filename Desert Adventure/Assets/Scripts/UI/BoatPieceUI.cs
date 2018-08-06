using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BoatPieceUI : MonoBehaviour {

    public Sprite m_inactiveSprite;
    public Sprite m_activeSprite;
    private Image m_image;

    private void Awake()
    {
        m_image = GetComponent<Image>();
    }

    public void ChangeSprite(bool _active)
    {
        if (_active)
            m_image.sprite = m_activeSprite;
        else
            m_image.sprite = m_inactiveSprite;
    }
}
