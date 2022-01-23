using System.Collections;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.AI.GOAP;
using TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;

namespace TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Actions
{
    public class GuardAction : GoapAction
    {
        private bool _invulnerable;
        private Soldier _soldier;
        private bool _onCooldown;
        private void Awake()
        {
            _soldier = GetComponent<Soldier>();
            AddEffect("Invulnerable",true);
        }

        public override void Reset()
        {
            _invulnerable = false;
        }

        public override bool IsDone()
        {
            return _invulnerable;
        }

        public override bool CheckProceduralPrecondition(GameObject agent)
        {
            return _onCooldown == false;
        }

        public override bool Perform(GameObject agent)
        {
            _soldier.Guard();
            StartCoroutine(StartCooldown());
            _invulnerable = true;
            return true;
        }

        public override bool RequiresInRange()
        {
            return false;
        }

        private IEnumerator StartCooldown()
        {
            _onCooldown = true;
            yield return new WaitForSeconds(30f);
            _onCooldown = false;
        }
    }
}