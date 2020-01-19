using System;
using UniRx.Async;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Map.Serialized {
    [Serializable]
    public class MapReference : IMapReference, IMutableMapReference {
        public string name;
        public string MapName {
            get {
                return name;
            }
        }


        public AssetReference mapData;
        async UniTask<IMapData> IMapReference.LoadMap() {
            // Reuse the method that returns the mutable object.
            // Can't cast, so await then return the value.
            MapData loadedData = await LoadMap();
            return loadedData;
        }

        public async UniTask<MapData> LoadMap() {
            var handle = mapData.LoadAssetAsync<MapData>();
            await UniTask.WaitUntil(() => handle.IsDone)
                         .Timeout(TimeSpan.FromSeconds(10));
            
            if (handle.Status != AsyncOperationStatus.Succeeded) {
                throw new Exception($"Failed to load map: {name}");
            }

            return handle.Result;
        }
    }
}