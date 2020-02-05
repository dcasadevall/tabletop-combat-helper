namespace Async {
    public class CancellableTaskResult<TResult> {
        public readonly TResult result;
        public readonly bool isCanceled;

        public static CancellableTaskResult<TResult> FromResult(TResult result) {
            return new CancellableTaskResult<TResult>(result, isCanceled: false);
        }
        
        public static CancellableTaskResult<TResult> FromCancellation() {
            return new CancellableTaskResult<TResult>(default(TResult), isCanceled: true);
        }

        private CancellableTaskResult(TResult result, bool isCanceled) {
            this.result = result;
            this.isCanceled = isCanceled;
        }
    }
}