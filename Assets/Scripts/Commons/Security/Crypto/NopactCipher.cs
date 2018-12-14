using System;
using System.Security.Cryptography;
using System.Text;
using nopact.Commons.Utility;

namespace nopact.Commons.Security.Crypto
{
	public sealed class NopactCipher {

		private const string ENC_SEP = "%";

		private static volatile NopactCipher instance;
		private static object syncLock = new System.Object();

		private SHA256 sha256;
	
		private NopactCipher(){		
		}
	
		public static NopactCipher Instance{
		
			get{
			
				if (instance == null){
				
					lock (syncLock){
					
						if (instance == null){
						
							instance = new NopactCipher();
							instance.sha256 = SHA256Managed.Create();
						
						}
					}
				}
		
				return instance;
			}
		}

		public string GenerateAuthenticityCheck(string version, string key) {

			DateTime now = DateTime.UtcNow;
			TimeSpan ts = new TimeSpan(now.Ticks - NopactUtility.unixEpoch.Ticks);
			long millisecs = (long) ts.TotalMilliseconds;

			StringBuilder sb = new StringBuilder();
			sb.Append(millisecs).Append(key);

			string hash = DigestSha256(sb.ToString());

			StringBuilder check = new StringBuilder();
			check.Append(millisecs).Append(ENC_SEP).Append(version).Append(ENC_SEP).Append(hash);

			return check.ToString();

		} 

		public string DigestSha256(string data) {

			return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(data)));
		
		}

		public string DigestSha256(string data, string key) {

			StringBuilder sb = new StringBuilder();

			string toBeHashed = sb.Append(data).Append(key).ToString();

			return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(toBeHashed)));

		}

		public byte[] Xor(byte[] data, byte[] key) {

			if (data.Length != key.Length) {

				throw new ArgumentException("Data and key length are not equal.");

			}

			byte[] result = new byte[data.Length];

			for (int i = 0; i < result.Length; i++) {

				result[i] = (byte) (data[i] ^ key[i]);

			}

			return result;

		}
	
	}
}
