using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class SupportAgent : MOBAAgent
{

    public GameObject healPrefab;
    public Transform shoot;

    public override void Initialize() {
        base.Initialize();
        this.role = Role.Support;
        this.m_MoveSpeed *= 1.15f;
        this.m_Power *= 0.8f;
        this.m_Armor *= 0.8f;
        this.m_AbilityCd *= 6.0f;
    }

    public override void AtkBehaviour() {
        GameObject heal = Instantiate(healPrefab, shoot.position, shoot.rotation);
        Projectile proj = heal.GetComponent<Projectile>();
        proj.Initialize(this.m_Power, Squad.Neutral);
        /* envController.RegisterAtk(heal); */
    }

    public override void AbilityBehaviour() {
        var envController = GetComponentInParent<MOBAEnvController>();
        foreach (var pi in envController.AgentsList) {
            if (pi.Agent.team == this.team)
                pi.Agent.HealHealth(m_Power * 5.0f);
        }
    }
}
