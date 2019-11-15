using System;
using UniRx;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Map.Serialized {
    [Serializable]
    public class MapReference : IMapReference {
        public string name;
        public string MapName {
            get {
                return name;
            }
        }


        public AssetReference mapData;
        public IObservable<IMapData> LoadMap() {
            var handle = mapData.LoadAssetAsync<MapData>();
            return Observable.EveryUpdate()
                             .Where(_ => handle.IsDone)
                             .FirstOrDefault()
                             .Timeout(TimeSpan.FromSeconds(10))
                             .Select(_ => {
                                 if (handle.Status != AsyncOperationStatus.Succeeded) {
                                     throw new Exception($"Failed to load map: {name}");
                                 }

                                 return handle.Result;
                             });
        }
    }
}