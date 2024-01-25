using Mdb.EasyTrigger.Character;
using Mdb.EasyTrigger.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Mdb.EasyTrigger.Level
{
    public class Campaign : MonoBehaviour, ICampaign
    {
        [SerializeField] private Level[] _levels;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private Transform _initialPosition;
        [SerializeField] private float _nLevelOffset;
        [SerializeField] private float _nextLevelCameraSpeed = 1f;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Image[] _playerAttackImages;

        public ILevel CurrentLevel => _currentLevel;
        public CharacterView[] Players => _players.ToArray();

        public Vector2 StartPoint => _initialPosition.position;

        private IPlatformConfig _platformConfig;

        private int _currentLevelIndex = 0;
        private Level _currentLevel;
        private Transform _camera;

        private Action _onWin;
        private Action _onGameOver;

        private List<CharacterView> _players = new List<CharacterView>();

        public void Shoot(Vector2 point)
        {
            _audioSource.PlayOneShot(_audioClip);

            _currentLevel.AlertEnemies(point);
        }

        public void Setup(IPlatformConfig platformConfig, Transform camera, Action onWin, Action onGameOver)
        {
            _platformConfig = platformConfig;
            _camera = camera;

            _onWin = onWin;
            _onGameOver = onGameOver;
        }

        public void StartCampaign()
        {
            _currentLevel = Instantiate(_levels[_currentLevelIndex]);
            _currentLevel.Setup(this, _platformConfig, OnCurrentLevelComplete);

            _canvas.gameObject.SetActive(true);
        }

        private void OnCurrentLevelComplete()
        {
            _currentLevelIndex++;

            if (_currentLevelIndex < _levels.Length)
            {
                StartCoroutine(LoadNextLevel());
            }
            else
            {
                _onWin.Invoke();
            }
        }

        private IEnumerator LoadNextLevel()
        {
            float nextLevelPosition = _currentLevelIndex * _nLevelOffset;
            Level newLevel = Instantiate(_levels[_currentLevelIndex], new Vector2(nextLevelPosition, 0),
                Quaternion.identity);
            newLevel.Setup(this, _platformConfig, OnCurrentLevelComplete);

            float cameraInitialPosition = _camera.position.x;
            float totalTime = 0;
            do
            {
                float x = Mathf.Lerp(cameraInitialPosition, nextLevelPosition, totalTime);
                _camera.position = new Vector3(x, _camera.position.y, _camera.position.z);

                yield return new WaitForEndOfFrame();

                totalTime += _nextLevelCameraSpeed * Time.deltaTime;

                if (totalTime >= 1)
                {
                    _camera.position = new Vector3(nextLevelPosition, _camera.position.y, _camera.position.z);
                    break;
                }
            } while (true);

            Destroy(_currentLevel.gameObject);
            _currentLevel = newLevel;
        }

        public void AddPlayers(CharacterView[] players)
        {
            _players.AddRange(players);

            _players.ForEach(player => player.SetOnDeathCallback(OnPlayerDeath));

            for (int i = 0; i < _playerAttackImages.Length; i++)
            {
                if (i < _players.Count)
                {
                    _playerAttackImages[i].gameObject.SetActive(true);
                    _players[i].SetAttackImage(_playerAttackImages[i]);
                }
                else
                {
                    _playerAttackImages[i].gameObject.SetActive(false);
                }
            }
        }

        private void OnPlayerDeath()
        {
            if (_players.All(player => player.IsDead))
            {
                _onGameOver();
            }
        }

        public void Clear()
        {
            _currentLevelIndex = 0;
            _players.Clear();

            if (_currentLevel != null)
            {
                Destroy(_currentLevel.gameObject);
            }

            _canvas.gameObject.SetActive(false);
        }
    }
}
