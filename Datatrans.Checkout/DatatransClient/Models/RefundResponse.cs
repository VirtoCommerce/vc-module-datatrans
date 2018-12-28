﻿namespace Datatrans.Checkout.DatatransClient.Models
{
    public class RefundResponse
    {
        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class paymentService
        {

            private paymentServiceBody[] bodyField;

            private string versionField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("body", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public paymentServiceBody[] body
            {
                get
                {
                    return this.bodyField;
                }
                set
                {
                    this.bodyField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string version
            {
                get
                {
                    return this.versionField;
                }
                set
                {
                    this.versionField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class paymentServiceBody
        {

            private paymentServiceBodyTransaction[] transactionField;

            private string merchantIdField;

            private string statusField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("transaction", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public paymentServiceBodyTransaction[] transaction
            {
                get
                {
                    return this.transactionField;
                }
                set
                {
                    this.transactionField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string merchantId
            {
                get
                {
                    return this.merchantIdField;
                }
                set
                {
                    this.merchantIdField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string status
            {
                get
                {
                    return this.statusField;
                }
                set
                {
                    this.statusField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class paymentServiceBodyTransaction
        {

            private paymentServiceBodyTransactionRequest[] requestField;

            private paymentServiceBodyTransactionResponse[] responseField;

            private string refnoField;

            private string trxStatusField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("request", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public paymentServiceBodyTransactionRequest[] request
            {
                get
                {
                    return this.requestField;
                }
                set
                {
                    this.requestField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("response", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public paymentServiceBodyTransactionResponse[] response
            {
                get
                {
                    return this.responseField;
                }
                set
                {
                    this.responseField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string refno
            {
                get
                {
                    return this.refnoField;
                }
                set
                {
                    this.refnoField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string trxStatus
            {
                get
                {
                    return this.trxStatusField;
                }
                set
                {
                    this.trxStatusField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class paymentServiceBodyTransactionRequest
        {

            private string amountField;

            private string currencyField;

            private string uppTransactionIdField;

            private string transtypeField;

            private string reqtypeField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string amount
            {
                get
                {
                    return this.amountField;
                }
                set
                {
                    this.amountField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string currency
            {
                get
                {
                    return this.currencyField;
                }
                set
                {
                    this.currencyField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string uppTransactionId
            {
                get
                {
                    return this.uppTransactionIdField;
                }
                set
                {
                    this.uppTransactionIdField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string transtype
            {
                get
                {
                    return this.transtypeField;
                }
                set
                {
                    this.transtypeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string reqtype
            {
                get
                {
                    return this.reqtypeField;
                }
                set
                {
                    this.reqtypeField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class paymentServiceBodyTransactionResponse
        {

            private string responseCodeField;

            private string responseMessageField;

            private string uppTransactionIdField;

            private string authorizationCodeField;

            private string acqAuthorizationCodeField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string responseCode
            {
                get
                {
                    return this.responseCodeField;
                }
                set
                {
                    this.responseCodeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string responseMessage
            {
                get
                {
                    return this.responseMessageField;
                }
                set
                {
                    this.responseMessageField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string uppTransactionId
            {
                get
                {
                    return this.uppTransactionIdField;
                }
                set
                {
                    this.uppTransactionIdField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string authorizationCode
            {
                get
                {
                    return this.authorizationCodeField;
                }
                set
                {
                    this.authorizationCodeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string acqAuthorizationCode
            {
                get
                {
                    return this.acqAuthorizationCodeField;
                }
                set
                {
                    this.acqAuthorizationCodeField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class paymentServiceBodyTransactionError
        {

            private string errorCodeField;

            private string errorMessageField;

            private string errorDetailField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string errorCode
            {
                get
                {
                    return this.errorCodeField;
                }
                set
                {
                    this.errorCodeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string errorMessage
            {
                get
                {
                    return this.errorMessageField;
                }
                set
                {
                    this.errorMessageField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public string errorDetail
            {
                get
                {
                    return this.errorDetailField;
                }
                set
                {
                    this.errorDetailField = value;
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class NewDataSet
        {

            private paymentService[] itemsField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("paymentService")]
            public paymentService[] Items
            {
                get
                {
                    return this.itemsField;
                }
                set
                {
                    this.itemsField = value;
                }
            }
        }
    }
}