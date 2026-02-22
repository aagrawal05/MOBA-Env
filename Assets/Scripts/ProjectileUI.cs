using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileUI : MonoBehaviour
{
    [HideInInspector]
    private Renderer objectRenderer;
    [HideInInspector]
    private Projectile proj;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        proj = GetComponentInParent<Projectile>();
        if (proj.m_Squad == Squad.Blue)
            objectRenderer.material.color *= Color.blue;
        else if (proj.m_Squad == Squad.Red)
            objectRenderer.material.color *= Color.red;
    }
}
