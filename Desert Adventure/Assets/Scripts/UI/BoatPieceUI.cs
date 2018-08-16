using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BoatPieceUI : MonoBehaviour {

    public Sprite m_inactiveSprite;
    public Sprite m_activeSprite;
    private Image m_image;
    float m_deactiveAlpha;
    private void Awake()
    {
        m_image = GetComponent<Image>();
        m_deactiveAlpha = m_image.color.a;
    }

    public void ChangeSprite(bool _active)
    {
        if (_active)
        {
            m_image.sprite = m_activeSprite;
            m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, 1);
        }
        else
        {
            m_image.sprite = m_inactiveSprite;
            m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, m_deactiveAlpha);
        }
    }
}
