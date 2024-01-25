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

        private Action _onLevelComplete;
        private List<CharacterView> _characterWaitingForNextLevel = new List<CharacterView>();

        public void AlertEnemies(Vector2 position)
        {
            _enemies.ForEach(enemy => enemy.AddPointOfInterest(position));
        }

        public void Setup(ICampaign campaign, IPlatformConfig platformConfig, Action onLevelComplete)
        {
            _platformConfig = platformConfig;
            _campaign = campaign;

            _onLevelComplete = onLevelComplete;

            _enemies.ForEach(enemie => enemie.Setup(_platformConfig, _campaign));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out CharacterView character))
            {
                _characterWaitingForNextLevel.Add(character);

                if (_enemies.All(enemy => enemy.View.IsDead) &&
                    _campaign.Players.All(player => _characterWaitingForNextLevel.Contains(player)))
                {
                    _onLevelComplete.Invoke();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out CharacterView character))
            {
                _characterWaitingForNextLevel.Remove(character);
            }
        }
    }
}
