﻿using Mdb.EasyTrigger.Presentation.Character.Attack;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Enemy
{
    public class FollowBehaviour : IEnemyBehaviour
    {
        [SerializeField] private CharacterAttack _characterAttack;
        public Transform Target { get; set; }

        public override Vector2? GetNextPoint()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdatedPosition(Vector2 position, Vector2 comparisonTolerance)
        {
            throw new System.NotImplementedException();
        }
    }
}