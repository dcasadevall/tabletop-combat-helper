using System;

namespace Utils {
    public class Preconditions {
        public static void CheckNotNull(object @object, string msg = "Null object found") {
            if (@object == null) {
                throw new Exception(msg);
            }
        }
        
        public static void CheckNotNull(params object[] objects) {
            foreach (var obj in objects) {
                CheckNotNull(obj);
            }
        }
    }
}