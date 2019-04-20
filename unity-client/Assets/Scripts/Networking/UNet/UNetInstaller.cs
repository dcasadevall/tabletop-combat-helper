using System;
using Networking.Messaging;
using Networking.UNet.Messaging;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using Zenject;

namespace Networking.UNet {
    public class UNetInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind(typeof(IInitializable), typeof(INetworkManager)).To<UNetMatchmakingNetworkManager>().AsSingle();
            Container.Bind(typeof(INetworkMessageHandler), typeof(IInitializable), typeof(IDisposable))
                     .To<UNetNetworkMessageHandler>()
                     .AsSingle();
        }
    }
}