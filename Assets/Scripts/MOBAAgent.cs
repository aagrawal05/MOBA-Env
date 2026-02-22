using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

public enum Squad
{
    Blue,
    Red,
    Neutral
}

// public enum Role {
//     Tank,
//     Damage,
//     Support
// }

// public enum Role {
//     Damage,
//     Support,
//     Tank
// }

public enum Role {
    Support,
    Damage,
    Tank
}

[Flags]
public enum Status {
    None = 0,
    Empowered = 1 << 0,
    /* Stunned = 1 << 2, */
    /* Slowed = 1 << 3, */
    /* Poisoned = 1 << 4, */
}

public class MOBAAgent : Agent
{
    // Note that that the detectable tags are different for the blue and red teams. The order is
    // * wall
    // * arrow
    // * bigArrow
    // * heal
    // * allySupport
    // * allyDamage
    // * allyTank
    // * opposingSupport
    // * opposingDamage
    // * opposingTank

    public Transform tomb;

    const float baseHp = 100.0f;
    const float baseMoveSpeed = 1.0f;
    const float baseLateralMSCoef = 1.0f;
    const float baseArmor = 0.2f;
    const float basePower = 10.0f;
    const float baseAtkSpeed = 0.35f;
    const float baseRegen = 0.5f;
    const float baseAbilityCd = 10.0f;
    const float baseAbilityDuration = 0.0f;

    [HideInInspector]
    public Squad team;
    [HideInInspector]
    public Role role;

    float m_Existential;
    public bool m_Died = false;

    [HideInInspector]
    public float m_baseHp;
    [HideInInspector]
    public float m_Hp;
    [HideInInspector]
    public float m_MoveSpeed;
    [HideInInspector]
    public float m_LateralMSCoef;
    [HideInInspector]
    public float m_Armor;
    [HideInInspector]
    public float m_Power;
    [HideInInspector]
    public float m_AtkSpeed;
    [HideInInspector]
    public float m_Regen;
    [HideInInspector]
    public float m_AbilityCd;
    [HideInInspector]
    public float m_AbilityDuration;

    public Status m_Status;

    MOBASettings m_MOBASettings;
    MOBAEnvController envController;
    EnvironmentParameters m_ResetParams;
    BehaviorParameters m_BehaviorParameters;

    [HideInInspector]
    public Rigidbody agentRb;
    [HideInInspector]
    public Vector3 initialPos;
    [HideInInspector]
    public float rotSign;


    [HideInInspector]
    public float atkTimestamp = 0f;
    [HideInInspector]
    public float abilityTimestamp = 0f;
    [HideInInspector]
    public float abilityEndTimestamp;
    

    public virtual void FixedUpdate() {
        if (!m_Died) {
            m_Hp += m_Regen * Time.deltaTime;
            if (m_Hp > m_baseHp) m_Hp = m_baseHp;
        }
    }
        
    public override void Initialize()
    {
        envController = GetComponentInParent<MOBAEnvController>();
        if (envController != null)
        {
            m_Existential = 1f / envController.MaxEnvironmentSteps;
        }
        else
        {
            m_Existential = 1f / MaxStep;
        }

        m_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        if (m_BehaviorParameters.TeamId == (int)Squad.Blue)
        {
            team = Squad.Blue;
            rotSign = 0f;
        }
        else
        {
            team = Squad.Red;
            rotSign = 1f;
        }

        m_baseHp = baseHp;
        m_Hp = baseHp;
        m_MoveSpeed = baseMoveSpeed;
        m_LateralMSCoef = baseLateralMSCoef;
        m_Armor = baseArmor;
        m_Power = basePower;
        m_AtkSpeed = baseAtkSpeed;
        m_Regen = baseRegen;
        m_AbilityCd = baseAbilityCd;
        m_AbilityDuration = baseAbilityDuration;

        m_Status = Status.None;

        m_MOBASettings = FindObjectOfType<MOBASettings>();
        initialPos = transform.position;
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;

        m_ResetParams = Academy.Instance.EnvironmentParameters;

        // Children override to set their computed stats from parent class
        // Set the role enum to the specific type in overriden initialize as well
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var forwardAxis = act[0];
        var rightAxis = act[1];
        var rotateAxis = act[2];

        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward * m_MoveSpeed;
                break;
            case 2:
                dirToGo = transform.forward * -m_MoveSpeed;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                dirToGo = transform.right * m_MoveSpeed * m_LateralMSCoef;
                break;
            case 2:
                dirToGo = transform.right * -m_MoveSpeed * m_LateralMSCoef;
                break;
        }

