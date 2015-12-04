using System;
using System.Globalization;

namespace UriApp
{
    public class Encoder
    {
        public static string Encode(string decoded)
        {
            string result = "";
            for (int idx = 0; idx < decoded.Length; ++idx)
            {
                if (decoded[idx] < 0x7F && decoded[idx] >= 0x20)
                {
                    result += decoded[idx];
                }
                else
                {
                    result += String.Format("\\u{0,4:X4}", (int)decoded[idx]);
                }
            }
            return result;
        }

        public static string Decode(string encoded)
        {
            string result = "";
            for (int idx = 0; idx < encoded.Length; ++idx)
            {
                if (encoded[idx] == '\\' && encoded[idx + 1] == 'u')
                {
                    char decoded = (char)int.Parse(encoded.Substring(idx + 2, 4), NumberStyles.HexNumber);
                    result += decoded;
                    idx += "\\uABCD".Length - 1;
                }
                else
                {
                    result += encoded[idx];
                }
            }

            return result;
        }
    }
}
