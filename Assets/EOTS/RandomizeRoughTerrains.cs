using System.Collections;
using System.Collections.Generic;
using Assets.TeamBlue.GoalOrientedBehaviour.Scripts.GameData;
using UnityEngine;

namespace Assets.EOTS
{
    public class RandomizeRoughTerrains : MonoBehaviour
    {
        public List<Transform> TerrainList;

        private List<Vector3> _scalesInitialValue;

        private void Awake()
        {
            _scalesInitialValue = new List<Vector3>();
            foreach (var t in TerrainList)
            {
                _scalesInitialValue.Add(t.localScale);
            }
        }

        private void Start()
        {
            StartCoroutine(InitializeRandomizationNightmare());
        }

        private IEnumerator InitializeRandomizationNightmare()
        {
            while (true)
            {
                for (var i = 0; i < TerrainList.Count; i++)
                    TerrainList[i].localScale = _scalesInitialValue[i] * Random.Range(.5f, 2);

                yield return new WaitForSeconds(15);
            }
        }
    }
}
