using Mdb.EasyTrigger.Character;
using Mdb.EasyTrigger.Config;
using Mdb.EasyTrigger.Input;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Mdb.EasyTrigger.Level.Meta
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlatformConfig _platformConfig;
        [SerializeField] private CharacterInputListener _playerPrefab;
        [SerializeField] private InputController _inputControllerPrefab;
        [SerializeField] private Campaign _singlePlayerCampaign;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Canvas _mainMenu;
        [SerializeField] private TMP_Text _text;

        private List<CharacterInputListener> _players = new List<CharacterInputListener>();
        private List<InputController> _inputControllers = new List<InputController>();

        private float _cameraInitialPosition;

        public void OnSingleCampaignStart()
        {
            ClearCampaigns();

            _cameraTransform.position = new Vector3(_cameraInitialPosition, _cameraTransform.position.y,
                _cameraTransform.position.z);
            _mainMenu.gameObject.SetActive(false);

            StartSinglePlayerCampaign();
        }

        private void ClearCampaigns()
        {
            _singlePlayerCampaign.Clear();

            _players.ForEach(player => Destroy(player.gameObject));
            _players.Clear();

            _inputControllers.ForEach(input => Destroy(input.gameObject));
            _inputControllers.Clear();
        }

        public void OnExit()
        {
            Application.Quit();
        }

        private void Start()
        {
            _cameraInitialPosition = _cameraTransform.position.x;
            _mainMenu.gameObject.SetActive(true);

            _singlePlayerCampaign.Setup(_platformConfig, _cameraTransform, OnWin, OnGameOver);
        }

        private void StartSinglePlayerCampaign()
        {
            CharacterInputListener player = Instantiate(_playerPrefab, _singlePlayerCampaign.StartPoint,
                Quaternion.identity);
            InputController inputController = Instantiate(_inputControllerPrefab);
            _players.Add(player);
            _inputControllers.Add(inputController);

            player.Setup(inputController, _platformConfig, _singlePlayerCampaign);

            _singlePlayerCampaign.AddPlayers(new CharacterView[] { player.View });
            _singlePlayerCampaign.StartCampaign();
        }

        private void OnGameOver()
        {
            DisplayMenu("You Died...");
        }

        private void OnWin()
        {
            DisplayMenu("YOU WON!");
        }

        private void DisplayMenu(string message)
        {
            _text.text = message;
            _mainMenu.gameObject.SetActive(true);
        }
    }
}
