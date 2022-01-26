using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.AI.GOAP;
using TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using Assets.EOTS;
using System.Collections;
using UnityEngine;
using System.Linq;
using Assets.General_Scripts;

namespace Assets.TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Actions
{
    public class SprintAction : GoapAction
    {
        private bool _ran;
        private bool _onCooldown;
        

        public GameObject AttTarget;
        public float Range;
        private Soldier _soldier;


        private void Awake()
        {
            _soldier = GetComponent<Soldier>();
            AddEffect("ran", true);
        }


        public override void Reset()
        {
            _ran = false;
        }

        public override bool IsDone()
        {
            return _ran;
        }

        public override bool RequiresInRange()
        {
            return false;
        }



        public override bool CheckProceduralPrecondition(GameObject agent)
        {
            if (_onCooldown == false)
            {
                return true;
            }
            return false;
        }


        public override bool Perform(GameObject agent)
        {
            _soldier.Sprint();
            StartCoroutine(StartCooldown());
            _ran = true;
            print("RAN");
            return true;
        }


        private IEnumerator StartCooldown()
        {
            _onCooldown = true;
            yield return new WaitForSeconds(30f);
            _onCooldown = false;
        }

        public override bool Equals(object other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }





        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}