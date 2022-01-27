using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.EOTS;
using Assets.General_Scripts;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Actions;
using Assets.TeamBlue.Pathfinding;
using TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Actions;
using TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;
using UnityEngine.Diagnostics;
using Utils = Assets.TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Utils;

namespace Assets.TeamBlue
{
    public class TeamManager : MonoBehaviour
    {
        //wops nao estava a seguir as convençoes de nomes serem com letra minuscula
        public List<Soldier> myArmy;
        public List<Base> mapBases;
        public FlagComponent flag;
        public Teams myTeam;
        public Teams otherTeam;
        private float priorityCost = -5000f;
        private float normalCost = 1f;
        public List<ISoldier> _enemyArmy;

        // private ISoldier _closestEnemy;

        private void Awake()
        {
            flag = FindObjectOfType<FlagComponent>();
            myArmy = FindObjectsOfType<Soldier>().ToList();
            mapBases = FindObjectsOfType<Base>().ToList();
            myTeam = Teams.BlueTeam;
            //if my team = redTeam then otherTeam = Blue Team else contrario
            otherTeam = myTeam == Teams.RedTeam ? Teams.BlueTeam : Teams.RedTeam;
            
            //vai buscar todos os objetos do tipo MonoBehaviour (ou subclasses dele) para ter todos os scripts 
            //depois procura dos acima os que implementam a classe ISoldier cuja team seja a OtherTeam
            //source: https://answers.unity.com/questions/863509/how-can-i-find-all-objects-that-have-a-script-that.html
            _enemyArmy = FindObjectsOfType<MonoBehaviour>().OfType<ISoldier>().Where(s => s.MyTeam == otherTeam).ToList();
        }

        
        private void Start()
        {
            StartCoroutine(PeriodicPlanReset());
            // PeriodicPlanReset();
        }

        public bool BaseClose(Soldier soldier)
        {
            //vai base da equipa mais proxima
            if (Utils.GetClosest(FindObjectsOfType<Base>().Where(b => b.MyTeam == myTeam), soldier.MyTransform, out var mono)) ;
            {
                var _closestBase = mono.gameObject.GetComponent<Base>();
                //verifica se a base mais proxima esta a pelo menos 5x o range do attack (o range é 1.5f entao 5x é relativamente perto)
                return Vector3.Distance(soldier.MyTransform.position, _closestBase.gameObject.transform.position) < 1.5f * 5;
            }
        }

        public bool FlagClose(Soldier soldier)
        {
            var _closestFlag = FindObjectOfType<FlagComponent>();
            //verifica se a bandeira esta a pelo menos 5x o range do attack (o range é 1.5f entao 5x é relativamente perto)

            if (_closestFlag.CanBeCarried == false) return false;
            
            return Vector3.Distance(soldier.MyTransform.position, _closestFlag.transform.position) <= 1.5f * 3;
            
        }
        
        public bool LotsOfEnemiesClose(Soldier soldier)
        {
            var i = 0;
            foreach (ISoldier enemy in _enemyArmy)
            {
                if (Vector3.Distance(soldier.MyTransform.position, enemy.MyTransform.position) < 2.5f * 2)
                {
                    i++;
                }
            }

            return i>=2;
        }
        
        
        
        
        
        public string GetSoldierGoal(Soldier soldier)
        {
            var goal ="";
            if(soldier.GetComponent<AttackNearestEnemyAction>().OnCooldown() == false && Utils.EnemyClose(soldier, _enemyArmy))
            {
                soldier.GetComponent<AttackNearestEnemyAction>().Cost = priorityCost;
                soldier.GetComponent<GoapAgent>().AbortPlan();
                return "attackNearestEnemy";
            }
            
            if (soldier.GetComponent<SprintAction>().OnCooldown() == false)
            {
                soldier.GetComponent<SprintAction>().Cost = priorityCost;
                soldier.GetComponent<GoapAgent>().AbortPlan();
                return "ran";
            }
            
            if (LotsOfEnemiesClose(soldier) && soldier.GetComponent<GuardAction>() == false)
            {
                soldier.GetComponent<GuardAction>().Cost = priorityCost;
                soldier.GetComponent<GoapAgent>().AbortPlan();
                return "Invulnerable";
            }

            soldier.GetComponent<GuardAction>().Cost = normalCost;
            soldier.GetComponent<AttackNearestEnemyAction>().Cost = normalCost;
            soldier.GetComponent<SprintAction>().Cost = normalCost;

            goal = soldier.HasFlag ? "scored" : "captureBaseAction";
            
            return goal;
        }


