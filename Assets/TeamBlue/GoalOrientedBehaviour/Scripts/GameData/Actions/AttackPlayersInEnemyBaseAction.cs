using System.Collections;
using System.Linq;
using System.Transactions;
using Assets.EOTS;
using Assets.General_Scripts;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.TeamRed.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;

namespace Assets.TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Actions
{
    public class AttackPlayersInEnemyBaseAction : GoapAction
    {
        private bool _attacked;
        private bool _onCooldown;
        private ISoldier _target;
        private ISoldier _me;
        

        public GameObject AttTarget;
        public float Range = 1.5f;


        private void Awake()
        {
            _me = GetComponent<ISoldier>();

            AddEffect("attackEnemiesInBase", true);
        }


        public override void Reset()
        {
            _attacked = false;
        }

        public override bool IsDone()
        {
            return _attacked;
        }

        public override bool RequiresInRange()
        {
            return true;
        }

        public override bool CheckProceduralPrecondition(GameObject agent)
        {
            
            //ataca os soldados na base inimiga ou neutra mais proxima
            if (Utils.GetClosest(
                FindObjectsOfType<Base>().
                    Where(b => _me.MyTeam == Teams.RedTeam && b.MemberOfTeamBlue.Count != 0 && b._myMat.color != Color.red ||
                               _me.MyTeam == Teams.BlueTeam && b.MemberOfTeamRed.Count != 0 && b._myMat.color != Color.blue), _me.MyTransform, out var @base))
            {

                var list = _me.MyTeam == Teams.BlueTeam
                    ? @base.MemberOfTeamRed
                    : @base.MemberOfTeamBlue;

                //GameObject go;
                if (Utils.GetClosest(list.Select(s => s.MyTransform.GetComponent<MonoBehaviour>()), agent.transform, out var mono))
                {
                    _target = mono.gameObject.GetComponent<ISoldier>();
                    Target = mono.gameObject;
                    AttTarget = Target;
                    return true;
                }

                return true;
            }

            return false;


        }
         
        public override bool Perform(GameObject agent)
        {

            if (Target == null || _target.Invulnerable || _onCooldown || Vector3.Distance(agent.transform.position, _target.MyTransform.position) > Range)
                return false;

            
            _target.Died();
            _target = null;
            StartCoroutine(StartCooldown());

            _attacked = true;

            print("ATTACKED");


            return true;
        }


        private IEnumerator StartCooldown()
        {
            _onCooldown = true;
            yield return new WaitForSeconds(15f);
            _onCooldown = false;
        }

    }
}