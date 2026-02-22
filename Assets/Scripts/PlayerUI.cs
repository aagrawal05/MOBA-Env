using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image hpBar;
    public Image cooldown;

    [HideInInspector]
    private Light m_Light;
    [HideInInspector]
    private MOBAAgent m_Agent;

    // Empowered UI
    [HideInInspector]
    private float blinkTimer = 0f;
    [HideInInspector]
    private float blinkPeriod = 0.5f;
    [HideInInspector]
    private Renderer objectRenderer;
    [HideInInspector]
    private Color originalColor;

    void Start()
    {
        m_Light = GetComponent<Light>();
        m_Agent = GetComponentInParent<MOBAAgent>();
    
        if (m_Agent.team == Squad.Blue)
            m_Light.color *= Color.blue;
        else if (m_Agent.team == Squad.Red)
            m_Light.color *= Color.red;

        objectRenderer = GetComponent<Renderer>();
        originalColor = objectRenderer.material.color;
    }

    void FixedUpdate()
    {
        hpBar.fillAmount = m_Agent.m_Hp / m_Agent.m_baseHp;
        var delta = m_Agent.abilityTimestamp - Time.time;
        if (delta < 0) {
            cooldown.fillAmount = 1.0f;
            cooldown.color = Color.green;
        } else {
            cooldown.fillAmount = 1.0f - delta/m_Agent.m_AbilityCd;
            cooldown.color = Color.white;
        }

        if ((m_Agent.m_Status & Status.Empowered) != 0) {
            blinkTimer += Time.deltaTime;
            var progress = Mathf.PingPong(blinkTimer / blinkPeriod, 0.5f);
            Color currentColor = Color.Lerp(originalColor, Color.red, progress);
            objectRenderer.material.color = currentColor;
            if (Time.time >= m_Agent.abilityEndTimestamp)
                objectRenderer.material.color = originalColor;
        }
    }

    // TODO: Incorporate indirectly with status system
    void Reset() {
        objectRenderer.material.color = originalColor;
    }
}
