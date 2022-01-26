using System.Collections.Generic;
using System.Linq;
using Assets.General_Scripts;
using TeamBlue.GoalOrientedBehaviour.Scripts.GameData.Soldiers;
using UnityEngine;

namespace Assets.TeamBlue.GoalOrientedBehaviour.Scripts.GameData
{
    public static class Utils
    {
        /// <summary>
        /// Get the closest T from agent. Returns true if there is any in the objects list and false if there is none. if there is any, put it on the out variable
        /// </summary>
        /// <param name="objects">The list of T objects where we are going to search.</param>
        /// <param name="agent">The agent we are going to be using for comparing distances</param>
        /// <param name="closest">The out variable where we going to store the result. Can be null</param>
        /// <returns>Returns true fi we find a object.</returns>
        public static bool GetClosest<T>(IEnumerable<T> objects, Transform agent, out T closest) where T : MonoBehaviour
        {
            closest = objects
                .OrderBy(go => Vector3.Distance(go.transform.position, agent.position))
                .FirstOrDefault();

            return closest != default(T);
        }
        
        public static bool EnemyClose(Soldier soldier, List<ISoldier> enemyArmy)
        {
            //vai buscar o inimigo mais proximo
            if(GetClosest(enemyArmy.Select(s => s.MyTransform.GetComponent<MonoBehaviour>()), soldier.MyTransform, out var mono));
            {
                //atraves do MonoBehaviour do inimigo mais proximo vamos buscar o seu ISoldier
                var closestEnemy = mono.gameObject.GetComponent<ISoldier>();
            
                //verifica se o inimigo mais proximo esta a pelo menos 5x o range do attack (o range é 1.5f entao 5x é relativamente perto)
                return Vector3.Distance(soldier.MyTransform.position, closestEnemy.MyTransform.position) < 1.5f * 2; 
            }
        }
        
    }
}
