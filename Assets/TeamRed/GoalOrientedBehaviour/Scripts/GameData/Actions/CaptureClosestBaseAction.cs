using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.EOTS;
using Assets.General_Scripts;
using Assets.Scripts.SteeringBehaviours.Basics;
using Assets.TeamRed.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.TeamRed.GoalOrientedBehaviour.Scripts.GameData;
using Assets.TeamRed.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;

namespace TeamRed.GoalOrientedBehaviour.Scripts.GameData.Actions
{
    public class CaptureClosestBaseAction : GoapAction
    {
        private Base _captureBase;
        private ISoldier _soldier;
            
        private void Awake()
        {
            _soldier = GetComponent<Soldier>();
            AddEffect("captureBaseAction", true);

        }

        public override void Reset()
        {
            _captureBase = null;
        }

        public override bool IsDone()
        {
            return _captureBase.MyTeam == _soldier.MyTeam;
        }

        public override bool CheckProceduralPrecondition(GameObject agent)
        {
            if (Utils.GetClosest(FindObjectsOfType<Base>().
                Where(b => _soldier.MyTeam == Teams.RedTeam && (b.MyTeam == Teams.BlueTeam || b.MyTeam == Teams.Neutral) || 
                           _soldier.MyTeam == Teams.BlueTeam && (b.MyTeam == Teams.RedTeam || b.MyTeam == Teams.Neutral )), _soldier.MyTransform, out _captureBase))
                    
            {
                print(_captureBase);
                Target = _captureBase.gameObject;
                print(Target);
                return true;
            }

            return false;    
        }

        public override bool Perform(GameObject agent)
        {
            //para quando captura for now
            _soldier.MyTransform.GetComponent<SteeringBasics>().Stop();
            return true;    
        }

        public override bool RequiresInRange()
        {
            return true; // must be in range para capturar a base
        }
    }
}

