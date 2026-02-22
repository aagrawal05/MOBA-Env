using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{   
    const float basePowerScaling = 1.0f;
    const float baseSpeed = 3.00f;
    const bool basePiercing = false;
    const bool baseHealing = false;
    const bool baseMelee = false;
    const float baseDespawnTime = -1.0f; // Doesn't despawn

    public float m_PowerScaling = basePowerScaling;
    public float m_Speed = baseSpeed;
    public bool m_Piercing = basePiercing;
    public bool m_Healing = baseHealing;
    public bool m_Melee = baseMelee;
    public float m_DespawnTime = baseDespawnTime;

    public string baseTagName;
    [HideInInspector]
    public float m_Power;
    [HideInInspector]
    public Squad m_Squad;
    [HideInInspector]
    public float despawnTimestamp;

    public void Initialize(float power, Squad squad)
    {
        m_Power = power * m_PowerScaling;
        m_Squad = squad;
        m_Piercing |= m_Melee; // Melee implicitly means piercing and AOE
        despawnTimestamp = Time.time + m_DespawnTime;

        var envController = FindObjectOfType<MOBAEnvController>();
        envController.RegisterProjectile(gameObject);

        if (m_Squad == Squad.Blue)
            gameObject.tag = "blue" + baseTagName;
        else if (m_Squad == Squad.Red)
            gameObject.tag = "red" + baseTagName;
        else
            gameObject.tag = baseTagName;
    }
        
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);

        if (m_DespawnTime > 0 && Time.time >= despawnTimestamp)
            Destroy(gameObject);
    }

    static readonly HashSet<string> redTags = new() { "redSupport", "redDamage", "redTank" };
    static readonly HashSet<string> blueTags = new() { "blueSupport", "blueDamage", "blueTank" };

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("wall")) {
            Destroy(gameObject);
            return;
        }

        void HandleCollision(HashSet<string> validTags, Squad opposingSquad) {
            if (validTags.Contains(col.tag) && (m_Squad == opposingSquad || m_Squad == Squad.Neutral)) {
                var otherAgent = col.GetComponent<MOBAAgent>();
                if (m_Healing)
                    otherAgent.HealHealth(m_Power);
                else
                    otherAgent.TakeDamage(m_Power);
                if (!m_Piercing)
                    Destroy(gameObject);
            }
        }

        HandleCollision(redTags, Squad.Blue);
        HandleCollision(blueTags, Squad.Red);
    }

    void OnDestroy()
    {
        var envController = FindObjectOfType<MOBAEnvController>();
        envController.DegisterProjectile(gameObject);
    }
}
