# Datatrans Lightbox Integration - QA Test Plan

## Overview

This test plan covers the Datatrans payment module with two payment modes: **Secure Fields** (existing, regression) and **Lightbox** (new). Testing should verify both modes work correctly end-to-end, including all backend settings, frontend rendering, payment flows, and edge cases.

## Environment Setup

### Prerequisites

- Virto Commerce Platform with Datatrans module deployed
- Virto Commerce Frontend application running
- Datatrans sandbox account with valid MerchantId and Secret
- `appsettings.json` configured:
  ```json
  {
    "Payments": {
      "Datatrans": {
        "MerchantId": "<sandbox-merchant-id>",
        "Secret": "<sandbox-secret>",
        "UseSandbox": true
      }
    }
  }
  ```

### Datatrans Sandbox Test Cards

| Card Type | Number | Expiry | CVV | 3-D Secure |
|-----------|--------|--------|-----|------------|
| Visa | 4242 4242 4242 4242 | Any future | Any | No |
| Visa (3DS) | 4000 0000 0000 0002 | Any future | Any | Yes |
| Mastercard | 5555 5555 5555 4444 | Any future | Any | No |

> Refer to [Datatrans test data](https://docs.datatrans.ch/docs/testing-credentials) for the full list.

---

## 1. Backend Settings Verification

### 1.1 Settings Visibility

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 1.1.1 | All settings appear in Admin | Go to Store > Payment methods > Datatrans > Settings | All 5 settings visible: Sandbox, Return URL, Payment mode, Lightbox payment methods, Lightbox auto-settle |
| 1.1.2 | Payment mode dropdown values | Click the Payment mode dropdown | Shows two options: "SecureFields" and "Lightbox" |
| 1.1.3 | Payment mode default value | Open settings on a fresh store | Default is "SecureFields" |
| 1.1.4 | Lightbox auto-settle default | Open settings on a fresh store | Default is unchecked (false) |
| 1.1.5 | Lightbox payment methods default | Open settings on a fresh store | Default is empty |

### 1.2 Settings Localization

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 1.2.1 | English labels | Switch Admin to English | All 5 settings show English title and description |
| 1.2.2 | German labels | Switch Admin to German | PaymentMode shows "Zahlungsmodus", descriptions in German |
| 1.2.3 | Other languages | Switch to each supported language (de, es, fi, fr, it, ja, no, pl, pt, ru, sv, zh) | All settings have localized title and description |

---

## 2. Secure Fields Mode (Regression)

> **Precondition**: Payment mode set to "SecureFields" in store settings.

### 2.1 Checkout Flow

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 2.1.1 | Card form renders | Add item to cart > Proceed to checkout > Reach payment step | Inline card form displays: Card number, Cardholder name, Expiration date, CVV fields |
| 2.1.2 | Successful payment | Fill valid card details > Click "Pay now" | Payment succeeds, redirected to success page |
| 2.1.3 | Invalid card number | Enter invalid card number | Real-time validation error on card number field |
| 2.1.4 | Missing cardholder name | Leave cardholder name empty > Try to submit | Validation error, button remains disabled |
| 2.1.5 | Expired card date | Enter past expiration date | Validation error on expiration field |
| 2.1.6 | Invalid CVV | Enter invalid CVV | Validation error on CVV field |
| 2.1.7 | Payment button disabled | Open form without filling any fields | "Pay now" button is disabled |

### 2.2 3-D Secure Flow

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 2.2.1 | 3-D Secure redirect | Use a 3DS test card > Submit payment | Browser redirects to 3-D Secure challenge page |
| 2.2.2 | 3-D Secure success | Complete 3-D Secure challenge successfully | Redirected back to payment page, payment finalized, success shown |
| 2.2.3 | 3-D Secure failure | Fail the 3-D Secure challenge | Redirected back with error, payment failed message shown |

### 2.3 Order Payment Page

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 2.3.1 | Pay from order page | Go to Account > Orders > Select unpaid order > Pay | Card form renders correctly on order payment page |
| 2.3.2 | Successful payment from order | Fill valid card > Pay | Payment succeeds, order status updates |

---

## 3. Lightbox Mode

> **Precondition**: Payment mode set to "Lightbox" in store settings.

### 3.1 UI Rendering

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 3.1.1 | Lightbox form renders | Add item to cart > Proceed to checkout > Reach payment step | Centered card layout: lock icon, "Secure Payment" title, description text, "Pay now" button with order total, payment policies below |
| 3.1.2 | No card fields shown | Observe the payment form | No card number, CVV, expiration, or cardholder fields visible |
| 3.1.3 | Order total on button | Check the Pay button text | Button shows "Pay now" followed by the formatted order total (e.g., "Pay now · $99.00") |
| 3.1.4 | Button loading state | Observe button while Lightbox script loads | Button shows loading spinner, becomes clickable after script loads |
| 3.1.5 | Lock icon visible | Observe the form header | Lock icon in a colored circular badge is displayed above the title |

### 3.2 Lightbox Payment Flow

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 3.2.1 | Lightbox opens | Click "Pay now" button | Datatrans Lightbox popup/overlay opens on top of the page |
| 3.2.2 | Payment methods displayed | Observe the Lightbox popup | Available payment methods are shown for selection |
| 3.2.3 | Successful card payment | Select Visa > Enter valid card > Submit | Lightbox closes, browser redirects to return URL, payment authorized, success page shown |
| 3.2.4 | User cancels Lightbox | Close the Lightbox popup (X button or click outside) | Lightbox closes, user stays on payment page, "Pay now" button is clickable again (no spinner) |
| 3.2.5 | Retry after cancel | Cancel Lightbox > Click "Pay now" again | Lightbox opens again, payment can be retried |
| 3.2.6 | Payment error handling | Trigger a payment error (e.g., declined card) | Error notification displayed, user can retry |

### 3.3 Lightbox Redirect Return

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 3.3.1 | Success redirect | Complete payment in Lightbox | Browser redirects to return URL with `datatransTrxId` parameter |
| 3.3.2 | Payment finalization | After redirect | `authorizePayment` GraphQL mutation called, payment status updated to Authorized or Paid |
| 3.3.3 | Order status update | After successful payment | Order status changes to "Processing" |
| 3.3.4 | Cancel redirect | Cancel payment in Lightbox (if redirected) | Return URL handles cancel gracefully, error shown |
| 3.3.5 | Error redirect | Trigger error in Lightbox (if redirected) | Return URL handles error gracefully, error message shown |

### 3.4 Order Payment Page (Lightbox)

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 3.4.1 | Lightbox on order page | Go to Account > Orders > Select unpaid order > Pay | Lightbox UI renders (lock icon, description, Pay button with amount) |
| 3.4.2 | Payment from order page | Click "Pay now" > Complete in Lightbox | Payment succeeds, order payment page shows success |
| 3.4.3 | Cancel from order page | Click "Pay now" > Close Lightbox | Returns to order payment page, can retry |

---

## 4. Lightbox Configuration Options

### 4.1 Payment Methods Filter

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 4.1.1 | Empty filter (default) | Leave "Lightbox payment methods" empty > Open Lightbox | All available payment methods are shown |
| 4.1.2 | Single method | Set "Lightbox payment methods" to `VIS` | Only Visa appears in the Lightbox |
| 4.1.3 | Multiple methods | Set to `VIS,ECA` | Only Visa and Mastercard appear |
| 4.1.4 | Multiple methods with spaces | Set to `VIS, ECA, PAP` | Visa, Mastercard, and PayPal appear (spaces trimmed) |
| 4.1.5 | Invalid method code | Set to `VIS,INVALID` | Visa appears, invalid code is ignored by Datatrans |

### 4.2 Auto-Settle

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 4.2.1 | Auto-settle disabled | Uncheck "Lightbox auto-settle" > Complete payment | Transaction status is "authorized" (requires manual capture) |
| 4.2.2 | Auto-settle enabled | Check "Lightbox auto-settle" > Complete payment | Transaction status is "settled" (automatically captured) |
| 4.2.3 | Verify in Datatrans dashboard | After auto-settle payment | Transaction shows as settled in Datatrans admin |

---

## 5. Mode Switching

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 5.1 | Switch SecureFields to Lightbox | Change Payment mode from SecureFields to Lightbox > Save > Checkout | Lightbox UI renders (no card fields) |
| 5.2 | Switch Lightbox to SecureFields | Change Payment mode from Lightbox to SecureFields > Save > Checkout | Secure Fields card form renders |
| 5.3 | Default mode on new store | Create a new store, enable Datatrans | SecureFields mode is active by default |

---

## 6. Capture, Void, and Refund (Both Modes)

> These operations are performed from the Virto Commerce Admin.

### 6.1 Capture

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 6.1.1 | Full capture | Complete payment (auto-settle OFF) > Admin > Capture full amount | Payment status changes to Paid, capture recorded |
| 6.1.2 | Partial capture | Authorize > Capture partial amount | Partial capture recorded, remaining amount available |

### 6.2 Void

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 6.2.1 | Void pending payment | Authorize > Void before capture | Payment voided successfully |
| 6.2.2 | Void captured payment | Capture > Attempt void | Error: "Cannot void a captured payment. Use refund instead." |

### 6.3 Refund

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 6.3.1 | Full refund | Capture > Refund full amount | Refund processed, payment status updated |
| 6.3.2 | Partial refund | Capture > Refund partial amount | Partial refund processed |
| 6.3.3 | Refund without capture | Authorize (no capture) > Attempt refund | Error: "No captured amount to refund." |

---

## 7. Error Handling

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 7.1 | Invalid Merchant ID | Configure wrong MerchantId in appsettings > Attempt payment | Payment initialization fails, error message displayed |
| 7.2 | Network timeout | Simulate slow/no connection to Datatrans API | Appropriate error message shown, no spinner stuck |
| 7.3 | Lightbox script load failure | Block Datatrans script URL > Attempt Lightbox payment | Error notification, button not stuck in loading |
| 7.4 | Double-click prevention | Click "Pay now" rapidly multiple times | Only one Lightbox opens, button shows loading state |
| 7.5 | Browser back during Lightbox | Open Lightbox > Press browser back | Graceful handling, no broken state |
| 7.6 | Page refresh during payment | Start payment > Refresh page | Payment can be restarted cleanly |

---

## 8. Frontend Localization

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 8.1 | English Lightbox UI | Set storefront language to English | Title: "Secure Payment", description and button in English |
| 8.2 | German Lightbox UI | Set storefront language to German | Title: "Sichere Zahlung", description in German |
| 8.3 | All languages | Test each supported locale (de, es, fi, fr, it, ja, no, pl, pt, ru, sv, zh) | Lightbox title and description are properly translated |
| 8.4 | Fallback to English | Use unsupported locale | Falls back to English strings |

---

## 9. Cross-Browser Testing

| # | Browser | Test Cases |
|---|---------|------------|
| 9.1 | Chrome (latest) | Lightbox opens, payment completes, redirect works |
| 9.2 | Firefox (latest) | Lightbox opens, payment completes, redirect works |
| 9.3 | Safari (latest) | Lightbox opens (popup blocker may interfere), redirect works |
| 9.4 | Edge (latest) | Lightbox opens, payment completes, redirect works |
| 9.5 | Mobile Chrome (Android) | Lightbox renders correctly on mobile viewport |
| 9.6 | Mobile Safari (iOS) | Lightbox renders correctly on mobile viewport |

---

## 10. Security & Edge Cases

| # | Test Case | Steps | Expected Result |
|---|-----------|-------|-----------------|
| 10.1 | Tampered transactionId | Modify `datatransTrxId` URL parameter to invalid value | Payment authorization fails gracefully, error shown |
| 10.2 | Expired transaction | Wait for transaction to expire > Try to complete | Error message shown, user can restart payment |
| 10.3 | Concurrent sessions | Open two browser tabs, start payment in both | Each tab gets its own transaction, no conflicts |
| 10.4 | HTTPS enforcement | Attempt payment over HTTP | Datatrans scripts require HTTPS, verify proper handling |

---

## Test Execution Summary

| Section | Total Tests | Priority |
|---------|------------|----------|
| 1. Backend Settings | 8 | Medium |
| 2. Secure Fields (Regression) | 10 | High |
| 3. Lightbox Mode | 14 | Critical |
| 4. Lightbox Configuration | 7 | High |
| 5. Mode Switching | 3 | High |
| 6. Capture/Void/Refund | 6 | High |
| 7. Error Handling | 6 | Medium |
| 8. Frontend Localization | 4 | Low |
| 9. Cross-Browser | 6 | Medium |
| 10. Security & Edge Cases | 4 | Medium |
| **Total** | **68** | |

### Suggested Execution Order

1. **Smoke test**: 3.1.1, 3.2.1, 3.2.3 (verify Lightbox opens and payment works)
2. **Core Lightbox**: All Section 3 tests
3. **Configuration**: All Section 4 tests
4. **Regression**: All Section 2 tests (Secure Fields still works)
5. **Mode switching**: Section 5
6. **Operations**: Section 6
7. **Error handling**: Section 7
8. **Localization, browsers, security**: Sections 8-10
