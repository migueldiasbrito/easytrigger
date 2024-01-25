using Mdb.EasyTrigger.Character;
using Mdb.EasyTrigger.Config;
using System.Collections.Generic;
using UnityEngine;

namespace Mdb.EasyTrigger.Level
{
    public class Campaign : MonoBehaviour, ICampaign
    {
        [SerializeField] private Level[] _levels;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip audioClip;

        public ILevel CurrentLevel => _currentLevel;
        public CharacterView[] Players => _players.ToArray();

        private int _currentLevelIndex = 0;
        private Level _currentLevel;
        private List<CharacterView> _players = new List<CharacterView>();
        private IPlatformConfig _platformConfig;

        public void Shoot(Vector2 point)
        {
            _audioSource.PlayOneShot(audioClip);

            _currentLevel.AlertEnemies(point);
        }

        public void Setup(IPlatformConfig platformConfig)
        {
            _platformConfig = platformConfig;
        }

        public void StartCampaign()
        {
            _currentLevel = Instantiate(_levels[_currentLevelIndex]);
            _currentLevel.Setup(this, _platformConfig);
        }

        public void AddPlayers(CharacterView[] players)
        {
            _players.AddRange(players);
        }

        public void Clear()
        {
            _currentLevelIndex = 0;
            _players.Clear();
            Destroy(_currentLevel);
        }
    }
}
