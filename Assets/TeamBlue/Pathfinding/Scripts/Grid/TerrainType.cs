using System;
using UnityEngine;

namespace Assets.TeamBlue.Pathfinding.Scripts.Grid
{
    [Serializable]
    public class TerrainType
    {
        /// <summary>
        /// The layer of this terrain
        /// </summary>
        [Tooltip("The layer of this terrain")]
        public LayerMask TerrainMask;
        /// <summary>
        /// The penalty cost of this terrain
        /// </summary>
        [Tooltip("The penalty cost of this terrain")]
        public int TerrainPenalty;
    }
}