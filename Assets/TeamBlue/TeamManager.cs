using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.EOTS;
using Assets.General_Scripts;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Actions;
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

        public List<ISoldier> _enemyArmy;

        private ISoldier _closestEnemy;

        private void Awake()
        {
            flag = FindObjectOfType<FlagComponent>();
            myArmy = FindObjectsOfType<Soldier>().ToList();
            mapBases = FindObjectsOfType<Base>().ToList();
            
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
        }

        //TODO fazer uma funçao para verificar se tem mais que um inimigo por perto (basta ver as proximas posiçoes do GetClosest)
        public bool EnemyClose(Soldier soldier)
        {
            //vai buscar o inimigo mais proximo
            if(Utils.GetClosest(_enemyArmy.Select(s => s.MyTransform.GetComponent<MonoBehaviour>()), soldier.MyTransform, out var mono));
            {
                //atraves do MonoBehaviour do inimigo mais proximo vamos buscar o seu ISoldier
                _closestEnemy = mono.gameObject.GetComponent<ISoldier>();
            
                //verifica se o inimigo mais proximo esta a pelo menos 5x o range do attack (o range é 1.5f entao 5x é relativamente perto)
                return Vector3.Distance(soldier.MyTransform.position, _closestEnemy.MyTransform.position) < 1.5f * 2; 
            }
        }
        
        //TODO meter o soldado a atacar/guard tambem se tiver 1 ou mais inimigos por perto
        public string GetGoal(Soldier soldier)
        {
            var goal ="";
            var attackCost = -5000f;
            if (EnemyClose(soldier) && !soldier.HasFlag)
            {
                // soldier.GetComponent<GoapAgent>().AbortPlan();
                soldier.GetComponent<AttackNearestEnemyAction>().Cost = attackCost;
                return goal = "attackNearestEnemy";
            }

            soldier.GetComponent<AttackNearestEnemyAction>().Cost = 1f;
            goal = soldier.HasFlag ? "scored" : "captureBaseAction";

            

            // foreach (var bases in MapBases)
            // {
            //     if (bases.name == "NE" && IsBaseOurs(bases))
            //     {
            //         goal = "hasFlag";
            //     }
            // }
            // if (!DoWeHaveOurBases(MyTeam))
            // {
            //     goal = "captureBaseAction";
            // }
            // else
            // {
            //     // foreach (var soldiers in MyArmy)
            //     // {
            //     //     soldiers.GetComponent<PickUpFlag>().Cost = 1;
            //     //     soldiers.GetComponent<ScoreFlag>().Cost = 1;
            //     //     
            //     // }
            //     goal = "scored";
            // }
            
                
            
            return goal;
        }

        //TODO dar double check se o reset dos planos esta a funcionar corretamente
        private IEnumerator PeriodicPlanReset()
        {
            while (true)
            {
                StartCoroutine(ResetTeamPlans());

                yield return new WaitForSeconds(0.5f);
            }
        }
        
        public IEnumerator ResetTeamPlans()
        {
            foreach (var soldier in myArmy)
            {
                // ResetPlan(soldier.GetComponent<GoapAgent>());
                soldier.GetComponent<GoapAgent>().AbortPlan();
            }

            yield return null;
        }
        
        

        public void ResetPlan(GoapAgent agent)
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