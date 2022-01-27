using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.EOTS;
using Assets.General_Scripts;
using Assets.Scripts.SteeringBehaviours.Basics;
using Assets.TeamBlue;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.TeamBlue.Pathfinding;
using Assets.TeamBlue.Pathfinding.Scripts.AStar;
using UnityEngine;

namespace TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Soldiers
{
    public class BaseGuards : Soldier
    {
        public new void SetGoal()
        {
            _soldierGoal.Clear();
            _soldierGoal.Add(new KeyValuePair<string, object>(_teamManager.GetBaseGuardGoal(this),true));
        }
    }
}