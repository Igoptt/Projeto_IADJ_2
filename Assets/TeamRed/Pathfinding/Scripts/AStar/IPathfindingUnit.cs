using UnityEngine;

namespace Assets.TeamRed.Pathfinding.Scripts.AStar
{
    public interface IPathfindingUnit
    {
        void SetTarget(Transform transform);
        bool DoFollowPathStep();

    }
}