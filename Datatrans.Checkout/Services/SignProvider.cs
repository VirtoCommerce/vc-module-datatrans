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
        private string _hmacHex { get; }

        private byte[] Hmac => _hmac ?? (_hmac = HexDecode(_hmacHex));
        private byte[] _hmac;

        public SignProvider(string hmacKey)
        {
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

            var stringToBeSigned = string.Join(string.Empty, aliasCC, merchantId, amount.ToString(), currency, refno);

            var toSign = new ASCIIEncoding().GetBytes(stringToBeSigned);

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