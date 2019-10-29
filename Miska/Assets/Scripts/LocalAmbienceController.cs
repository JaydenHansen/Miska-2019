using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LocalAmbienceController : MonoBehaviour
{
    //Ignore Terrain Collision

    //Public void list

    GameObject m_player;

    public Duck[] m_ducks; //make generic

    private void Start()
    {
        m_player = GameObject.Find("Character(1)");
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.gameObject == m_player)
    //    {
    //        foreach (var duck in m_ducks)
    //        {
    //            duck.PlayDuckSound();
    //        }
    //    }
    //}
}
