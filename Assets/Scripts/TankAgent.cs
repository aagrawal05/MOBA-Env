using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class TankAgent : MOBAAgent
{
    public GameObject swingPrefab;
    public Transform shoot;
    
    const float empoweredPowerCoef = 2.0f;
    const float empoweredRegenCoef = 1.2f;
    const float empoweredMoveSpeedCoef = 1.25f;
    const float empoweredAtkSpeedCoef = 1.0f;
    
    // TODO: Status System and Computed Stats in MOBAAgent => Not exclusive to TankAgent
    public override void FixedUpdate() {
        base.FixedUpdate();
        if ((this.m_Status & Status.Empowered) != 0 && 
            Time.time >= abilityEndTimestamp)
            EndEmpowered();
    }

    public override void Initialize() {
        base.Initialize();
        this.role = Role.Tank;
        this.m_baseHp *= 1.5f;
        this.m_Hp = m_baseHp;
        this.m_Power *= 1.2f;
        this.m_Armor *= 1.3f;
        this.m_AbilityCd *= 3.0f;
        this.m_AbilityDuration = 12.0f;
    }

    public override void AtkBehaviour() {
        GameObject atk = Instantiate(swingPrefab, shoot.position, shoot.rotation);
        Projectile proj = atk.GetComponent<Projectile>();
        proj.Initialize(this.m_Power, this.team);
    }

    public override void AbilityBehaviour() {
        abilityEndTimestamp = Time.time + this.m_AbilityDuration;
        this.m_Power *= empoweredPowerCoef;
        this.m_Regen *= empoweredRegenCoef;
        this.m_MoveSpeed *= empoweredMoveSpeedCoef;
        this.m_AtkSpeed *= empoweredAtkSpeedCoef;
        this.m_Status |= Status.Empowered;
    }

    public override void Reset() {
        base.Reset();
        if ((this.m_Status & Status.Empowered) != 0)
            EndEmpowered();
    }

    private void EndEmpowered() {
        this.m_Power /= empoweredPowerCoef;
        this.m_Regen /= empoweredRegenCoef;
        this.m_MoveSpeed /= empoweredMoveSpeedCoef;
        this.m_AtkSpeed /= empoweredAtkSpeedCoef;
        this.m_Status &= ~Status.Empowered;
    }
}
