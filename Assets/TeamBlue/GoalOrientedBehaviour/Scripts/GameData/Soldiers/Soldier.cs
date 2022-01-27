using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.EOTS;
using Assets.General_Scripts;
using Assets.Scripts.SteeringBehaviours.Basics;
using Assets.TeamBlue;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.AI.GOAP;
using Assets.TeamBlue.Pathfinding;
using Assets.TeamBlue.Pathfinding.Scripts.AStar;
using UnityEngine;

namespace TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Soldiers
{
    public class Soldier : MonoBehaviour, IGoap, ISoldier
    {
        public bool DroppedFlag;
        public bool HasFlag { get; set; }
        public Transform MyTransform { get; set; }
        
        private PathfindingUnit _pathfinding;
        protected TeamManager _teamManager;
        protected HashSet<KeyValuePair<string, object>> _soldierGoal;
        
        public Teams MyTeam
        {
            get => _myTeam;
            set => _myTeam = value;
        }

        /// <summary>
        /// Used for the Guard logic
        /// </summary>
        public bool Invulnerable
        {
            get => _invulnerable;
            set => _invulnerable = value;
        }


        private GoapAgent _agent;
        private Respawner _myRespawner;
        private IPathfindingUnit _pathfindingUnit;
        [SerializeField] private Teams _myTeam;
        [SerializeField] private bool _invulnerable;
        private SteeringBasics _mySB;
        private bool _sprintOnCD;


        private void Awake()
        {
            _pathfinding = GetComponent<PathfindingUnit>();
            _pathfindingUnit = GetComponent<IPathfindingUnit>();
            _agent = GetComponent<GoapAgent>();
            _myRespawner = FindObjectsOfType<Respawner>().First(sp => sp.MyTeam == MyTeam);
            MyTransform = transform;
            _mySB = GetComponent<SteeringBasics>();
            _teamManager = FindObjectOfType<TeamManager>();
            _soldierGoal = new HashSet<KeyValuePair<string, object>>();
        }

        

        /// <summary>
        /// Key-Value data that will feed the GOAP actions and system while planning.
        /// </summary>
        /// <returns></returns>
        public HashSet<KeyValuePair<string, object>> GetWorldState()
        {
            var worldData = new HashSet<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("hasFlag", HasFlag),
                new KeyValuePair<string, object>("scored", DroppedFlag),
            };

            return worldData;
        }


        /// <summary>
        /// Our only goal will ever be to drop flags.
        /// The ScoreFlag action will be able to fulfill this goal.
        /// </summary>
        /// <returns></returns>
        public HashSet<KeyValuePair<string, object>> CreateGoalState()
        {
            // var goal = new HashSet<KeyValuePair<string, object>>
            // {
            //     //new KeyValuePair<string, object>("hasFlag",true),
            //     new KeyValuePair<string, object>("scored", true),
            //     //new KeyValuePair<string, object>("attacked", true),
            //     // new KeyValuePair<string, object>(_teamManager.GetGoal(this),true),
            //     // new KeyValuePair<string, object>(_teamManager.GetGoal(this),true),
            //    //new KeyValuePair<string, object>("captureBaseAction",true),
            //     
            // };
            //
            // return goal;
            
            SetGoal();
            return _soldierGoal;
        }

        public void SetGoal()
        {
            _soldierGoal.Clear();
            _soldierGoal.Add(new KeyValuePair<string, object>(_teamManager.GetSoldierGoal(this),true));
        }
        
        /// <summary>
        /// Plan failed. Add cleanup code if necessary
        /// </summary>
        /// <param name="failedGoal"></param>
        public void PlanFailed(HashSet<KeyValuePair<string, object>> failedGoal)
        {
            // Not handling this here since we are making sure our goals will always succeed.
            // But normally you want to make sure the world state has changed before running
            // the same goal again, or else it will just fail.
        }

        /// <summary>
        /// Plan found.
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="actions"></param>
        public void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<IGoapAction> actions)
        {
            // Yay we found a plan for our goal
            //Debug.Log("<color=green>Plan found</color> " + GoapAgent.PrettyPrint(actions));

        }

        /// <summary>
        /// Função que vai dar o valor á variavel global do goal (depois no goalState so da return da variavel)
        /// </summary>
        

        /// <summary>
        /// Everything is done, we completed our actions for this gool. Hooray! Add code if necessary
        /// </summary>
        public void ActionsFinished()
        {
        }
         /// <summary>
         /// Current action is finished. We are no longer using the workstation.
         /// </summary>
         /// <param name="currentAction"></param>
        public void CurrentActionFinished(IGoapAction currentAction)
        {
        }

        /// <summary>
        /// Clean up for the current action and setup for the next action.
        /// </summary>
        /// <param name="currentAction"></param>
        /// <param name="nextAction"></param>
        public void CurrentActionFinished(IGoapAction currentAction, IGoapAction nextAction)
        {
            CurrentActionFinished(currentAction);
            // if we have more actions, verify that it is still possible to perform the plan. Abort if there is no longer an available target.
            if (nextAction != null && nextAction.CheckProceduralPrecondition(gameObject) == false)
                PlanAborted(nextAction);
        }

        /// <summary>
        /// An action bailed out of the plan. State has been reset to plan again.
        /// Take note of what happened and make sure if you run the same goal again
        /// that it can succeed.
        /// </summary>
        /// <param name="aborter"></param>
        public void PlanAborted(IGoapAction aborter)
        {
            _agent.AbortPlan();
        }

        /// <summary>
        /// Moves the agent to the target of the next action. This implementation uses AStart pathfinding.
        /// </summary>
        /// <param name="nextAction"></param>
        /// <returns>Returns true if we are in range of the target</returns>
        public bool MoveAgent(IGoapAction nextAction)
        {
            if (nextAction.Target == null)
                return false; // we are not there yet
            
            _pathfindingUnit.SetTarget(nextAction.Target.transform);

            if (_pathfindingUnit.DoFollowPathStep() == false) // if we are not following the path anymore
            {
                // we are at the target location, we are done
                nextAction.InRange = true;


                return true; // we have arrived
            }

            return false; // we are not there yet
        }

        

        public void Died()
        {
            if (HasFlag)
            {
                GetComponentInChildren<FlagComponent>().Drop();
                HasFlag = false;
            }
                

            transform.position = _myRespawner.transform.position;

            StartCoroutine(CantMove(5f));
        }

        private IEnumerator CantMove(float seconds)
        {
            while (seconds > 0)
            {
                transform.position = _myRespawner.transform.position;
                seconds -= Time.deltaTime;
                yield return null;
            }
        }

        public void Guard()
        {
            if (HasFlag)
            {
                GetComponent<FlagComponent>().Drop();
            }
            StartCoroutine(StartGuard());
        }

        private IEnumerator StartGuard()
        {
            Invulnerable = true;
            yield return new WaitForSeconds(2);
            Invulnerable = false;
        }

        public void Sprint()
        {
            StartCoroutine(DoubleSpeed());
        }

        private IEnumerator DoubleSpeed()
        {
            _mySB.MaxAcceleration *= 2;
            _mySB.MaxVelocity *= 2;

            _sprintOnCD = true;

            yield return new WaitForSeconds(5);

            _mySB.MaxAcceleration /= 2;
            _mySB.MaxVelocity /= 2;

            _sprintOnCD = false;
        }

        public bool IsSpringOnCD()
        {
            return _sprintOnCD;
        }
    }
}

