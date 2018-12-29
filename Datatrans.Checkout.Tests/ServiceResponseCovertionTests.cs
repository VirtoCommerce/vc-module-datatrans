using Datatrans.Checkout.DatatransClient.Converters;
using Datatrans.Checkout.DatatransClient.Models;
using System.IO;
using VirtoCommerce.Platform.Core.Common;
using Xunit;

namespace Datatrans.Checkout.Tests
{
    public class ServiceResponseTests
    {
        [Fact]
        public void ReadSettleResponse_SuccessfulReponse_ResponseRead()
        {
            //arrange
            var response = File.ReadAllText(@"../../data/settleResponseSuccess.xml");

            //act
            var deserializeXml = response.DeserializeXml<paymentService>();
            var target = deserializeXml.ToCoreModel();

            //assert
            Assert.Equal("01", target.ResponseCode);
            Assert.Equal("settlement succeeded", target.ResponseMessage);
        }

        [Fact]
        public void ReadSettleResponse_FailedReponse_ResponseRead()
        {
            var response = File.ReadAllText(@"../../data/settleResponseError.xml");

            var deserializeXml = response.DeserializeXml<paymentService>();
            var target = deserializeXml.ToCoreModel();

            //assert
            Assert.Equal("-80", target.ErrorCode);
            Assert.Equal("UPP record not found", target.ErrorMessage);
        }

        [Fact]
        public void StatuesResponse_SuccessfulReponse_ResponseRead()
        {
            var response = File.ReadAllText(@"../../data/statusResponseSuccess.xml");

            var deserializeXml = response.DeserializeXml<statusService>();
            var target = deserializeXml.ToCoreModel();

            //assert
            Assert.Equal("2", target.ResponseCode);
            Assert.Equal("Trx debit waiting for daily settlement process", target.ResponseMessage);
            Assert.Equal("00400", target.ReferenceNumber);
            Assert.Equal("24720", target.Amount);
            Assert.Equal("USD", target.Currency);
            Assert.Equal("9213", target.AuthorizationCode);
            Assert.Equal("VIS", target.PaymentMethod);
            Assert.Equal("111111111111111", target.TransactionId);
            Assert.Equal("424242xxxxxx4242", target.MaskedCC);
            Assert.Equal("12", target.ExpirationMonth);
            Assert.Equal("18", target.ExpirationYear);
            Assert.Equal("20171212", target.TransactionDate);
            Assert.Equal("222638", target.TransactionTime);
            Assert.Equal("05", target.TransactionType);
            Assert.Equal("24720", target.SettledAmount);
        }

        [Fact]
        public void StatusSettleResponse_FailedReponse_ResponseRead()
        {
            var response = File.ReadAllText(@"../../data/statusResponseError.xml");

            var deserializeXml = response.DeserializeXml<statusService>();
            var target = deserializeXml.ToCoreModel();

            //assert
            Assert.Equal("2022", target.ErrorCode);
            Assert.Equal("invalid value", target.ErrorMessage);
            Assert.Equal("merchantId", target.ErrorDetail);
        }

        [Fact]
        public void TestRefundResponseSuccessDeserializing()
        {
            var response = File.ReadAllText(@"../../data/refundResponseSuccess.xml");

            var deserializedXml = response.DeserializeXml<RefundResponse.paymentService>();

            var refundResponse = deserializedXml.ToCoreModel();

            Assert.NotEmpty(refundResponse.ResponseCode);
            Assert.NotEmpty(refundResponse.ResponseMessage);
            Assert.NotEmpty(refundResponse.TransactionId);

            Assert.Null(refundResponse.ResponseData);
            Assert.Null(refundResponse.ErrorMessage);
            Assert.Null(refundResponse.ErrorCode);
            Assert.Null(refundResponse.ErrorDetail);
        }

        [Fact]
        public void TestRefundResponseErrorDeserializing()
        {
            var response = File.ReadAllText(@"../../data/refundResponseError.xml");

            var deserializedXml = response.DeserializeXml<RefundResponse.paymentService>();

            var refundResponse = deserializedXml.ToCoreModel();

            Assert.NotEmpty(refundResponse.ErrorMessage);
            Assert.NotEmpty(refundResponse.ErrorCode);
            Assert.NotEmpty(refundResponse.ErrorDetail);

            Assert.Null(refundResponse.ResponseData);
            Assert.Null(refundResponse.ResponseCode);
            Assert.Null(refundResponse.ResponseMessage);
            Assert.Null(refundResponse.TransactionId);
        }
    }
}
