using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassPlayerPos : MonoBehaviour
{
    public Transform m_player;

    private Material m_material;

    private void Start()
    {
        m_material = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        m_material.SetVector("_PlayerPosition", new Vector4(m_player.position.x, m_player.position.y, m_player.position.z, 0));
    }
}
