using System.Collections;
using UnityEngine;

namespace Assets
{
    public class MovementPenalty : MonoBehaviour
    {
        public float MovementSpeedMultiplyer;
        
        private void Start()
        {
            StartCoroutine(InitializeRandomizationNightmare());
        }

        private IEnumerator InitializeRandomizationNightmare()
        {
            while (true)
            {
                MovementSpeedMultiplyer = Random.Range(1/3f, 1.5f);
                yield return new WaitForSeconds(15);
            }
        }
    }
}
