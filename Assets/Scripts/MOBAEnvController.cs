using System;
using System.Linq;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using Unity.MLAgents.SideChannels;

public class MOBAEnvController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public MOBAAgent Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }


    /// <summary>
    /// Max Academy steps before this platform resets
    /// </summary>
    [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;

    /// <summary>
    /// The negative return penalty when ending in a draw given to both teams
    /// </summary>
    [Tooltip("Draw Penalty")] public float DrawPenalty = 0.5f;
    
    // TODO: cleanup these <summary> tags
    /// <summary>
    /// The area bounds.
    /// </summary>

    /// <summary>
    /// We will be changing the ground material based on success/failue
    /// </summary>

    // List of Agents On Platform
    public List<PlayerInfo> AgentsList = new();

    [HideInInspector]
    public List<bool> BlueStatus;
    [HideInInspector]
    public List<bool> RedStatus;

    [HideInInspector]
    // TODO: Should this be public?
    public HashSet<GameObject> projs = new();

    private MOBASettings m_MOBASettings;
    EnvConfigurationChannel m_EnvConfigurationChannel;

    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_RedAgentGroup;

    private int m_ResetTimer;

    void Awake()
    {
        m_MOBASettings = FindObjectOfType<MOBASettings>();
        m_EnvConfigurationChannel = new();
        Application.logMessageReceived += m_EnvConfigurationChannel.SendDebugStatementToPython;
        m_EnvConfigurationChannel.m_MOBASettings = m_MOBASettings;
        m_EnvConfigurationChannel.m_MOBAEnvController = this;
        SideChannelManager.RegisterSideChannel(m_EnvConfigurationChannel);
        Debug.Log("EnvConfigurationChannel is set up.");
    }

    void Start()
    {
        m_MOBASettings = FindObjectOfType<MOBASettings>();
        // Initialize TeamManager
        m_BlueAgentGroup = new();
        m_RedAgentGroup = new();
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            if (item.Agent.team == Squad.Blue)
            {
                m_BlueAgentGroup.RegisterAgent(item.Agent);
                BlueStatus.Add(true);
            }
            else
            {
                m_RedAgentGroup.RegisterAgent(item.Agent);
                RedStatus.Add(true);
            }
        }
        ResetScene();
    }

    public void OnDestroy()
    {
        Application.logMessageReceived -= m_EnvConfigurationChannel.SendDebugStatementToPython;
        if (Academy.IsInitialized)
            SideChannelManager.UnregisterSideChannel(m_EnvConfigurationChannel);
    }

    void FixedUpdate()
    {
        m_ResetTimer += 1;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            int bAlive = 0;
            foreach (bool b in BlueStatus)
                if (b) bAlive++;

            int rAlive = 0;
            foreach (bool r in RedStatus)
                if (r) rAlive++;

            // Small reward from 0-1 per agent for partial wins (max 1 for total win)
            if (bAlive != rAlive) {
                if (bAlive > rAlive) {
                    float ratio = ((float)(bAlive - rAlive)) / BlueStatus.Count;
                    m_BlueAgentGroup.AddGroupReward(ratio);
                    m_RedAgentGroup.AddGroupReward(-ratio);
                } else {
                    float ratio = ((float)(rAlive - bAlive)) / RedStatus.Count;
                    m_BlueAgentGroup.AddGroupReward(-ratio);
                    m_RedAgentGroup.AddGroupReward(ratio);
                }
                m_RedAgentGroup.EndGroupEpisode();
                m_BlueAgentGroup.EndGroupEpisode();
            } else {
                m_BlueAgentGroup.AddGroupReward(-DrawPenalty);
                m_RedAgentGroup.AddGroupReward(-DrawPenalty);

                m_BlueAgentGroup.GroupEpisodeInterrupted();
                m_RedAgentGroup.GroupEpisodeInterrupted();
            }
            ResetScene();
        }
    }
    
    public float[] RequestObservations(MOBAAgent agent) 
    {
        float[] obs = new float[BlueStatus.Count * 2 + RedStatus.Count * 2];

        if ((agent.team == Squad.Blue && !BlueStatus[(int)agent.role]) ||
            (agent.team == Squad.Red && !RedStatus[(int)agent.role])) {
            Array.Fill(obs, -1f);
            return obs;
        }

        void FillObs(List<bool> statusList, int teamOffset, int obsOffset) {
            for (int i = 0; i < statusList.Count; i++) {
                int agentIndex = teamOffset + i;
                int obsIndex = obsOffset + 2 * i;

                obs[obsIndex] = -1f;
                obs[obsIndex + 1] = -1f;

                if (statusList[i]) {
                    float hp = AgentsList[agentIndex].Agent.m_Hp;
                    float cooldown = AgentsList[i].Agent.abilityTimestamp - Time.time;
                    if (cooldown < 0) cooldown = 0f;
                    obs[obsIndex] = hp;
                    obs[obsIndex + 1] = cooldown;
                }
            }
        }

        if (agent.team == Squad.Blue) {
            FillObs(BlueStatus, 0, 0);
            FillObs(RedStatus, BlueStatus.Count, BlueStatus.Count * 2);
        } else {
            FillObs(BlueStatus, 0, RedStatus.Count * 2);
            FillObs(RedStatus, BlueStatus.Count, 0);
        }

        return obs;
    }

    public void RegisterProjectile(GameObject proj) {
        projs.Add(proj);
    }

    public void DegisterProjectile(GameObject proj) {
        projs.Remove(proj);
    }

    public void AgentDied(MOBAAgent agent) 
    {
        // Can add reward/penalty here
        if (agent.team == Squad.Blue) {
            BlueStatus[(int)agent.role] = false;
        } else {
            RedStatus[(int)agent.role] = false;
        }

        if (BlueStatus.All(s => !s))
            Victory(Squad.Red);
        if (RedStatus.All(s => !s))
            Victory(Squad.Blue);
    }


    public void Victory(Squad winningTeam)
    {
        // Big reward from 1-2 per agent if total win based on timing
        if (winningTeam == Squad.Blue)
        {
            m_BlueAgentGroup.AddGroupReward(2 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_RedAgentGroup.AddGroupReward(-1);
        }
        else
        {
            m_RedAgentGroup.AddGroupReward(2 - (float)m_ResetTimer / MaxEnvironmentSteps);
            m_BlueAgentGroup.AddGroupReward(-1);
        }
        m_RedAgentGroup.EndGroupEpisode();
        m_BlueAgentGroup.EndGroupEpisode();
        ResetScene();
    }


    public void ResetScene()
    {
        m_ResetTimer = 0;

        // Reset Projectiles
        foreach (var proj in projs.ToList())
        {
            proj.SetActive(false);
            Destroy(proj);
        }

        // Reset Deaths
        for (int i = 0; i < BlueStatus.Count; i++)
            BlueStatus[i] = true;
        for (int i = 0; i < RedStatus.Count; i++)
            RedStatus[i] = true;

        //Reset Agents
        foreach (var item in AgentsList)
        {
            var randomPosX = UnityEngine.Random.Range(-m_MOBASettings.positionRange, m_MOBASettings.positionRange);
            var randomPosZ = UnityEngine.Random.Range(-m_MOBASettings.positionRange, m_MOBASettings.positionRange);
            var newStartPos = item.Agent.initialPos + new Vector3(randomPosX, 0f, randomPosZ);
            var rot = item.Agent.rotSign * 180.0f + UnityEngine.Random.Range(-m_MOBASettings.angleRange, m_MOBASettings.angleRange);
            var newRot = Quaternion.Euler(0, rot, 0);
            item.Agent.transform.SetPositionAndRotation(newStartPos, newRot);

            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
            item.Agent.Reset();
        }
    }
}
