using System;
using UnityEngine;

namespace Networking {
    /// <summary>
    /// A serializable implementation of <see cref="INetworkSettings"/>.
    /// </summary>
    [Serializable]
    public class SerializableNetworkSettings : INetworkSettings {
        #pragma warning disable 649
        [SerializeField] 
        private int _port = 7777;
        [SerializeField]
        private string _address = "localhost";
        #pragma warning restore 649
        
        public int Port {
            get {
                return _port;
            }
        }
        
        public string Address {
            get {
                return _address;
            }
        }
    }
}