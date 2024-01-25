using Mdb.EasyTrigger.Character;
using Mdb.EasyTrigger.Config;
using Mdb.EasyTrigger.Input;
using System.Collections.Generic;
using UnityEngine;

namespace Mdb.EasyTrigger.Level.Meta
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlatformConfig _platformConfig;
        [SerializeField] private CharacterInputListener _playerPrefab;
        [SerializeField] private InputController _inputControllerPrefab;
        [SerializeField] private Campaign _singlePlayerCampaign;

        private List<CharacterInputListener> _players = new List<CharacterInputListener>();
        private List<InputController> _inputControllers = new List<InputController>();

        private void Start()
        {
            _singlePlayerCampaign.Setup(_platformConfig);
            StartSinglePlayerCampaign();
        }

        private void StartSinglePlayerCampaign()
        {
            CharacterInputListener player = Instantiate(_playerPrefab, _singlePlayerCampaign.StartPoint,
                Quaternion.identity);
            InputController inputController = Instantiate(_inputControllerPrefab);
            _players.Add(player);
            _inputControllers.Add(inputController);

            player.Setup(inputController, _platformConfig, _singlePlayerCampaign);

            _singlePlayerCampaign.AddPlayers(new CharacterView[] { _playerPrefab.View });
            _singlePlayerCampaign.StartCampaign();
        }
    }
}
