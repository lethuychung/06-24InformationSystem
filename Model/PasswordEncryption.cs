using System;
using System.Security.Cryptography;
using System.Text;

namespace _06_24InformationSystem.Model
{
    public class PasswordEncryption
    {
        /// <summary>
        /// MD5 hash, takes an password and returns the hash
        /// </summary>
        /// <param name="thisPassword"></param>
        /// <returns></returns>
        public static string GeneratePasswordHash(string thisPassword)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] tmpSource;
            byte[] tmpHash;
            
            tmpSource = ASCIIEncoding.ASCII.GetBytes(thisPassword); // Turn password into byte array
            tmpHash = md5.ComputeHash(tmpSource);

            StringBuilder sOuput = new StringBuilder(tmpHash.Length);
            for (int i = 0; i < tmpHash.Length; i++)
            {
                sOuput.Append(tmpHash[i].ToString("X2"));  // X2 formats to hexadecimal
            }
            return sOuput.ToString();
        }

        /// <summary>
        /// Validates the password hash, gets unencrypted password and encrypted password and compares them
        /// </summary>
        /// <param name="thisPassword"></param>
        /// <param name="thisHash"></param>
        /// <returns></returns>
        public static Boolean VerifyHashPassword(string thisPassword, string thisHash)
        {
            Boolean IsValid = false;
            string tmpHash = GeneratePasswordHash(thisPassword); // Call the routine on user input
            if (tmpHash == thisHash) IsValid = true;  // Compare to previously generated hash
            return IsValid;
        }

    }
}
