module.exports = {
  name: "eyes_playwright_dotnet",
  emitter: "/home/itaibh/devel/sdk.coverage.tests/DotNet/playwright/emitter.js",
  overrides: [
    'https://raw.githubusercontent.com/applitools/sdk.coverage.tests/universal-sdk/js/overrides.js',
    "/home/itaibh/devel/sdk.coverage.tests/eg.overrides.js",
    "/home/itaibh/devel/sdk.coverage.tests/DotNet/overrides/overrides-playwright.js"
  ],
  template: "/home/itaibh/devel/sdk.coverage.tests/DotNet/playwright/template.hbs",
  tests: "/home/itaibh/devel/sdk.coverage.tests/coverage-tests.js",
  ext: ".cs",
  outPath: './test/Playwright/coverage/generic',
  emitOnly: test => {
    return !test.name.startsWith("appium") &&
           !test.name.includes('on android') &&
           !test.name.includes('on ios') &&
          (!test.features || (!test.features.includes('image') && 
                              !test.features.includes('native-selectors') &&
                              !test.features.includes('sauce')))
  },
  // emitOnly: ["check region by selector within shadow dom with vg"]
};