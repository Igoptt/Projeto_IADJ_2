using System;
using System.Linq;
using Assets.EOTS;
using Assets.General_Scripts;
using Assets.Scripts.SteeringBehaviours.Basics;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.GameData;
using TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;

namespace TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Actions
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
            //capturar a base mais proxima
            //segui o exemplo do AttackPlayer
            //Teams é um enum -> BlueTeam, RedTeam e Neutral
            //verifica de que equipa é o soldado e compara com a equipa da base (se é da blue, red, ou nenhuma(neutra))
            //getClosest devolve true se estiver na lista e coloca na variavel out 
            //out -> guarda o resultado
            print(_soldier.MyTeam);
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