using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotSpawner : MonoBehaviour {

    public int m_spawnRadius;

    public GameObject m_pot;

    public int m_numOfPots;

    public int m_respawnDistanceThreshold;

    private int terrainLayerMask = 1 << 9;

    private List<GameObject> m_pots = new List<GameObject>();

    private GameObject player;

    // Use this for initialization
    void Start()
    {
        SpawnPots();
        player = GameObject.Find("Player");
        StartCoroutine(CheckRespawnEnemies());
    }

    IEnumerator CheckRespawnEnemies()
    {
        while (true)
        {
            if ((player.transform.position - this.transform.position).sqrMagnitude > m_respawnDistanceThreshold)
            {
                RespawnPots();
            }
            yield return new WaitForSeconds(5);
        }
    }

    void RespawnPots()
    {
        for (int i = 0; i < m_pots.Count; ++i)
        {
            if (m_pots[i] == null)
            {
                m_pots[i] = SpawnEnemy(m_pot);
                return;
            }
        }
    }

    void SpawnPots()
    {
        for (int i = 0; i < m_numOfPots; ++i)
        {
            m_pots.Add(SpawnEnemy(m_pot));
        }
    }

    GameObject SpawnEnemy(GameObject enemy)
    {
        Vector2 random2dPosition = Random.insideUnitCircle * m_spawnRadius;
        Vector3 randomPosition = new Vector3(transform.position.x + random2dPosition.x, transform.position.y, transform.position.z + random2dPosition.y);
        randomPosition.y = transform.position.y - ReturnGroundLevel(randomPosition) + 0.5f;
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
