using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialScene : MonoBehaviour {

    [Header("items must be >= cacti")]
    public List<CactusMin> Cactilist;
    public List<Transform> ItemList;

    float startTimeToEnd = -1;

    Transform m_player = null;

    Transform m_oldCamPos = null;

    Vector3 m_campos, m_camrot;

    bool m_started = false;
    public ParticleSystem m_splosion;
    GameObject psgo;
    // Use this for initialization
    private void Start()
    {
        for (int i = 0; i < ItemList.Count; i++)
        {
            ItemList[i].gameObject.SetActive(false);
        }
        ItemList[3].GetChild(2).gameObject.SetActive(false);

        m_campos = new Vector3(358.3f, 29.9f, 199.2f);
        m_campos = new Vector3(350, 14.9f, 215.12f);
        m_camrot = new Vector3(30.47f, 22.44f, 0);

        
    }

    void SpecialStart()
    {
        psgo = Instantiate(m_splosion.gameObject);
        psgo.transform.position = transform.position;
        psgo.transform.localScale = new Vector3(4,4,4);

        Camera.main.GetComponent<CameraController>().b_auto = false;
        Camera.main.GetComponent<CameraController>().m_target = null;
        m_oldCamPos = Camera.main.transform;
        Camera.main.transform.position = m_campos;
        Camera.main.transform.eulerAngles = m_camrot;

        for (int i = 0; i < Cactilist.Count; i++) 
        {
            Cactilist[i].SpecRunToTransform(ItemList[i]);
        }
        m_started = true;
    }

    void SpecialEnd()
    {
        ItemList[3].GetChild(2).gameObject.SetActive(true);
        PlayerDontMove(false);
        Camera.main.GetComponent<CameraController>().b_auto = true;
        Camera.main.GetComponent<CameraController>().m_target = m_player;

        EffectCanvas.Instance.TitleText("OBJECTIVE: " + "Recollect your 4 ship pieces");
        Destroy(gameObject);
        //done
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            SpecialStart();
        }

        if (m_started == true)
        {
            CheckIfItemsTaken();
        }
    }

    void CheckIfItemsTaken()
    {
        bool alloff = true;
        for (int i=0; i< 3; i++)
        {
            if (ItemList[i].gameObject.activeSelf == true)
            {
                alloff = false;
                break;
            }
        }

        if(alloff == true)
        {
            startTimeToEnd = Time.time;
            Invoke("SpecialEnd", 4.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool hit = false, boat = false;
        if (other.GetComponentInParent<BoatMovement>())
        {
            hit = true;
            boat = true;
        }
        else if (other.GetComponent<PlayerMovement>())
        {
            hit = true;
        }

        if(hit == true)
        {
            BoatMovement bm = null;
            if(boat == true)
            {
               // m_player = other.GetComponentInChildren<PlayerMovement>().transform;
                bm = other.GetComponentInParent<BoatMovement>();
                m_player = bm.transform.GetChild(3);
                PlayerDontMove(true);
                bm.m_specialDismountBoat = true;
            }
            else
            {
                m_player = other.GetComponent<PlayerMovement>().transform;
                PlayerDontMove(true);
            }
            DropBoatParts();

            //turn off this triggerbox
            GetComponent<BoxCollider>().enabled = false;
            //turnoff all other colliders
            transform.GetChild(0).gameObject.SetActive(false);

            SpecialStart();
            //at the end of the scene turn off boat
        }

    }

    void PlayerDontMove(bool _move)
    {
        if(m_player == null)
        {
            return;
        }
        m_player.GetComponent<PlayerMovement>().m_specialDontMove = _move;
        m_player.GetComponent<PlayerActions>().m_specialDontMove = _move;
        m_player.GetComponent<PlayerController>().m_specialDontMove = _move;
    }

    void DropBoatParts()
    {
        m_player.GetComponent<PlayerController>().LoseAllBoatParts();

        for(int i=0; i< ItemList.Count; i++)
        {
            ItemList[i].gameObject.SetActive(true);
        }
    }

}
