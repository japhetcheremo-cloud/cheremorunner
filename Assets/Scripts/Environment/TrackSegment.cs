using UnityEngine;
using System.Collections.Generic;

namespace CheremoRunner.Environment
{
    public class TrackSegment : MonoBehaviour, Gameplay.IPoolable
    {
        private List<GameObject> activeChildren = new List<GameObject>();

        public void RegisterChildObject(GameObject obj)
        {
            if (obj != null && !activeChildren.Contains(obj))
            {
                activeChildren.Add(obj);
            }
        }

        public void OnSpawnFromPool()
        {
            activeChildren.Clear();
        }

        public void OnReturnToPool()
        {
            for (int i = 0; i < activeChildren.Count; i++)
            {
                if (activeChildren[i] != null)
                {
                    activeChildren[i].SetActive(false);
                }
            }
            activeChildren.Clear();
        }
    }
}
