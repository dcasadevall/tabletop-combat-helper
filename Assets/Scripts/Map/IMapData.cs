using System;
using Grid.Serialized;
using UnityEngine;

namespace Map {
    public interface IMapData {
        IGridData GridData { get; }
        String Name { get; }
        Sprite BackgroundSprite { get; }
    }
}