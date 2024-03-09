using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace kWMS.Extras
{
    public class Utils
    {

        public static string EncryptPassword(string password)
        {
            string strPlainText = password;
            byte[] hashedDataBytes;
            UTF8Encoding encoder = new UTF8Encoding();
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(strPlainText));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashedDataBytes.Length; i++)
            {
                sb.Append(hashedDataBytes.GetValue(i).ToString());
            }
            return sb.ToString();


        }
        public static string EncryptPasswordold(string Message, string Passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToEncrypt = UTF8.GetBytes(Message);
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            return Convert.ToBase64String(Results);
        }

        public static string Decrypt(string TextToBeDecrypted)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();
            string Password = "encryptprescriptionid";
            string DecryptedData;
            try
            {
                byte[] EncryptedData = Convert.FromBase64String(TextToBeDecrypted);
                byte[] Salt = Encoding.ASCII.GetBytes(Password.Length.ToString());
                //Making of the key for decryption          
                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(Password, Salt);
                //Creates a symmetric Rijndael decryptor object.   
                ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
                MemoryStream memoryStream = new MemoryStream(EncryptedData);
                //Defines the cryptographics stream for decryption.THe stream contains decrpted data  
                CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);
                byte[] PlainText = new byte[EncryptedData.Length];
                int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
                memoryStream.Close();
                cryptoStream.Close();
                //Converting to string  
                DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
            }
            catch
            {
                DecryptedData = TextToBeDecrypted;
            }
            return DecryptedData;
        }

        public static string DecryptPassword(string Message, string Passphrase)
        {
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB; TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToDecrypt = Convert.FromBase64String(Message);
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            return UTF8.GetString(Results);
        }
        public static string EncryptPassword1(string password)
        {
            string strPlainText = password;
            byte[] hashedDataBytes;
            UTF8Encoding encoder = new UTF8Encoding();
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(strPlainText));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashedDataBytes.Length; i++)
            {
                sb.Append(hashedDataBytes.GetValue(i).ToString());
            }
            return sb.ToString();
        }

    }
}
