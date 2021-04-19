#region Copyright © ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

// Microsoft Reciprocal License (Ms-RL)
//
// This license governs use of the accompanying software. If you use the software, you accept this
// license. If you do not accept the license, do not use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same
// meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// copyright license to reproduce its contribution, prepare derivative works of its contribution,
// and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and
// limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free
// license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or
// otherwise dispose of its contribution in the software or derivative works of the contribution in
// the software.
//
// 3. Conditions and Limitations
// (A) Reciprocal Grants- For any file you distribute that contains code from the software (in
// source code or binary format), you must provide recipients the source code to that file along
// with a copy of this license, which license will govern that file. You may license other files
// that are entirely your own work and do not contain code from the software under any terms you
// choose.
// (B) No Trademark License- This license does not grant you rights to use any contributors' name,
// logo, or trademarks.
// (C) If you bring a patent claim against any contributor over patents that you claim are
// infringed by the software, your patent license from such contributor to the software ends
// automatically.
// (D) If you distribute any portion of the software, you must retain all copyright, patent,
// trademark, and attribution notices that are present in the software.
// (E) If you distribute any portion of the software in source code form, you may do so only under
// this license by including a complete copy of this license with your distribution. If you
// distribute any portion of the software in compiled or object code form, you may only do so under
// a license that complies with this license.
// (F) The software is licensed "as-is." You bear the risk of using it. The contributors give no
// express warranties, guarantees or conditions. You may have additional consumer rights under your
// local laws which this license cannot change. To the extent permitted under your local laws, the
// contributors exclude the implied warranties of merchantability, fitness for a particular purpose
// and non-infringement.

#endregion Copyright © ThotLab Games 2011. Licensed under the terms of the Microsoft Reciprocal Licence (Ms-RL).

namespace GameBrains.AI
{
    using System.Collections.Generic;
    
    using UnityEngine;

    /// <summary>
    /// Sensory memory keeps track of opponents detected by sound or sight.
    /// </summary>
    /// <remarks>
    /// TODO: how about add smell or tracking if crossing recent path.
    /// </remarks>
    public class SensoryMemory
    {
        /// <summary>
        /// Initializes a new instance of the SensoryMemory class.
        /// </summary>
        /// <param name="owner">The agent that owns this sensory memory.</param>
        /// <param name="memorySpan">How soon we forget.</param>
        public SensoryMemory(Agent agent, float memorySpan)
        {
            Agent = agent;
            MemorySpan = memorySpan;
            MemoryMap = new Dictionary<Agent, SensoryMemoryRecord>();
        }

        /// <summary>
        /// Gets the owner of this instance.
        /// </summary>
        public Agent Agent { get; private set; }

        /// <summary>
        /// Gets the container is used to simulate memory of sensory events. A  record is created
        /// for each opponent in the environment. Each record is updated whenever the opponent is
        /// encountered (i.e., whenever it is seen or heard).
        /// </summary>
        public Dictionary<Agent, SensoryMemoryRecord> MemoryMap { get; private set; }

        /// <summary>
        /// Gets the agent's memory span. When an agent requests a list of all recently sensed opponents
        /// this value is used to determine if the agent is able to remember an opponent or not.
        /// </summary>
        public float MemorySpan { get; private set; }

        /// <summary>
        /// Remove an agent's record from memory.
        /// </summary>
        /// <param name="agent">The agent whose record is to be removed.</param>
        public void RemoveAgentFromMemory(Agent agent)
        {
            MemoryMap.Remove(agent);
        }

        /// <summary>
        /// Update the record for an individual opponent.
        /// <remarks>
        /// Note, there is no need to test if the opponent is within the FOV because that test will
        /// be done when the <see cref="UpdateVision"/>method is called.
        ///  </remarks>
        /// </summary>
        /// <param name="noiseMaker">The agent that made the sound.</param>
        public void UpdateWithSoundSource(Agent noiseMaker)
        {
            // make sure the agent being examined is not this agent
            if (Agent == noiseMaker || noiseMaker.SameTeam(Agent))
            {
                return;
            }

            // if the agent is already part of the memory then update its data,
            // else create a new memory record and add it to the memory
            MakeNewRecordIfNotAlreadyPresent(noiseMaker);

            SensoryMemoryRecord info = MemoryMap[noiseMaker];

            // test if there is LOS between agents 
            if (Agent.HasLineOfSight(noiseMaker.Kinematic.Position))
            {
                info.IsShootable = true;

                // record the position of the agent
                info.LastSensedPosition = noiseMaker.Kinematic.Position;
            }
            else
            {
                info.IsShootable = false;
            }

            // record the time it was sensed
            info.TimeLastSensed = Time.time;
        }

