using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Assets.EOTS;
using Assets.General_Scripts;
using Assets.TeamBlue;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.GameData;
using UnityEngine;


namespace TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Actions
{
    public class AttackNearestEnemyAction : GoapAction
    {
        private bool _attacked;
        [SerializeField] private bool _onCooldown;
        private ISoldier _target;
        private ISoldier _me;
        private TeamManager _teamManager;

        public GameObject AttTarget;
        public float Range = 1.5f;


        private void Awake()
        {
            _me = GetComponent<ISoldier>();
            _teamManager = FindObjectOfType<TeamManager>();
            AddEffect("attackNearestEnemy", true);
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
            
            if (_onCooldown == false)
            {
                if(Utils.GetClosest(_teamManager._enemyArmy.Select(s => s.MyTransform.GetComponent<MonoBehaviour>()), _me.MyTransform, out var mono))
                {
                    _target = mono.gameObject.GetComponent<ISoldier>();
                    if (_target.Invulnerable || _target.MyTransform.position == FindObjectsOfType<Respawner>().First(sp => sp.MyTeam == _target.MyTeam).transform.position )
                    {
                        return false;
                    }
                    else
                    {
                        Target = mono.gameObject;
                        AttTarget = Target;
                        // return true;
                        return Vector3.Distance(_me.MyTransform.position, _target.MyTransform.position) < 1.5f * 2; 
                    }
                }
            }
            
            // return _onCooldown == false && Utils.GetClosest(FindObjectsOfType<Soldier>().ToList(), transform, out _target);
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
