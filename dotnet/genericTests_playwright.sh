#!/usr/bin/env bash

RESULT=0

echo "installing powershell"
wget --no-verbose https://github.com/PowerShell/PowerShell/releases/download/v7.3.4/powershell_7.3.4-1.deb_amd64.deb
sudo dpkg -i powershell_7.3.4-1.deb_amd64.deb
sudo apt-get install -f

echo "generating tests"
pushd coverage-tests
# export UFG_ON_EG=true
npm run generate:playwright
if [ $? -ne 0 ]; then
    RESULT=1
    echo "npm run dotnet:generate:playwright have failed"
    exit 1
fi

# start eg client and save process id
# commented out if need eg client logs
export APPLITOOLS_SHOW_LOGS=true
yarn universal:eg &
EG_PID="$!"
export EXECUTION_GRID_URL=http://localhost:8080
echo $EXECUTION_GRID_URL
echo "building playwright tests"

dotnet build ./test/Playwright
pwsh ./test/Playwright/bin/Debug/net6.0/playwright.ps1 install

echo "running tests playwright"
npm run run:parallel:playwright
result=$?
echo $result
if [ $result -ne 0 ]; then
    echo "Not all tests passed... Retrying."
    npm run run:parallel:playwright
	if [ $? -ne 0 ]; then
      RESULT=1
      echo "npm run dotnet:run:parallel:playwright have failed"
    fi
fi

# Kill eg client by the process id
echo $EG_PID
kill $EG_PID

echo "RESULT = ${RESULT}"
if [ $RESULT -eq 0 ]; then
    exit 0
else
    exit 1
fi