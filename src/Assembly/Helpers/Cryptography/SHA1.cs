using System.Security.Cryptography;
using System.Text;

namespace Assembly.Helpers.Cryptography
{
	public class Sha1Crypto
	{
		public static string ComputeHashToString(string str)
		{
			Encoder enc = Encoding.Unicode.GetEncoder();
			var unicodeText = new byte[str.Length*2];
			enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

			SHA1 sha1 = new SHA1CryptoServiceProvider();
			byte[] result = sha1.ComputeHash(unicodeText);

			var sb = new StringBuilder();
			for (int i = 0; i < result.Length; i++)
			{
				sb.Append(result[i].ToString("X2"));
			}
			return sb.ToString();
		}

		public static string ComputeHashToString(byte[] byteArr)
		{
			SHA1 sha1 = new SHA1CryptoServiceProvider();
			byte[] result = sha1.ComputeHash(byteArr);

			var sb = new StringBuilder();
			for (int i = 0; i < result.Length; i++)
			{
				sb.Append(result[i].ToString("X2"));
			}
			return sb.ToString();
		}

		public static byte[] ComputeHashToByteArray(string str)
		{
			Encoder enc = Encoding.Unicode.GetEncoder();
			var unicodeText = new byte[str.Length*2];
			enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

			SHA1 sha1 = new SHA1CryptoServiceProvider();
			return sha1.ComputeHash(unicodeText);
		}

		public static byte[] ComputeHashToByteArray(byte[] byteArr)
		{
			SHA1 sha1 = new SHA1CryptoServiceProvider();
			return sha1.ComputeHash(byteArr);
		}
	}
}