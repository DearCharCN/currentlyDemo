using System;

public static class BitUtls
{
    public static bool BytesEquals(byte[] a, int aStartIndex, byte[] b, int bStartIndex, int length)
    {
        int aIdx = aStartIndex;
        int bIdx = bStartIndex;
        int index = 0;

        while (index < length)
        {
            if (a.Length <= aIdx || b.Length <= bIdx)
                return false;

            if (!a[aIdx].Equals(b[bIdx]))
                return false;

            ++index;
            ++aIdx;
            ++bIdx;
        }
        return true;
    }

    public static byte[] SubBytes(byte[] bytes, int start, int length)
    {
        byte[] result = new byte[length];
        Array.Copy(bytes, start, result, 0, length);
        return result;
    }

    public static int GetInt32(byte[] data, int startIdx)
    {
        return BitConverter.ToInt32(data, startIdx);
    }
}