using System;
using Networking.Messaging;
using UnityEngine.Networking.Match;
using Zenject;

namespace Networking.UNet {
    public class UNetInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<NetworkMatch>().FromNewComponentOnNewGameObject().AsSingle();
            Container.Bind<INetworkManager>().To<UNetMatchmakingNetworkManager>().AsSingle();
            Container.Bind(typeof(INetworkMessageHandler), typeof(IInitializable), typeof(IDisposable))
                     .To<UNetNetworkMessageHandler>()
                     .AsSingle();
        }
    }
}