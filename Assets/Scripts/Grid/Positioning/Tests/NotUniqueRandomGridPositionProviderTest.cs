using NUnit.Framework;
using Zenject;

namespace Grid.Positioning.Tests {
    [TestFixture]
    internal class NotUniqueRandomGridPositionProviderTest : ZenjectUnitTestFixture {
        [SetUp]
        public void CommonInstall() {
            Container.Bind<NotUniqueRandomGridPositionProvider>().AsSingle();
            Container.Inject(this);
        }
        
        [Inject]
        private NotUniqueRandomGridPositionProvider _randomGridPositionProvider;

        [TestCase(-5.02, -5)]
        public void testNoDistanceFromCenter_GetsCenterPosition() {
            
        }
    }
}