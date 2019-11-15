using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OLD SCRIPT UNUSED
/// Activated the glow when looking at trash items
/// </summary>
public class TrashHighlighter : MonoBehaviour
{
    public CameraController m_cameraController;
    public float m_glowDist;

    Renderer m_lastLookedAt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_cameraController.DirectionRay, out hit, m_glowDist, 1 << LayerMask.NameToLayer("Highlighters"), QueryTriggerInteraction.Collide))
        {
            if (m_lastLookedAt)
            {
                m_lastLookedAt.material.SetFloat("_LookAt", 0);
            }
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.material.SetFloat("_LookAt", 1);
                m_lastLookedAt = renderer;
            }
            else
            {
                Renderer childRenderer = hit.collider.GetComponentInChildren<Renderer>();
                if (childRenderer)
                {
                    childRenderer.material.SetFloat("_LookAt", 1);
                    m_lastLookedAt = childRenderer;
                }
            }
        }
        else if (m_lastLookedAt)
        {
            m_lastLookedAt.material.SetFloat("_LookAt", 0);
            m_lastLookedAt = null;
        }
    }
}
