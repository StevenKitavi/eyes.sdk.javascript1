# Changelog

## [5.67-beta04](https://github.com/applitools/eyes.sdk.javascript1/compare/dotnet/appium2@5.66-beta04...dotnet/appium2@5.67-beta04) (2023-08-11)

### Dependencies

* Eyes.Selenium4 bumped to 3.69

* Eyes.Images bumped to 3.40

* Eyes.Image.Core bumped to 3.9.0

* js/core bumped to 3.9.0

  #### Features

  * re-release ([e62abc7](https://github.com/applitools/eyes.sdk.javascript1/commit/e62abc7e74ea0e193eb7770036ae7f97bd11188a))

  #### Bug Fixes

  * propagate stitch mode to applitools lib ([a2dcedb](https://github.com/applitools/eyes.sdk.javascript1/commit/a2dcedb4bc6b999c137ed2aab43e0a463aa90169))

* @applitools/nml-client bumped to 1.5.7
  #### Bug Fixes

  * propagate stitch mode to applitools lib ([a2dcedb](https://github.com/applitools/eyes.sdk.javascript1/commit/a2dcedb4bc6b999c137ed2aab43e0a463aa90169))




## [5.66-beta04](https://github.com/applitools/eyes.sdk.javascript1/compare/dotnet/appium2@5.65-beta04...dotnet/appium2@5.66-beta04) (2023-07-31)

### Bug Fixes

* fixed `SetMobileCapabilities`

### Dependencies

* Eyes.Selenium4 bumped to 3.68

* Eyes.Images bumped to 3.39

  #### Features

  * added `Resize` stitch mode

* Eyes.Image.Core bumped to 3.6.6

* js/core bumped to 3.6.6

  #### Bug Fixes

  * populate log event settings with env vars ([#1840](https://github.com/applitools/eyes.sdk.javascript1/issues/1840)) ([0a6af60](https://github.com/applitools/eyes.sdk.javascript1/commit/0a6af60b5b988f59b7adb03f6606b3417fbeb537))

* @applitools/core-base bumped to 1.5.0
  #### Features

  * add stuck request retries to all requests to UFG and Eyes ([#1826](https://github.com/applitools/eyes.sdk.javascript1/issues/1826)) ([5884d42](https://github.com/applitools/eyes.sdk.javascript1/commit/5884d428b230e3a832a2110a388ebe63a94006fc))
  * mark session as component ([#1841](https://github.com/applitools/eyes.sdk.javascript1/issues/1841)) ([c579bb6](https://github.com/applitools/eyes.sdk.javascript1/commit/c579bb69de8f3bffc64e73ac8bd4fa646e96eb01))

  #### Bug Fixes

  * populate log event settings with env vars ([#1840](https://github.com/applitools/eyes.sdk.javascript1/issues/1840)) ([0a6af60](https://github.com/applitools/eyes.sdk.javascript1/commit/0a6af60b5b988f59b7adb03f6606b3417fbeb537))
* @applitools/driver bumped to 1.13.4
  #### Bug Fixes

  * extract device orientation from a browser for web executions ([d8d4e91](https://github.com/applitools/eyes.sdk.javascript1/commit/d8d4e919965fb9105915e762c397ec2cc57a8a71))

* @applitools/snippets bumped to 2.4.22
  #### Bug Fixes

  * improve orientation extraction for ios devices ([378d989](https://github.com/applitools/eyes.sdk.javascript1/commit/378d9894e4fbc7247087ccb8c46266dc4737e2e5))
* @applitools/ufg-client bumped to 1.6.0
  #### Features

  * add stuck request retries to all requests to UFG and Eyes ([#1826](https://github.com/applitools/eyes.sdk.javascript1/issues/1826)) ([5884d42](https://github.com/applitools/eyes.sdk.javascript1/commit/5884d428b230e3a832a2110a388ebe63a94006fc))

  #### Bug Fixes

  * improve fetch error experience when fetching from tunnel ([e7d8b49](https://github.com/applitools/eyes.sdk.javascript1/commit/e7d8b49947c07beb27f110cb4952e8c3206469af))
* @applitools/ec-client bumped to 1.7.4

* @applitools/spec-driver-webdriver bumped to 1.0.41

* @applitools/nml-client bumped to 1.5.6

* @applitools/spec-driver-webdriverio bumped to 1.5.10

* @applitools/screenshoter bumped to 3.8.7

* @applitools/spec-driver-puppeteer bumped to 1.1.72

* @applitools/spec-driver-selenium bumped to 1.5.55

## [5.65-beta04](https://github.com/applitools/eyes.sdk.javascript1/compare/dotnet/appium2@5.64-beta04...dotnet/appium2@5.65-beta04) (2023-07-31)

### Dependencies

* dotnet/Eyes.Selenium4 bumped to 3.67 

* dotnet/Eyes.Images bumped to 3.38
  ### Bug Fixes
  
  * debug screenshots support

## [5.64-beta04](https://github.com/applitools/eyes.sdk.javascript1/compare/dotnet/appium2@5.63-beta04...dotnet/appium2@5.64-beta04) (2023-07-30)

### Dependencies

* dotnet/Eyes.Selenium4 bumped to 3.66 

* dotnet/Eyes.Images bumped to 3.37

* dotnet/Eyes.Image.Core bumped to 3.6.5

* @applitools/core bumped to 3.6.5
  #### Bug Fixes

  * rendering issue with chrome &gt;113 and css white-space property ([cf34ad1](https://github.com/applitools/eyes.sdk.javascript1/commit/cf34ad1a5b3cba0b29b3509616b20a2b1313c62f))

* @applitools/ufg-client bumped to 1.5.3
  #### Bug Fixes

  * consider response headers and status code which are returned from the EC resource handler ([#1823](https://github.com/applitools/eyes.sdk.javascript1/issues/1823)) ([b7bd541](https://github.com/applitools/eyes.sdk.javascript1/commit/b7bd5415ae8f92a8032fc68ba993ccac1d9ff76a))
* com.applitools:eyes-universal-core bumped to 5.63.4

## [5.63-beta04](https://github.com/applitools/eyes.sdk.javascript1/releases/tag/dotnet/appium2@5.63-beta04) (2023-07-27)

### Dependencies

* dotnet/Eyes.Selenium4 bumped to 3.65 

* dotnet/Eyes.Images bumped to 3.36

* dotnet/Eyes.Image.Core bumped to 3.6.4

* @applitools/core bumped to 3.6.4
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))

* @applitools/core-base bumped to 1.4.3
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))

* @applitools/image bumped to 1.1.2
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))
* @applitools/logger bumped to 2.0.7
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))
* @applitools/req bumped to 1.5.2
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))
* @applitools/driver bumped to 1.13.3
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))

* @applitools/ec-client bumped to 1.7.3
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))

* @applitools/socket bumped to 1.1.7
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))

* @applitools/spec-driver-webdriver bumped to 1.0.40
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))

* @applitools/tunnel-client bumped to 1.1.3
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))

* @applitools/nml-client bumped to 1.5.5
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))

* @applitools/spec-driver-webdriverio bumped to 1.5.9
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))

* @applitools/screenshoter bumped to 3.8.6
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))

* @applitools/ufg-client bumped to 1.5.2
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))

* @applitools/spec-driver-puppeteer bumped to 1.1.71
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))

* @applitools/spec-driver-selenium bumped to 1.5.54
  #### Bug Fixes

  * fix workspace dependencies ([2a3856f](https://github.com/applitools/eyes.sdk.javascript1/commit/2a3856f3ce3bcf1407f59c676653b6f218556760))


## [5.62-beta04] (NEW)
### Added
- DotNet Appium2 support.
- Server Core version 3.6.2.