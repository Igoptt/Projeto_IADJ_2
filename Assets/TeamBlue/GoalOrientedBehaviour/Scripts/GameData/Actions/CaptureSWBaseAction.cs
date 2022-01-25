using System.Collections.Generic;
using System.Linq;
using Assets.EOTS;
using Assets.General_Scripts;
using Assets.Scripts.SteeringBehaviours.Basics;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.AI.GOAP;
using TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;

public class CaptureSWBaseAction : GoapAction
{
     
    private Base _captureBase;
    private ISoldier _soldier;
    public List<Base> MapBases;
        
    private void Awake()
    {
        _soldier = GetComponent<Soldier>();
        AddEffect("captureSWBase", true);

    }

    public override void Reset()
    {
        _captureBase = null;
    }

    public override bool IsDone()
    {
        return _captureBase.MyTeam == _soldier.MyTeam;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        MapBases = new List<Base>();
        var allBases = FindObjectsOfType<Base>().ToList();
            
        foreach (var bases in allBases)
        {
            if (bases.name == "SW")
            {
                _captureBase = bases;
            }
                
            MapBases.Add(bases);
        }
            
        Target = _captureBase.gameObject;
        return true;
            
    }

    public override bool Perform(GameObject agent)
    {
        //para quando captura for now
        _soldier.MyTransform.GetComponent<SteeringBasics>().Stop();
        return true;
    }

    public override bool RequiresInRange()
    {
        return true; // must be in range para capturar a base

    }

}
