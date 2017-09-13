#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("ySQN4m/ghqcQJ+OxJm+ZS5woZKbUZtsoB+6GQcoqx5Cc+vva/Se9cIF+EVMEpIROiOuw9/tBGLxP1E9s/ACnRgY1gEgi8y/jDRd67dZOsE8amZeYqBqZkpoamZmYKH/5bRtQriXnEOBt18gNO6Ywv5VnW11BtUG4Qr0n+6gmYQCJ0KwlHxu6hckcGOjA1E/emHNBtOOhqbhsYnzGAZ2lxJPRWquRXHRO+WNQ+vmzINidczfJqBqZuqiVnpGyHtAeb5WZmZmdmJuSCytpZXWnXCWt0NxO8VUluXyoCZ/ZQMkV7GuTFaOIv6Gn4NtWoxxC8Rk4h4B5Wimnihc8vDhLVR5NGZrR3OhHN0o+QRkhw0ef8dhFfeTSggaq0WsExdac35qbmZiZ");
        private static int[] order = new int[] { 3,13,10,11,13,10,10,12,10,11,11,12,13,13,14 };
        private static int key = 152;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
