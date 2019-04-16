using System;
using System.Collections.Generic;
using Logging;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Logging.ILogger;

namespace Replays.Persistence.UI {
    public class ReplayLoaderViewController : MonoBehaviour, IReplayLoaderViewController {
        public event Action<string> LoadReplayClicked = delegate {};
        
        private ICommandHistoryLoader _commandHistoryLoader;
        private ILogger _logger;

        [SerializeField]
        private Button _loadReplayButton;

        [SerializeField]
        private Dropdown _dropdown;

        [Inject]
        public void Construct(ICommandHistoryLoader commandHistoryLoader, ILogger logger) {
            _commandHistoryLoader = commandHistoryLoader;
            _logger = logger;
        }

        private void Start() {
            if (_dropdown == null) {
                _logger.LogError(LoggedFeature.Replays, "Dropdown not assigned.");
                return;
            }
            
            if (_loadReplayButton == null) {
                _logger.LogError(LoggedFeature.Replays, "Load button not assigned.");
                return;
            }

            _dropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (var saveName in _commandHistoryLoader.SavedCommandHistories) {
                options.Add(new Dropdown.OptionData(saveName));
            }
            _dropdown.AddOptions(options);
            
            _loadReplayButton.onClick.AddListener(HandleLoadButtonPressed);
        }

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        public void HandleLoadButtonPressed() {
            string saveName = _dropdown.options[_dropdown.value].text;
            LoadReplayClicked.Invoke(saveName);
        }
    }
}