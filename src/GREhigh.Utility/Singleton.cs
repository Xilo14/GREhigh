using System;
using System.Reflection;

namespace GREhigh.Utility {
    public class Singleton<T> where T : class {
        private static T s_instance;

        protected Singleton() {
        }

        private static T CreateInstance() {
            var cInfo = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new Type[0],
                new ParameterModifier[0]);

            return (T)cInfo.Invoke(null);
        }

        public static T Instance {
            get {
                if (s_instance == null) {
                    s_instance = CreateInstance();
                }

                return s_instance;
            }
        }
    }
}
