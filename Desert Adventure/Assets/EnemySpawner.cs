using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public int m_spawnRadius;

    public GameObject m_bigRockEnemy;
    public GameObject m_spearEnemy;

    public int m_numOfBigRockEnemies;
    public int m_numOfSpearEnemies;

    private int terrainLayerMask = 1 << 9;

    private List<GameObject> m_bigRockEnemies = new List<GameObject>();
    private List<GameObject> m_spearEnemies = new List<GameObject>();

    private bool m_playerCloseby;

    // Use this for initialization
    void Start () {
        SpawnEnemies();
    }

    private void Update()
    {
        if (m_playerCloseby)
            return;

    }

    void RespawnEnemies()
    {

    }

    void SpawnEnemies()
    {
        for (int i = 0; i < m_numOfBigRockEnemies; ++i)
        {
            m_bigRockEnemies.Add(SpawnEnemy(m_bigRockEnemy));
        }
        for (int i = 0; i < m_numOfSpearEnemies; ++i)
        {
            m_spearEnemies.Add(SpawnEnemy(m_spearEnemy));
        }
    }

    GameObject SpawnEnemy(GameObject enemy)
    {
        Vector2 random2dPosition = Random.insideUnitCircle * m_spawnRadius;
        Vector3 randomPosition = new Vector3(transform.position.x + random2dPosition.x, transform.position.y, transform.position.z + random2dPosition.y);
        randomPosition.y = transform.position.y - ReturnGroundLevel(randomPosition);
        return Instantiate(enemy, randomPosition, transform.rotation);
    }

    float ReturnGroundLevel(Vector3 position)
    {
        RaycastHit hit;
        Physics.Raycast(position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, terrainLayerMask);
        return hit.distance;
    }

    private void OnTriggerExit(Collider other)
    {

    }
}
