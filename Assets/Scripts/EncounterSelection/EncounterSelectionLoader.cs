
using System;
using CommandSystem;
using Map.MapData.Store.Commands;
using Map.MapSelection;
using Replays.Persistence;
using Replays.Persistence.UI;
using Zenject;

namespace EncounterSelection {
    public class EncounterSelectionLoader : IInitializable, IDisposable {
        private readonly EncounterSelectionContext _encounterSelectionContext;
        private readonly ICommandQueue _commandQueue;
        private readonly ICommandHistoryLoader _commandHistoryLoader;
        private readonly IMapSelectViewController _mapSelectViewController;
        private readonly IReplayLoaderViewController _replayLoaderViewController;

        public EncounterSelectionLoader(EncounterSelectionContext encounterSelectionContext,
                                        ICommandQueue commandQueue,
                                        ICommandHistoryLoader commandHistoryLoader,
                                        IMapSelectViewController mapSelectViewController,
                                        IReplayLoaderViewController replayLoaderViewController) {
            _encounterSelectionContext = encounterSelectionContext;
            _commandQueue = commandQueue;
            _commandHistoryLoader = commandHistoryLoader;
            _mapSelectViewController = mapSelectViewController;
            _replayLoaderViewController = replayLoaderViewController;
        }

        public void Initialize() {
            _mapSelectViewController.LoadMapClicked += HandleLoadMapClicked;
            _mapSelectViewController.EditMapClicked += HandleEditMapClicked;
            _replayLoaderViewController.LoadReplayClicked += HandleLoadReplayClicked;
            
            _mapSelectViewController.Show();
            _replayLoaderViewController.Show();
        }

        public void Dispose() {
            _mapSelectViewController.LoadMapClicked -= HandleLoadMapClicked;
            _mapSelectViewController.EditMapClicked -= HandleEditMapClicked;
            _replayLoaderViewController.LoadReplayClicked -= HandleLoadReplayClicked;
        }

        private void HandleEditMapClicked(int mapIndex) {
            _mapSelectViewController.LoadMapClicked -= HandleLoadMapClicked;
            _replayLoaderViewController.LoadReplayClicked -= HandleLoadReplayClicked;
            _mapSelectViewController.Hide();
            _replayLoaderViewController.Hide();

            _encounterSelectionContext.EncounterType = EncounterType.EditMap;
            LoadMapCommandData commandData = new LoadMapCommandData((uint) mapIndex, isMapEditor: true);
            _commandQueue.Enqueue<LoadMapCommand, LoadMapCommandData>(commandData, CommandSource.Game);
        }

        private void HandleLoadMapClicked(int mapIndex) {
            _mapSelectViewController.LoadMapClicked -= HandleLoadMapClicked;
            _replayLoaderViewController.LoadReplayClicked -= HandleLoadReplayClicked;
            _mapSelectViewController.Hide();
            _replayLoaderViewController.Hide();

            _encounterSelectionContext.EncounterType = EncounterType.Combat;
            LoadMapCommandData commandData = new LoadMapCommandData((uint)mapIndex);
            _commandQueue.Enqueue<LoadMapCommand, LoadMapCommandData>(commandData, CommandSource.Game);
        }
        
        private void HandleLoadReplayClicked(string saveName) {
            _replayLoaderViewController.LoadReplayClicked -= HandleLoadReplayClicked;
            _mapSelectViewController.LoadMapClicked -= HandleLoadMapClicked;
            _replayLoaderViewController.Hide();
            _mapSelectViewController.Hide();
            
            _encounterSelectionContext.EncounterType = EncounterType.Replay;
            _commandHistoryLoader.LoadCommandHistory(saveName);
        }
    }
}