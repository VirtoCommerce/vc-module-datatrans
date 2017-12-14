# Datatrans Checkout payment gateway module
Datatrans Checkout payment  module provides integration with <a href="https://www.datatrans.ch/en" target="_blank">Datatrans</a> Lightbox mode. 

# Installation
Installing the module:
* Automatically: in VC Manager go to Configuration -> Modules -> Datatrans Checkout payment gateway module -> Install
* Manually: download module zip package from https://github.com/VirtoCommerce/vc-module-datatrans/releases. In VC Manager go to Configuration -> Modules -> Advanced -> upload module package -> Install.

# Settings
* **Merchant Id** - ID assoiciated with this merchant. Can be fount in Datatrans merchant dashboard.
* **Sign** - An additional Merchant-IDentificatio. Can be fount in Datatrans UPP Administration.
* **Working mode** - Test or live working mode.
* **Payment action type** - Payment action type. Sale (immediate sale) or Settlement (Authorize/Capture)
* **Payment Method** - Codes for enabled payment methods. Examples VIS - Visa, ECA - MasterCard, AMX - American Express, DIN - Diners, MPW - Masterpass, UATP - UATP.
* **Language** - Purchase locale code used for creating payments.
* **Process Payment action** - Action URl that is going to be hit in case of successful or error authorization.

Transaction is going to use order's currency.


# License
Copyright (c) Virtosoftware Ltd.  All rights reserved.

Licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at

http://virtocommerce.com/opensourcelicense

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
