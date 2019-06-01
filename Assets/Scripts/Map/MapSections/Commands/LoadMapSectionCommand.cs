using System;
using CommandSystem;
using UniRx;

namespace Map.MapSections.Commands {
    public class LoadMapSectionCommand : ICommand {
        private readonly LoadMapSectionCommandData _data;
        private readonly MapSectionContext _mapSectionContext;
        private readonly int _numSections;

        public bool IsInitialGameStateCommand {
            get {
                return false;
            }
        }

        private uint _previousSection;

        public LoadMapSectionCommand(LoadMapSectionCommandData data, MapSectionContext mapSectionContext,
                                     IMapData mapData) {
            _data = data;
            _mapSectionContext = mapSectionContext;
            _numSections = mapData.Sections.Length;
        }

        public IObservable<Unit> Run() {
            if (_data.sectionIndex >= _numSections) {
                return Observable.Throw<Unit>(new ArgumentException($"Section Index: [{_data.sectionIndex}] is out of bounds."));
            }

            _previousSection = _mapSectionContext.CurrentSectionIndex;
            _mapSectionContext.CurrentSectionIndex = _data.sectionIndex;
            return Observable.ReturnUnit();
        }

        public void Undo() {
            _mapSectionContext.CurrentSectionIndex = _previousSection;
        }
    }
}