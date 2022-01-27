﻿using System.Linq;
using Assets.EOTS;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.AI.GOAP;
using TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;

namespace Assets.TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Actions
{
    public class ScoreFlag : GoapAction
    {
        /// <summary>
        /// The object used for the effect
        /// </summary>
        private bool _scored;

        /// <summary>
        /// Target of this action
        /// </summary>
        private Transform _myTeamBase;

        private Base _droppingBase;
        private FlagComponent _flag;

        private Soldier _soldier;

        private void Awake()
        {
            _soldier = GetComponent<Soldier>();
            _flag = FindObjectOfType<FlagComponent>();
            AddPrecondition("hasFlag", true); // we must have the flag to drop it at the base
            AddEffect("scored", true); // we will have dropped the flag once we finish
            AddEffect("hasFlag", false); // we will no longer have the flag after we drop it

        }

        public override void Reset()
        {
            //print("Reset action");
            _scored = false;
            StartTime = 0;
        }

        public override bool IsDone()
        {
            return _scored;
        }

        public override bool RequiresInRange()
        {
            return true; // you must be in range to drop the flag
        }

        public override bool CheckProceduralPrecondition(GameObject agent)
        {
            if (_soldier.Invulnerable) return false;
            
            if (Utils.GetClosest(FindObjectsOfType<Base>().Where(b => b.MyTeam == _soldier.MyTeam), transform, out _droppingBase))
            // if (Utils.GetClosest(FindObjectsOfType<Base>(), transform, out _droppingBase) && _soldier.Invulnerable == false)
            {
                Target = _droppingBase.gameObject;
                return true;
            }
            
            return false;
        }

        public override bool Perform(GameObject agent)
        {

            if (_soldier.HasFlag == false)
                return false; // lost the flag somewhere

            _flag.Score(_droppingBase);
            _soldier.HasFlag = false;
            _scored = true; // you have dropped the flag

            print("scored flag");

            return true;
        }
    }
}