        /// <summary>
        /// This method iterates through all the agents in the game world to test if they are in the
        /// field of view. Each agents's memory record is updated accordingly.
        /// </summary>
        public void UpdateVision()
        {
            // for each agent in the world test to see if it is visible to the
            // agent of this class
            List<Agent> agents = EntityManager.FindAll<Agent>();
            foreach (Agent agent in agents)
            {
                // make sure the agent being examined is not this agents
                if (Agent == agent || agent.SameTeam(Agent))
                {
                    continue;
                }

                // make sure it is part of the memory map
                MakeNewRecordIfNotAlreadyPresent(agent);

                // get a reference to this agent's data
                SensoryMemoryRecord info = MemoryMap[agent];

                if (Agent.HasLineOfSight(agent.Kinematic.Position))
                {
                    info.IsShootable = true;

                    if (Agent.IsTargetInFieldOfView(agent.Kinematic.Position))
                    {
                        info.TimeLastSensed = Time.time;
                        info.LastSensedPosition = agent.Kinematic.Position;
                        info.TimeLastVisible = Time.time;

                        if (info.IsWithinFieldOfView == false)
                        {
                            info.IsWithinFieldOfView = true;
                            info.TimeBecameVisible = info.TimeLastSensed;
                        }
                    }
                    else
                    {
                        info.IsWithinFieldOfView = false;
                    }
                }
                else
                {
                    info.IsShootable = false;
                    info.IsWithinFieldOfView = false;
                }
            }
        }

        /// <summary>
        /// Gets the list of recently sensed opponents.
        /// </summary>
        /// <returns>A list of the agents that have been sensed recently.</returns>
        public List<Agent> GetListOfRecentlySensedOpponents()
        {
            // this will store all the opponents the agent can remember
            var opponents = new List<Agent>();

            float currentTime = Time.time;
            foreach (KeyValuePair<Agent, SensoryMemoryRecord> kvp in MemoryMap)
            {
                // if this agent has been updated in the memory recently, add to list
                if ((currentTime - kvp.Value.TimeLastSensed) <= MemorySpan)
                {
                    opponents.Add(kvp.Key);
                }
            }

            return opponents;
        }

        /// <summary>
        /// Tests if opponent is shootable.
        /// </summary>
        /// <param name="opponent">The opponent.</param>
        /// <returns>
        /// True if opponent can be shot (i.e. its not obscured by walls).
        /// </returns>
        public bool IsOpponentShootable(Agent opponent)
        {
            if (opponent != null && MemoryMap.ContainsKey(opponent))
            {
                return MemoryMap[opponent].IsShootable;
            }

            return false;
        }

        /// <summary>
        /// Tests if opponent within FOV.
        /// </summary>
        /// <param name="opponent">The opponent.</param>
        /// <returns>True if opponent is within FOV.</returns>
        public bool IsOpponentWithinFieldOfView(Agent opponent)
        {
            if (opponent != null && MemoryMap.ContainsKey(opponent))
            {
                return MemoryMap[opponent].IsWithinFieldOfView;
            }

            return false;
        }

        /// <summary>
        /// Gets the last recorded position of opponent.
        /// </summary>
        /// <param name="opponent">The opponent.</param>
        /// <returns>The last recorded position of opponent.</returns>
        /// <exception cref="System.Exception">
        /// Attempting to get position of unrecorded agent.
        /// </exception>
        public Vector2 GetLastRecordedPosition(Agent opponent)
        {
            if (opponent != null && MemoryMap.ContainsKey(opponent))
            {
                return MemoryMap[opponent].LastSensedPosition;
            }

            throw new System.Exception(
                "SensoryMemory.GetLastRecordedPosition: Attempting to get position of unrecorded agent.");
        }

        /// <summary>
        /// Gets the time opponent has been visible.
        /// </summary>
        /// <param name="opponent">The opponent.</param>
        /// <returns>The amount of time opponent has been visible.</returns>
        public float GetTimeVisible(Agent opponent)
        {
            if (opponent != null && MemoryMap.ContainsKey(opponent) && 
                MemoryMap[opponent].IsWithinFieldOfView)
            {
                return Time.time - MemoryMap[opponent].TimeBecameVisible;
            }

            return 0;
        }

        /// <summary>
        /// Gets the time opponent has been out of view.
        /// </summary>
        /// <param name="opponent">The opponent.</param>
        /// <returns>
        /// The amount of time the given opponent has remained out of view (or a high value if
        /// opponent has never been seen or not present).
        /// </returns>
        public float GetTimeOutOfView(Agent opponent)
        {
            if (opponent != null && MemoryMap.ContainsKey(opponent))
            {
                return Time.time - MemoryMap[opponent].TimeLastVisible;
            }

            return float.MaxValue;
        }

        /// <summary>
        /// Get the time since opponent was last sensed.
        /// </summary>
        /// <param name="opponent">The opponent.</param>
        /// <returns>The amount of time opponent has been visible.</returns>
        public float GetTimeSinceLastSensed(Agent opponent)
        {
            if (opponent != null && MemoryMap.ContainsKey(opponent) &&
                MemoryMap[opponent].IsWithinFieldOfView)
            {
                return Time.time - MemoryMap[opponent].TimeLastSensed;
            }

            return 0;
        }

        /// <summary>
        /// Check to see if there is an existing record for the opponent. If not a new record is
        /// made and added to the memory map.
        ///  </summary> 
        /// <remarks>
        /// Called by <see cref="UpdateWithSoundSource"/> and <see cref="UpdateVision"/>.
        /// </remarks>
        /// <param name="opponent">The opponent.</param>
        private void MakeNewRecordIfNotAlreadyPresent(Agent opponent)
        {
            // check to see if this Opponent already exists in the memory. If it doesn't,
            // create a new record
            if (!MemoryMap.ContainsKey(opponent))
            {
                MemoryMap[opponent] = new SensoryMemoryRecord();
            }
        }
    }
}