        switch (rotateAxis)
        {
            case 1:
                rotateDir = transform.up * -1f;
                break;
            case 2:
                rotateDir = transform.up * 1f;
                break;
        }

        transform.Rotate(rotateDir, Time.deltaTime * 100f);
        agentRb.AddForce(dirToGo, ForceMode.VelocityChange);
    }


    public void AtkAgent(ActionSegment<int> act)
    {
        var atkAxis = act[3];
        if (atkAxis == 1) {
            if (Time.time >= atkTimestamp) {
                AtkBehaviour();
                atkTimestamp = Time.time + 1.0f/m_AtkSpeed;
            }
        }
    }

    public void AbilityAgent(ActionSegment<int> act)
    {
        var abilityAxis = act[4];
        if (abilityAxis == 1) {
            if (Time.time >= abilityTimestamp) {
                AbilityBehaviour();
                abilityTimestamp = Time.time + m_AbilityCd;
            }
        }
    }

    public virtual void AtkBehaviour() {
        Debug.Log("ATTACKED!");
    }

    public virtual void AbilityBehaviour() {
        Debug.Log("ABILITY ACTIVATED!");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // DEBUG
        /* RayPerceptionSensorComponent3D[] raySensors = GetComponentsInChildren<RayPerceptionSensorComponent3D>(); */
        /* Debug.Log($"Forward Raycast: {raySensors[0].RaySensor.GetObservationSpec().Shape}"); */
        /* Debug.Log($"Backwards Raycast: {raySensors[1].RaySensor.GetObservationSpec().Shape}"); */
        /* Debug.Log($"Extra: {sensor.GetObservationSpec().Shape}"); */
        // DEBUG

        float[] observations = envController.RequestObservations(this);
        foreach (var o in observations)
            sensor.AddObservation(o);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (!m_Died) {
            MoveAgent(actionBuffers.DiscreteActions);
            AtkAgent(actionBuffers.DiscreteActions);
            AbilityAgent(actionBuffers.DiscreteActions);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        //forward
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        //rotate
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[2] = 2;
        }
        //right
        if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[1] = 2;
        }

        if (Input.GetKey(KeyCode.X))
        {
            discreteActionsOut[3] = 1;
        }

        if(Input.GetKey(KeyCode.Space)) {
            discreteActionsOut[4] = 1;
        }
    }

    public virtual void Reset() {
        m_Died = false;
        m_Hp = m_baseHp;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | 
                                                           RigidbodyConstraints.FreezeRotation;
        atkTimestamp = 0f;
        abilityTimestamp = 0f;
    }

    public override void OnEpisodeBegin()
    {
        //Reset Any Parameters
    }

    public void TakeDamage(float damage) {
        /* AddReward(-0.05f); */
        m_Hp -= damage / m_Armor;
        if (m_Hp <= 0) {
            m_Died = true;
            gameObject.transform.position = tomb.position;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | 
                                                               RigidbodyConstraints.FreezeRotation;
            envController.AgentDied(this);
        }
    }

    public void HealHealth(float health) {
        /* AddReward(0.05f); */
        m_Hp += health;
        if (m_Hp > m_baseHp) m_Hp = m_baseHp;
    }
}
