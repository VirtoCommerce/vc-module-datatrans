using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("Datatrans.Checkout.Tests")]
namespace Datatrans.Checkout.Services
{
    internal class SignProvider : ISignProvider
    {
        private readonly string _hmacHex;

        private byte[] Hmac => _hmac ?? (_hmac = HexDecode(_hmacHex));
        private byte[] _hmac;

        public SignProvider(string hmacKey)
        {
            if (string.IsNullOrEmpty(hmacKey))
            {
                throw new ArgumentNullException(nameof(hmacKey));
            }

            _hmacHex = hmacKey;
        }

        public string Sign(string merchantId, int amount, string currency, string refno, string aliasCC = null)
        {
            if (merchantId == null)
            {
                throw new ArgumentNullException(nameof(merchantId));
            }

            if (currency == null)
            {
                throw new ArgumentNullException(nameof(currency));
            }

            if (refno == null)
            {
                throw new ArgumentNullException(nameof(refno));
            }

            var targetData = string.Join(string.Empty, aliasCC, merchantId, amount.ToString(), currency, refno);

            return GenerateSignature(targetData);
        }

        public bool ValidateSignature(string signature, string merchantId, int amount, string currency, string transactionId)
        {
            var targetData = string.Join(string.Empty, merchantId, amount.ToString(), currency, transactionId);

            var expectedSignature = GenerateSignature(targetData);

            return expectedSignature == signature;
        }

        private string GenerateSignature(string source)
        {
            var toSign = Encoding.ASCII.GetBytes(source);

            using (var hmac = new HMACSHA256(Hmac))
            {
                var hash = hmac.ComputeHash(toSign);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            }
        }

        /// <summary>
        /// https://stackoverflow.com/a/12187656
        /// </summary>
        private byte[] HexDecode(string hex)
        {
            // In these case, key is not a string, it is byte array in hexadecimal string representation
            var bytes = new byte[hex.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return bytes;
        }
    }
}