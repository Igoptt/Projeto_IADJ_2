using UnityEngine;

namespace Assets.TeamBlue.Pathfinding.Scripts.AStar
{
    public interface IPathfindingUnit
    {
        void SetTarget(Transform transform);
        bool DoFollowPathStep();

    }
}