using System.Text;

namespace DearChar.Net
{
    public static class NetConfigration
    {
        const string FLAG = "[Dearchar]";
        public static byte[] FLAGBytes = Encoding.UTF8.GetBytes(FLAG);
    }
}

