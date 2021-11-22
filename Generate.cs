using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

namespace LuPerfect.Utils
{
    public enum Lang
    {
        En,
        Ru,
    }

    public static partial class Generate
    {
        public static string Slug(this string? phrase)
        {
            if (phrase is null)
                return string.Empty;

            var str = phrase.ToLowerInvariant();

            // invalid chars
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim
            str = str[..(str.Length <= 45 ? str.Length : 45)].Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens

            return str;
        }

        public static string DigitCode(int length)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", nameof(length));
            if (length > int.MaxValue / 8)
                throw new ArgumentException("length is too big", nameof(length));

            if (length > 1024)
            {
                var builder = new StringBuilder(length);

                for (var i = 0; i < length; i++)
                {
                    builder.Append(Digit());
                }

                return builder.ToString();
            } 
            else
            {
                var str = string.Empty;

                for (var i = 0; i < length; i++)
                {
                    str += Digit();
                }

                return str;
            }
        }

        public static int Integer() => RandomNumberGenerator.GetInt32(int.MinValue, int.MaxValue);
        public static int Digit() => RandomNumberGenerator.GetInt32(10);
        public static int Integer(int max) => RandomNumberGenerator.GetInt32(max);
        public static int Integer(int min, int max) => RandomNumberGenerator.GetInt32(min, max);

        public static string Password(int length = 12)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "1234567890";
            const string special = "!@#$%^&*_-=+";

            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);

            var password = new StringBuilder();
            foreach (byte b in bytes)
            {
                password.Append(RandomNumberGenerator.GetInt32(4) switch
                {
                    0 => lower[b % lower.Length],
                    1 => upper[b % upper.Length],
                    2 => number[b % number.Length],
                    3 => special[b % special.Length],
                    _ => throw new NotImplementedException(),
                });
            }

            return password.ToString();
        }

        public static string String(int length, Lang lang = Lang.Ru)
        {
            const string digits = "0123456789";

            var symbols = lang == Lang.En ? "abcdefghijklmnopqrstuvwxyz" : "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

            var alphanumericCharacters = symbols + symbols.ToUpper() + digits;

            return String(length, alphanumericCharacters);
        }
        public static string String(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", nameof(length));
            if (length > int.MaxValue / 8)
                throw new ArgumentException("length is too big", nameof(length));

            if (characterSet == null)
                throw new ArgumentNullException(nameof(characterSet));

            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", nameof(characterSet));

            var bytes = new byte[length * 8];
            RandomNumberGenerator.Fill(bytes);
            
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }

            return new string(result);
        }
    }
}
