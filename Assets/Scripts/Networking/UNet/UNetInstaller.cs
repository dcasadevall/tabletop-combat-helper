using System;
using Networking.Messaging;
using Zenject;

namespace Networking.UNet {
    public class UNetInstaller : Installer {
        public override void InstallBindings() {
            Container.Bind<INetworkManager>().To<UNetNetworkManager>().AsSingle();
            Container.Bind(typeof(INetworkMessageHandler), typeof(IInitializable), typeof(IDisposable))
                     .To<UNetNetworkMessageHandler>()
                     .AsSingle();
        }
    }
}