using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.Tilemaps;

namespace Grid.Tiles.PathTile {
    public class Path : MonoBehaviour {
        
        public LinkedList<PathLink> PathLinks { get; private set; }
        public Dictionary<Vector3Int, LinkedListNode<PathLink>> PathLinkMap { get; private set; }

        public void Awake() {
            PathLinks = new LinkedList<PathLink>();
            PathLinkMap = new Dictionary<Vector3Int, LinkedListNode<PathLink>>();
        }

        public void AddLastLink(Vector3Int pos) {
            var node = PathLinks.AddLast(new PathLink());
            PathLinkMap.Add(pos, node);
        }

        public void RemoveLink(Vector3Int pos) {
            var node = PathLinkMap[pos];
            PathLinks.Remove(node);
            PathLinkMap.Remove(pos);
        }

        public PathLink GetLastLink() {
            return PathLinks.Last?.Value;
        }

        public PathLink GetLink(Vector3Int pos) {
            return PathLinkMap[pos]?.Value;
        }

        public PathLink GetPrevLink(Vector3Int pos) {
            return PathLinkMap[pos].Previous?.Value;
        }

        public PathLink GetNextLink(Vector3Int pos) {
            return PathLinkMap[pos].Next?.Value;
        }

        public bool ContainsTile(Vector3Int pos) {
            return PathLinkMap.ContainsKey(pos);
        }
    }

    public class PathLink {
        public PathType PathType = PathType.Single;
        public Vector3Int Direction = Vector3Int.zero;
        public float Rotation = 0f;
    }
}