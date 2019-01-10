using Datatrans.Checkout.Services;
using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Datatrans.Checkout.Tests
{
    public class SignProviderTests
    {
        [Fact]
        public void TestSignature()
        {
            var testSignatureKey = "testSignature";

            var signProvider = CreateSignProvider(GetHexadecimalString(testSignatureKey));

            var merchantId = "testMerchant";
            var amount = 123;
            var currency = "USD";
            var refno = "testRefno";

            var sign = signProvider.Sign(merchantId, amount, currency, refno);
            var expectedSign = CreateSignature(GetStringBytesArray(testSignatureKey), $"{merchantId}{amount.ToString()}{currency}{refno}");

            Assert.Equal(expectedSign, sign);
        }

        private SignProvider CreateSignProvider(string hex)
        {
            return new SignProvider(hex);
        }

        private string GetHexadecimalString(string key)
        {
            var bytes = GetStringBytesArray(key);

            return ConvertBytesArrayToHexadecimalString(bytes);
        }

        private byte[] GetStringBytesArray(string source)
        {
            return new ASCIIEncoding().GetBytes(source);
        }

        private string CreateSignature(byte[] bytes, string strToSign)
        {
            var toSign = GetStringBytesArray(strToSign);

            using (var hmac = new HMACSHA256(bytes))
            {
                var hash = hmac.ComputeHash(toSign);
                return ConvertBytesArrayToHexadecimalString(hash);
            }
        }

        private static string ConvertBytesArrayToHexadecimalString(byte[] source)
        {
            return BitConverter.ToString(source).Replace("-", string.Empty).ToLower();
        }
    }
}
