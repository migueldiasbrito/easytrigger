using Mdb.EasyTrigger.Character;
using Mdb.EasyTrigger.Config;
using Mdb.EasyTrigger.Enemy;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mdb.EasyTrigger.Level
{
    public class Level : MonoBehaviour, ILevel
    {
        [SerializeField] public List<EnemyController> _enemies;
        [field: SerializeField] public Collider2D PlatformCollider { get; private set; }

        public CharacterView[] Enemies => _enemies.Select(enemy => enemy.View).ToArray();

        private ICampaign _campaign;
        private IPlatformConfig _platformConfig;

        public void AlertEnemies(Vector2 position)
        {
            _enemies.ForEach(enemy => enemy.AddPointOfInterest(position));
        }

        public void Setup(ICampaign campaign, IPlatformConfig platformConfig)
        {
            _platformConfig = platformConfig;
            _campaign = campaign;

            _enemies.ForEach(enemie => enemie.Setup(_platformConfig, _campaign));
        }
    }
}
