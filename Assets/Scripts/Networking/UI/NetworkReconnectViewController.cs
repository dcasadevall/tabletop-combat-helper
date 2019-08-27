using UnityEngine;

namespace Networking.UI {
    public class NetworkReconnectViewController : MonoBehaviour, INetworkReconnectViewController {
        private void Start() {
            Hide();
        }

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }
    }
}