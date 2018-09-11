using System;
using System.Collections.Specialized;
using Datatrans.Checkout.Core.Event;
using Datatrans.Checkout.Core.Model;
using Datatrans.Checkout.Core.Services;
using Datatrans.Checkout.Helpers;
using VirtoCommerce.Domain.Order.Events;
using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace Datatrans.Checkout.Managers
{
    public class DatatransCheckoutPaymentMethod : PaymentMethod
    {
        private const string _merchantIdSetting = "Datatrans.Checkout.MerchantId";
        private const string _signSetting = "Datatrans.Checkout.Sign";

        private const string _datatransModeStoreSetting = "Datatrans.Checkout.Mode";
        private const string _paymentActionTypeSetting = "Datatrans.Checkout.PaymentActionType";
        private const string _paymentMethodSetting = "Datatrans.Checkout.PaymentMethod";
        private const string _formActionUrlSetting = "Datatrans.Checkout.FormAction";
        private const string _languageSetting = "Datatrans.Checkout.Language";

        private const string _transactionParamName = "uppTransactionId";
        private const string _paymentMethodCodeParamName = "paymentMethodCode";
        private const string _merchantIdParamName = "merchant";

        #region Settings        

        private string ApiMode
        {
            get
            {
                return GetSetting(_datatransModeStoreSetting);
            }
        }

        private string MerchantId
        {
            get
            {                
                return GetSetting(_merchantIdSetting);
            }
        }

        private string Sign
        {
            get
            {
                return GetSetting(_signSetting);
            }
        }

        private string PaymentAction
        {
            get
            {
                return GetSetting(_paymentActionTypeSetting);
            }
        }

        private string PaymentMethod
        {
            get
            {
                return GetSetting(_paymentMethodSetting);
            }
        }

        private string FormActionUrl
        {
            get
            {
                return GetSetting(_formActionUrlSetting);
            }
        }

        private string Language
        {
            get
            {
                return GetSetting(_languageSetting);
            }
        }

        public bool IsSale
        {
            get { return PaymentAction.EqualsInvariant("Sale"); }
        }

        public bool IsTest
        {
            get { return ApiMode.EqualsInvariant("test"); }
        }

        public string ServerToServerApi
        {
            get
            {
                var live = "https://api.sandbox.datatrans.com";
                var sandbox = "https://api.sandbox.datatrans.com";
                return IsTest ? sandbox : live;
            }
        }

        public string FrontendApi
        {
            get
            {
                var live = "https://pay.sandbox.datatrans.com";
                var sandbox = "https://pay.sandbox.datatrans.com";
                return IsTest ? sandbox : live;
            }
        }

        #endregion

        public override PaymentMethodType PaymentMethodType
        {
            get { return PaymentMethodType.PreparedForm; }
        }

        public override PaymentMethodGroupType PaymentMethodGroupType
        {
            get { return PaymentMethodGroupType.Alternative; }
        }

        private readonly IDatatransCheckoutService _datatransCheckoutService;
        private readonly Func<string, IDatatransClient> _datatransClientFactory;
        private readonly IEventPublisher<DatatransBeforeCapturePaymentEvent> _settlemntEventPublisher;
        private readonly IDatatransCapturePaymentService _capturePaymentService;

        public DatatransCheckoutPaymentMethod(IDatatransCheckoutService datatransCheckoutService, Func<string, IDatatransClient> datatransClientFactory, IEventPublisher<DatatransBeforeCapturePaymentEvent> settlemntEventPublisher, IDatatransCapturePaymentService capturePaymentService) : 
            base("DatatransCheckout")
        {
            _datatransCheckoutService = datatransCheckoutService;
            _datatransClientFactory = datatransClientFactory;
            _settlemntEventPublisher = settlemntEventPublisher;
            _capturePaymentService = capturePaymentService;
        }

        public override ProcessPaymentResult ProcessPayment(ProcessPaymentEvaluationContext context)
        {
            var result = new ProcessPaymentResult();
            if (context.Order != null && context.Store != null)
            {
                result = PrepareFormContent(context);
            }
            return result;
        }

        private ProcessPaymentResult PrepareFormContent(ProcessPaymentEvaluationContext context)
        {
            var formContent = _datatransCheckoutService.GetCheckoutFormContent(new DatatransCheckoutSettings
            {
                Order = context.Order,
                Store = context.Store,
                MerchantId = MerchantId,
                FormActionUrl = FormActionUrl,
                PaymentAction = PaymentAction,
                PaymentMethod = PaymentMethod,
                ReferenceNumber = context.Order.Number,
                Sign = Sign,
                Amount = context.Order.Sum.ToInt(),
                PurchaseCurrency = context.Order.Currency,
                FrontendApi = FrontendApi,
                Language = Language,
                InternalPaymentMethodCode = Code,
                PaymentMethodCodeParamName = _paymentMethodCodeParamName
            });

            var result = new ProcessPaymentResult();
            result.IsSuccess = true;
            result.NewPaymentStatus = context.Payment.PaymentStatus = PaymentStatus.Pending;
            result.HtmlForm = formContent;
            result.OuterId = null;

            return result;
        }

        public override PostProcessPaymentResult PostProcessPayment(PostProcessPaymentEvaluationContext context)
        {
            var result = new PostProcessPaymentResult();

            var status = GetParamValue(context.Parameters, "status"); //possible values: error, success
            var responseMessage = GetParamValue(context.Parameters, "responseMessage");
            var transactionId = GetParamValue(context.Parameters, _transactionParamName);

            context.Payment.OuterId = context.OuterId;
            if (status.EqualsInvariant("success") && IsSale)
            {
                var captureResult = CaptureProcessPayment(new CaptureProcessPaymentEvaluationContext
                {
                    Payment = context.Payment,
                    Order = context.Order,
                    Parameters = context.Parameters
                });

                if (captureResult.IsSuccess)
                {
                    context.OuterId = transactionId;
                    result.NewPaymentStatus = context.Payment.PaymentStatus = PaymentStatus.Paid;
                    context.Payment.IsApproved = true;
                    context.Payment.CapturedDate = DateTime.UtcNow;
                    result.IsSuccess = true;
                }
            }
            else if (status.EqualsInvariant("success"))
            {
                var transactionInfo = GetTransactionStatus(context.Payment.OuterId);
                context.Payment.Transactions.Add(new PaymentGatewayTransaction()
                {
                    Note = "Transaction Info",
                    ResponseData = transactionInfo.ResponseContent,
                    Status = transactionInfo.ResponseMessage,
                    ResponseCode = transactionInfo.ResponseCode,
                    ProcessError = transactionInfo.ErrorMessage,
                    CurrencyCode = context.Order.Currency,
                    Amount = context.Order.Sum,
                    IsProcessed = true,
                    ProcessedDate = DateTime.UtcNow
            });

                result.NewPaymentStatus = context.Payment.PaymentStatus = PaymentStatus.Authorized;
                context.Payment.OuterId = result.OuterId = context.OuterId;
                context.Payment.AuthorizedDate = DateTime.UtcNow;
                result.IsSuccess = true;
            }
            else
            {
                var errorMessage = GetParamValue(context.Parameters, "errorMessage");
                result.ErrorMessage = $"Order was not created. {errorMessage}";
            }

            result.OrderId = context.Order.Id;
            return result;
        }

        public override CaptureProcessPaymentResult CaptureProcessPayment(CaptureProcessPaymentEvaluationContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.Payment == null)
                throw new ArgumentNullException("context.Payment");

            if (context.Order == null)
                throw new ArgumentNullException("context.Order");

            var getAirlineContext = new GetAirlineDataContext(context.Order, context.Payment, context.Parameters);
            var airlineData = _capturePaymentService.GetAirlineData(getAirlineContext);

            var beforeSettlementEvent = new DatatransBeforeCapturePaymentEvent(context.Order, context.Payment, context.Parameters, airlineData);
            _settlemntEventPublisher.Publish(beforeSettlementEvent);

            var request = new DatatransSettlementRequest
            {
                MerchangId = MerchantId,
                TransactionId = context.Payment.OuterId,
                ReferenceNumber = context.Order.Number,
                Amount = context.Payment.Sum.ToInt(),                
                Currency = context.Order.Currency,
                AirlineData = airlineData
            };

            var paymentTransaction = new PaymentGatewayTransaction
            {
                Note = "Settle Transaction",
                CurrencyCode = context.Order.Currency,
                Amount = context.Payment.Sum
            };
            context.Payment.Transactions.Add(paymentTransaction);

            var result = new CaptureProcessPaymentResult();
            var datatransClient = _datatransClientFactory(ServerToServerApi);
            var settleResult = datatransClient.SettleTransaction(request);
            if (!settleResult.ErrorMessage.IsNullOrEmpty())
            {
                result.ErrorMessage = settleResult.ErrorMessage;
                paymentTransaction.ResponseData = settleResult.ResponseContent;
                return result;
            }

            var transactionInfo = GetTransactionStatus(context.Payment.OuterId);
            paymentTransaction.Note = "Transaction Info after Settle Transaction";
            paymentTransaction.Status = transactionInfo.ResponseMessage;
            paymentTransaction.ResponseData = transactionInfo.ResponseContent;
            paymentTransaction.ResponseCode = transactionInfo.ResponseCode;
            paymentTransaction.ProcessError = transactionInfo.ErrorMessage;
            paymentTransaction.IsProcessed = true;
            paymentTransaction.ProcessedDate = DateTime.UtcNow;

            result.NewPaymentStatus = context.Payment.PaymentStatus = PaymentStatus.Paid;
            context.Payment.CapturedDate = DateTime.UtcNow;
            context.Payment.IsApproved = true;
            result.IsSuccess = true;
            result.OuterId = context.Payment.OuterId;

            return result;
        }

        protected virtual DatatransTransactionResponse GetTransactionStatus(string transactionId)
        {
            var datatransClient = _datatransClientFactory(ServerToServerApi);
            return datatransClient.GetTransactionStatus(new DatatransTransactionRequest()
            {
                MerchangId = MerchantId,
                TransactionId = transactionId
            });
        }

        public override RefundProcessPaymentResult RefundProcessPayment(RefundProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        public override VoidProcessPaymentResult VoidProcessPayment(VoidProcessPaymentEvaluationContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check for transaction Id and payment method code in returned params collection
        /// This method is repsonsible for selecting this particular payment method among all active store payment methods
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public override ValidatePostProcessRequestResult ValidatePostProcessRequest(NameValueCollection queryString)
        {
            var transactionId = GetParamValue(queryString, _transactionParamName);
            var paymentMethodName = GetParamValue(queryString, _paymentMethodCodeParamName);

            return new ValidatePostProcessRequestResult
            {
                IsSuccess = !string.IsNullOrEmpty(transactionId) && paymentMethodName.EqualsInvariant(Code),
                OuterId = transactionId
            };
        }

        private string GetParamValue(NameValueCollection queryString, string paramName)
        {
            if (queryString == null || !queryString.HasKeys())
            {
                return null;
            }

            return queryString.Get(paramName);
        }
    }
}