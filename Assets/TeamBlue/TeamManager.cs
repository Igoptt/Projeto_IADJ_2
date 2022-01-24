using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.EOTS;
using Assets.General_Scripts;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.AI.GOAP;
using TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;

namespace Assets.TeamBlue
{
    public class TeamManager : MonoBehaviour
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
            MapBases = new List<Base>();
            var allBases = FindObjectsOfType<Base>().ToList();

            foreach (var bases in allBases)
            {
                MapBases.Add(bases);
            }

            //if my team = redTeam then otherTeam = Blue Team else contrario
            OtherTeam = MyTeam == Teams.RedTeam ? Teams.BlueTeam : Teams.RedTeam;
            EnemyArmy = FindObjectsOfType<MonoBehaviour>().OfType<ISoldier>().Where(s => s.MyTeam == OtherTeam).ToList();
        }
        

        public string GetGoal(Soldier soldier)
        {
            var goal = soldier.HasFlag ? "scored" : "captureBaseAction";

            return goal;
        }
    }
}