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

    private void Awake()
    {
        m_imageList = new List<Transform>();
    }

    public void UpdateLife(int _newLife)
    {
        m_imageList.Clear();
        ClearCurrentImages();

        Vector3 m_currentPosition = m_imageStartPoint.position;
        for (int i = 0; i < m_maxLife; i++)
        {
            if (i < _newLife)
                m_imageList.Add(Instantiate(m_singleEmptyPrefab, m_currentPosition, Quaternion.identity, transform));
            else
                m_imageList.Add(Instantiate(m_singleEmptyPrefab, m_currentPosition, Quaternion.identity, transform));

            m_currentPosition += m_OffsetPerLifePoint;
        }
    }

    public void SetMaxLife(int _maxLife) { m_maxLife = _maxLife; }

    void ClearCurrentImages()
    {
        foreach (Transform t in m_imageList)
            Destroy(t.gameObject);
    }
}
