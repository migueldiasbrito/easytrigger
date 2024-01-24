﻿using Mdb.EasyTrigger.Presentation.Character;
using Mdb.EasyTrigger.Presentation.Config;
using Mdb.EasyTrigger.Presentation.Enemy;
using Mdb.EasyTrigger.Presentation.Input;
using Mdb.EasyTrigger.Presentation.Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Dev
{
    public class PlayerAnimationsSetup : MonoBehaviour, ILevel
    {
        [SerializeField] private InputController _inputController;
        [SerializeField] private CharacterInputListener _characterController;
        [SerializeField] private PlatformConfig _platformConfig;
        [SerializeField] private List<EnemyController> _enemies;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip audioClip;

        [SerializeField] private LayerMask _charactersLayerMask;

        public CharacterView[] Players => new CharacterView[] { _characterController.View };
        public CharacterView[] Enemies => _enemies.Select(enemy => enemy.View).ToArray();
        [field: SerializeField] public Collider2D PlaftformCollider { get; private set; }

        public void Shoot(Vector2 point)
        {
            _audioSource.PlayOneShot(audioClip);

            _enemies.ForEach(enemy => enemy.AddPointOfInterest(point));
        }

        private void Start()
        {
            _characterController.Setup(_inputController, _platformConfig, this);

            _enemies.ForEach(enemie => enemie.Setup(_platformConfig, this));

            int charactersLayer = _characterController.View.gameObject.layer;
            Physics2D.IgnoreLayerCollision(charactersLayer, charactersLayer);
        }

        private void OnDestroy()
        {
            _characterController.Dispose();
        }
    }
}
