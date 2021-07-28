using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SPCS.Common.Helper
{
	public class RSAHelper
	{

		/// <summary>
		/// Create a pair of keys 
		/// </summary>
		/// <param name="strength"></param>
		/// <returns></returns>
		public static (string, string) CreateKey(int strength = 2048)
		{
			RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
			rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), strength));
			AsymmetricCipherKeyPair keys = rsaKeyPairGenerator.GenerateKeyPair();
			TextWriter privateTextWriter = new StringWriter();
			PemWriter pemWriter = new PemWriter(privateTextWriter);
			pemWriter.WriteObject(keys.Private);
			pemWriter.Writer.Flush();
			StringWriter stringWriter = new StringWriter();
			PemWriter pemWriter2 = new PemWriter(stringWriter);
			pemWriter2.WriteObject(keys.Public);
			pemWriter2.Writer.Flush();
			return (stringWriter.ToString(), privateTextWriter.ToString());
		}

		/// <summary>
		/// RSA decrypt method
		/// </summary>
		/// <param name="privateKey">SA algorithm private key</param>
		/// <param name="decryptstring">A string that need to be decrypted</param>
		/// <returns></returns>
		public static string Decrypt(string privateKey, string decryptstring)
		{
			using (TextReader reader = new StringReader(privateKey))
			{
				dynamic key = new PemReader(reader).ReadObject();
				Pkcs1Encoding rsaDecrypt = new Pkcs1Encoding(new RsaEngine());
				if (key is AsymmetricKeyParameter)
				{
					key = (AsymmetricKeyParameter)key;
				}
				else if (key is AsymmetricCipherKeyPair)
				{
					key = ((AsymmetricCipherKeyPair)key).Private;
				}
				rsaDecrypt.Init(false, key);
				byte[] entData2 = Convert.FromBase64String(decryptstring);
				entData2 = rsaDecrypt.ProcessBlock(entData2, 0, entData2.Length);
				return Encoding.UTF8.GetString(entData2);
			}
		}
		/// <summary>
		/// this is RSA encrypt method.
		/// </summary>
		/// <param name="publicKey">RSA algorithm public key</param>
		/// <param name="encryptstring">A string that needs to be encrypted</param>
		/// <returns></returns>
		public static string Encrypt(string publicKey, string encryptstring)
		{
			using (TextReader reader = new StringReader(publicKey))
			{
				AsymmetricKeyParameter key = new PemReader(reader).ReadObject() as AsymmetricKeyParameter;
				Pkcs1Encoding pkcs1Encoding = new Pkcs1Encoding(new RsaEngine());
				pkcs1Encoding.Init(forEncryption: true, key);
				byte[] entData2 = Encoding.UTF8.GetBytes(encryptstring);
				entData2 = pkcs1Encoding.ProcessBlock(entData2, 0, entData2.Length);
				return Convert.ToBase64String(entData2);
			}
		}

		public static string PrivateKey { get; set; }
		public static string PublicKey { get; set; }

		static RSAHelper()
		{
			var key = CreateKey(2048);
			PublicKey = key.Item1;
			PrivateKey = key.Item2;
		}
	}


}
