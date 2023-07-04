﻿using System.Security.Cryptography;
using System.Text;

namespace CatalogService.Api.Utilities.Encryption
{
    public class HashCreator
    {
        public static String Sha256_Hash(params string[] values)
        {
            var value = string.Join('-', values);

            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
