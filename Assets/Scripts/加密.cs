using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class 加密
{
	public const string Key = "vjkasrjhwlksdog7";

	public const string IV = "jksdfhlkjlkheopg";

	public static string EncryptString(string plainText)
	{
		if (plainText == null || plainText.Length <= 0)
		{
			throw new ArgumentNullException("plainText");
		}
		if ("vjkasrjhwlksdog7".Length <= 0)
		{
			throw new ArgumentNullException("Key");
		}
		if ("jksdfhlkjlkheopg".Length <= 0)
		{
			throw new ArgumentNullException("IV");
		}
		byte[] inArray;
		using (Aes aes = Aes.Create())
		{
			aes.Key = Encoding.ASCII.GetBytes("vjkasrjhwlksdog7");
			aes.IV = Encoding.ASCII.GetBytes("jksdfhlkjlkheopg");
			ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
				{
					using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
					{
						streamWriter.Write(plainText);
					}
					inArray = memoryStream.ToArray();
				}
			}
		}
		return Convert.ToBase64String(inArray);
	}

	public static string DecryptString(string cipherStr)
	{
		byte[] array = Convert.FromBase64String(cipherStr);
		if (array == null || array.Length == 0)
		{
			throw new ArgumentNullException("cipherText");
		}
		if ("vjkasrjhwlksdog7".Length <= 0)
		{
			throw new ArgumentNullException("Key");
		}
		if ("jksdfhlkjlkheopg".Length <= 0)
		{
			throw new ArgumentNullException("IV");
		}
		string text = null;
		using (Aes aes = Aes.Create())
		{
			aes.Key = Encoding.ASCII.GetBytes("vjkasrjhwlksdog7");
			aes.IV = Encoding.ASCII.GetBytes("jksdfhlkjlkheopg");
			ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
			using (MemoryStream stream = new MemoryStream(array))
			{
				using (CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read))
				{
					using (StreamReader streamReader = new StreamReader(stream2, Encoding.UTF8))
					{
						return streamReader.ReadToEnd();
					}
				}
			}
		}
	}
}
