using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealthBar : MonoBehaviour {

    public Transform m_singleFilledPrefab;
    public Transform m_singleEmptyPrefab;
    public Transform m_imageStartPoint;
    public Vector3 m_OffsetPerLifePoint;

    List<Transform> m_imageList;
    int m_maxLife;

    const float baseRatio = 16.0f / 9.0f;
    float m_screenRatio;

    private void Awake()
    {
        m_imageList = new List<Transform>();
        m_screenRatio = Camera.main.aspect / baseRatio;
    }

    public void UpdateLife(int _newLife)
    {
        ClearCurrentImages();
        m_imageList.Clear();

        Vector3 m_currentPosition = m_imageStartPoint.position;
        for (int i = 0; i < m_maxLife; i++)
        {
            if (i < _newLife)
                m_imageList.Add(Instantiate(m_singleFilledPrefab, m_currentPosition, Quaternion.identity, transform));
            else
                m_imageList.Add(Instantiate(m_singleEmptyPrefab, m_currentPosition, Quaternion.identity, transform));

            m_currentPosition += m_OffsetPerLifePoint * m_screenRatio;
        }
    }

    public void SetMaxLife(int _maxLife) { m_maxLife = _maxLife; }

    void ClearCurrentImages()
    {
        foreach (Transform t in m_imageList)
            Destroy(t.gameObject);
    }
}
