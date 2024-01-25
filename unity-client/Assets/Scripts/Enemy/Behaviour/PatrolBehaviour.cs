﻿using UnityEngine;

namespace Mdb.EasyTrigger.Enemy.Behaviour
{
    public class PatrolBehaviour : IPathedBehaviour
    {
        [SerializeField] private Transform[] _patrolPoints;

        private void Start()
        {
            foreach (Transform transform in _patrolPoints)
            {
                _path.Add(transform.position);
            }
        }
    }
}