using System.Collections.Generic;
using System.Linq;
using Assets.EOTS;
using Assets.General_Scripts;
using Assets.Scripts.SteeringBehaviours.Basics;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.AI.GOAP;
using TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;

public class CaptureNEBaseAction : GoapAction
{
     

    private Base _captureBase;
    private ISoldier _soldier;
    public List<Base> MapBases;
        
    private void Awake()
    {
        _soldier = GetComponent<Soldier>();
        MapBases = FindObjectsOfType<Base>().ToList();
        // AddEffect("captureNEBase", true);
        AddEffect("captureBaseAction", true);

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
        if (_soldier.Invulnerable) return false;
        
        foreach (var bases in MapBases)
        {
            if (bases.name == "NE")
            {
                _captureBase = bases;
            }
        }
        Target = _captureBase.gameObject;
        return true;
            
    }

    public override bool Perform(GameObject agent)
    {
        //para quando captura for now
        _soldier.MyTransform.GetComponent<SteeringBasics>().Stop();
        print("NE Capturing");
        return true;
    }

    public override bool RequiresInRange()
    {
        return true; // must be in range para capturar a base

    }

}
