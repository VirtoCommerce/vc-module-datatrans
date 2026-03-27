# Datatrans Payment Gateway Module

Datatrans payment module provides integration with [Datatrans](https://www.datatrans.ch/en) payment gateway for Virto Commerce. It supports two payment modes:

- **Secure Fields** - PCI-compliant inline card form rendered directly on the checkout page via Datatrans-hosted iframes
- **Lightbox** - A popup overlay provided by Datatrans where the customer selects a payment method and completes payment

Both modes support 3-D Secure authentication, authorize/capture, void, and refund operations.

## Features

- Two payment modes: Secure Fields (inline) and Lightbox (popup)
- Configurable payment method filtering for Lightbox mode
- Auto-settlement option for Lightbox transactions
- Full payment lifecycle: authorize, capture, void, refund
- Sandbox and production environments
- 3-D Secure support

## Installation

Install the module in one of two ways:

- **Automatically**: In Virto Commerce Admin, go to Configuration > Modules > Datatrans Payment Gateway > Install
- **Manually**: Download the module zip package from [GitHub Releases](https://github.com/VirtoCommerce/vc-module-datatrans/releases). In Virto Commerce Admin, go to Configuration > Modules > Advanced > Upload module package > Install

## Configuration

### Application Settings (appsettings.json)

API credentials are configured in the application settings under the `Payments:Datatrans` section:

```json
{
  "Payments": {
    "Datatrans": {
      "MerchantId": "<your-merchant-id>",
      "Secret": "<your-api-secret>",
      "UseSandbox": true
    }
  }
}
```

| Property | Description | Default |
|----------|-------------|---------|
| `MerchantId` | Datatrans Merchant ID | - |
| `Secret` | Datatrans API secret (used for Basic Auth) | - |
| `UseSandbox` | Use sandbox environment for testing | `true` |

Merchant ID and API secret can be found in the [Datatrans Web Admin](https://admin.sandbox.datatrans.com/).

### Store Settings (Virto Commerce Admin)

These settings are configured per store in the Virto Commerce Admin under Store > Payment methods > Datatrans:

| Setting | Description | Default |
|---------|-------------|---------|
| **Sandbox** | Toggle between sandbox (test) and production mode | `true` |
| **Return URL** | URL to redirect after payment completion. Use `{orderId}` placeholder for the order ID | `/account/orders/{orderId}/payment` |
| **Payment mode** | Select the payment mode: `SecureFields` (inline card form) or `Lightbox` (popup overlay) | `SecureFields` |
| **Lightbox payment methods** | Comma-separated list of payment method codes to display in Lightbox (e.g., `VIS,ECA,PAP,TWI`). Leave empty to show all available methods | *(empty)* |
| **Lightbox auto-settle** | Automatically settle (capture) transactions after authorization when using Lightbox mode | `false` |

### Common Datatrans Payment Method Codes

| Code | Payment Method |
|------|---------------|
| `VIS` | Visa |
| `ECA` | Mastercard |
| `AMX` | American Express |
| `PAP` | PayPal |
| `TWI` | Twint |
| `PFC` | PostFinance Card |
| `PEF` | PostFinance E-Finance |
| `APL` | Apple Pay |
| `GOO` | Google Pay |

For the full list, see the [Datatrans documentation](https://docs.datatrans.ch/docs/supported-payment-methods).

## Payment Modes

### Secure Fields

In Secure Fields mode, the card number and CVV fields are rendered as Datatrans-hosted iframes directly on your checkout page. The customer fills in their card details without leaving the page.

- Uses `POST /v1/transactions/secureFields` to initialize
- Loads `secure-fields-2.0.0.min.js` on the frontend
- Supports 3-D Secure with automatic redirect

### Lightbox

In Lightbox mode, the customer clicks a "Pay" button and a Datatrans popup overlay opens. The customer selects their preferred payment method and completes the payment within the popup.

- Uses `POST /v1/transactions` to initialize
- Loads `datatrans-2.0.0.js` on the frontend
- Supports filtering which payment methods are shown
- Supports auto-settlement
- 3-D Secure is handled within the Lightbox

For more details, see the [Datatrans Lightbox documentation](https://docs.datatrans.ch/docs/redirect-lightbox).

## Frontend Integration

The module includes Vue.js components for the Virto Commerce Frontend application:

- `payment-processing-datatrans.vue` - Wrapper component that detects the configured mode and renders the appropriate UI
- `payment-processing-datatrans-secure-fields.vue` - Secure Fields inline card form
- `payment-processing-datatrans-lightbox.vue` - Lightbox popup trigger with centered card layout

The frontend automatically detects the payment mode from the backend configuration via the `paymentMode` public parameter returned during payment initialization.

## References
* [Datatrans Introduction](https://docs.datatrans.ch/docs/introduction)
* [Lightbox mode](https://docs.datatrans.ch/docs/redirect-lightbox)
* [Secure Fields mode](https://docs.datatrans.ch/docs/redirect-lightbox)

## License

Copyright (c) Virtosoftware Ltd. All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
