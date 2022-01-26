using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.EOTS;
using Assets.General_Scripts;
using Assets.TeamRed.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.TeamRed.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;

namespace Assets.TeamRed
{
    public class EnemyTeamManger : MonoBehaviour
    {
        public List<Soldier> MyArmy;
        public List<Base> MapBases;
        public FlagComponent Flag;
        public Teams MyTeam;
        public Teams OtherTeam;

        public List<ISoldier> EnemyArmy;
        
        private void Awake()
        {
            Flag = FindObjectOfType<FlagComponent>();
            MyArmy = FindObjectsOfType<Soldier>().ToList();
            MapBases = FindObjectsOfType<Base>().ToList();
            
            //if my team = redTeam then otherTeam = Blue Team else contrario
            OtherTeam = MyTeam == Teams.RedTeam ? Teams.BlueTeam : Teams.RedTeam;

            //vai buscar todos os objetos do tipo MonoBehaviour (ou subclasses dele) para ter todos os scripts 
            //depois procura dos acima os que implementam a classe ISoldier cuja team seja a OtherTeam
            //source: https://answers.unity.com/questions/863509/how-can-i-find-all-objects-that-have-a-script-that.html
            EnemyArmy = FindObjectsOfType<MonoBehaviour>().OfType<ISoldier>().Where(s => s.MyTeam == OtherTeam).ToList();
        }
         
        //TODO meter o soldado a atacar/guard tambem se tiver 1 ou mais inimigos por perto
        public string GetGoal(Soldier soldier)
        {
            var goal ="";
            goal = soldier.HasFlag ? "scored" : "captureBaseAction";


            // foreach (var bases in MapBases)
            // {
            //     if (bases.name == "NE" && IsBaseOurs(bases))
            //     {
            //         goal = "score";
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
        
        //TODO meter um abort plan da equipa toda a cada tipo 5Segundos ou algo do genero
        private IEnumerator PeriodicPlanReset()
        {
            while (true)
            {
                StartCoroutine(ResetTeamPlans());

                yield return new WaitForSeconds(5f);
            }
        }
        
        public IEnumerator ResetTeamPlans()
        {
            foreach (var soldier in MyArmy)
            {
                ResetPlan(soldier.GetComponent<GoapAgent>());
            }

            yield return null;
        }
        private void Start()
        {
            StartCoroutine(PeriodicPlanReset());
        }

        public void ResetPlan(GoapAgent agent)
        {
            agent.AbortPlan();
        }

    }
}