using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public int m_spawnRadius;

    public GameObject m_bigRockEnemy;
    public GameObject m_spearEnemy;

    public int m_numOfBigRockEnemies;
    public int m_numOfSpearEnemies;

    public int m_respawnDistanceThreshold;

    private int terrainLayerMask = 1 << 9;

    private List<GameObject> m_bigRockEnemies = new List<GameObject>();
    private List<GameObject> m_spearEnemies = new List<GameObject>();

    private GameObject player;

    // Use this for initialization
    void Start () {
        SpawnEnemies();
        player = GameObject.Find("Player");
        StartCoroutine(CheckRespawnEnemies());
    }

    IEnumerator CheckRespawnEnemies()
    {
        while (true)
        {
            if ((player.transform.position - this.transform.position).sqrMagnitude > m_respawnDistanceThreshold)
            {
                RespawnEnemies();
            }
            yield return new WaitForSeconds(5);
        }
    }

    void RespawnEnemies()
    {
        for (int i = 0; i < m_spearEnemies.Count; ++i)
        {
            if (m_spearEnemies[i] == null)
            {
                m_spearEnemies[i] = SpawnEnemy(m_spearEnemy);
                return;
            }
        }

        for (int i = 0; i < m_bigRockEnemies.Count; ++i)
        {
            if (m_bigRockEnemies[i] == null)
            {
                m_bigRockEnemies[i] = SpawnEnemy(m_bigRockEnemy);
                return;
            }
        }
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_spawnRadius);
    }
}
