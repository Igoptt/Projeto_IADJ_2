using System.Collections;
using System.Collections.Generic;
using Assets.EOTS;
using Assets.TeamRed.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.TeamRed.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;

namespace TeamRed.GoalOrientedBehaviour.Scripts.GameData.Actions
{
        
    public class PickUpFlag : GoapAction
    {
        /// <summary>
        /// The object used for the effect
        /// </summary>
        private bool _hasFlag;

        /// <summary>
        /// The target of this action
        /// </summary>
        private FlagComponent _flag;

        private Soldier _soldier;
        private void Awake()
        {
            AddPrecondition("hasFlag", false); // we cannot have the flag to pick up the flag
            AddEffect("hasFlag", true); // we will have the flag after we picked it up

            // cache the flag
            _soldier = GetComponent<Soldier>();
            _flag = FindObjectOfType<FlagComponent>();
            Target = _flag.gameObject;
        }

        public override void Reset()
        {
            _hasFlag = false;
            StartTime = 0;    }

        public override bool IsDone()
        {
            return _hasFlag;
        }

        public override bool CheckProceduralPrecondition(GameObject agent)
        {
            if (_soldier.Invulnerable == false && _flag.BeingCarried == false && _flag.CanBeCarried)
                Target = _flag.gameObject;

            return _flag.BeingCarried == false;    
        }

        public override bool Perform(GameObject agent)
        {
            if (_soldier.Invulnerable)
                return false;

            if (Target == null)
                return false;

            if (_flag.BeingCarried || _flag.CanBeCarried == false) return false; // someone else got the flag before you

            var runner = agent.GetComponent<Soldier>();

            _hasFlag = true;
            runner.HasFlag = true;
            _flag.PickUp(runner);

            print("picked up flag");

            return true;
        }

        public override bool RequiresInRange()
        {
            return true; // yes we need to be near the flag to pick it up  
        }
    }
}
