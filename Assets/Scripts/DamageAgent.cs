using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class DamageAgent : MOBAAgent
{

    public GameObject arrowPrefab;
    public GameObject bigArrowPrefab;
    public Transform shoot;

    public override void Initialize() {
        base.Initialize();
        this.role = Role.Damage;
        this.m_Power *= 1.5f;
        this.m_AbilityCd *= 2.0f;
    }

    public override void AtkBehaviour() {
        GameObject atk = Instantiate(arrowPrefab, shoot.position, shoot.rotation);
        Projectile proj = atk.GetComponent<Projectile>();
        proj.Initialize(this.m_Power, this.team);
        /* envController.RegisterAtk(atk); */
    }

    public override void AbilityBehaviour() {
        GameObject bigAtk = Instantiate(bigArrowPrefab, shoot.position, shoot.rotation);
        Projectile proj = bigAtk.GetComponent<Projectile>();
        proj.Initialize(this.m_Power, this.team);
        /* envController.RegisterAtk(bigAtk); */
    }
}