        public string GetBaseGuardGoal(BaseGuards baseGuards)
        {
            var goal ="";
            if(baseGuards.GetComponent<AttackNearestEnemyAction>().OnCooldown() == false && Utils.EnemyClose(baseGuards, _enemyArmy))
            {
                baseGuards.GetComponent<AttackNearestEnemyAction>().Cost = priorityCost;
                baseGuards.GetComponent<GoapAgent>().AbortPlan();
                return "attackNearestEnemy";
            }
            
            if (baseGuards.GetComponent<SprintAction>().OnCooldown() == false)
            {
                baseGuards.GetComponent<SprintAction>().Cost = priorityCost;
                baseGuards.GetComponent<GoapAgent>().AbortPlan();
                return "ran";
            }
            if (LotsOfEnemiesClose(baseGuards) && baseGuards.GetComponent<GuardAction>() == false)
            {
                baseGuards.GetComponent<GuardAction>().Cost = priorityCost;
                baseGuards.GetComponent<GoapAgent>().AbortPlan();
                return "Invulnerable";
            }
            
            baseGuards.GetComponent<AttackNearestEnemyAction>().Cost = normalCost;
            baseGuards.GetComponent<SprintAction>().Cost = normalCost;
            baseGuards.GetComponent<GuardAction>().Cost = normalCost;
            
            goal = FlagClose(baseGuards) ? "scored" : "captureBaseAction";
            
            return goal;
        }
        
        public string GetFlagTeamGoal(FlagSquad flagSquad)
        {
            if(flagSquad.GetComponent<AttackNearestEnemyAction>().OnCooldown() == false && Utils.EnemyClose(flagSquad, _enemyArmy) && flagSquad.HasFlag == false)
            {
                flagSquad.GetComponent<AttackNearestEnemyAction>().Cost = priorityCost;
                flagSquad.GetComponent<GoapAgent>().AbortPlan();
                return "attackNearestEnemy";
            }
            
            if (flagSquad.GetComponent<SprintAction>().OnCooldown() == false)
            {
                flagSquad.GetComponent<SprintAction>().Cost = priorityCost;
                flagSquad.GetComponent<GoapAgent>().AbortPlan();
                return "ran";
            }
            if (LotsOfEnemiesClose(flagSquad) && flagSquad.GetComponent<GuardAction>() == false)
            {
                flagSquad.GetComponent<GuardAction>().Cost = priorityCost;
                flagSquad.GetComponent<GoapAgent>().AbortPlan();
                return "Invulnerable";
            }
            
            flagSquad.GetComponent<AttackNearestEnemyAction>().Cost = normalCost;
            flagSquad.GetComponent<SprintAction>().Cost = normalCost;
            flagSquad.GetComponent<GuardAction>().Cost = normalCost;
            var goal = flagSquad.HasFlag ? "scored" : "hasFlag";
            return goal;


        }

        
        private IEnumerator PeriodicPlanReset()
        {
            while (true)
            {
                StartCoroutine(ResetTeamPlans());
        
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        private IEnumerator ResetTeamPlans()
        {
            foreach (var soldier in myArmy)
            {
                ResetPlan(soldier.GetComponent<GoapAgent>());
                // soldier.GetComponent<GoapAgent>().AbortPlan();
            }

            yield return null;
        }
        
        private void ResetPlan(GoapAgent agent)
        {
            agent.AbortPlan();
            
        }
        
        public bool DoWeHaveOurBases(Teams ourTeam)
        {
            var i = 0;
            if (ourTeam == Teams.BlueTeam)
            {
                foreach (var @base in mapBases)
                {
                    if (IsBaseOurs(@base) && (@base.name == "NW" || @base.name == "NE"))
                    {
                        i++;
                    }
                }
            }
            else
            {
                foreach (var @base in mapBases)
                {
                    if (IsBaseOurs(@base) && (@base.name == "SE" || @base.name == "SW"))
                    {
                        i++;
                    }
                } 
            }
            
            

            return i == 2;
        }
        
        public bool IsBaseOurs(Base baseToCheck)
        {
            return baseToCheck.MyTeam != otherTeam;
        }
    }
}