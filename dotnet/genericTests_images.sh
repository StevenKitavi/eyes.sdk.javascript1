#!/usr/bin/env bash

RESULT=0

echo "generating tests - images"
pushd coverage-tests
# export UFG_ON_EG=true
npm run generate:images
if [ $? -ne 0 ]; then
    RESULT=1
    echo "npm run dotnet:generate have failed"
    exit 1
fi

# start eg client and save process id
# commented out if need eg client logs
export APPLITOOLS_SHOW_LOGS=true
yarn universal:eg &
EG_PID="$!"
export EXECUTION_GRID_URL=http://localhost:8080
echo $EXECUTION_GRID_URL

echo "running tests - images"
npm run run:parallel:images
result=$?
echo $result
if [ $result -ne 0 ]; then
    echo "Not all tests passed... Retrying."
    npm run run:parallel:images
	if [ $? -ne 0 ]; then
      RESULT=1
      echo "npm run dotnet:run:parallel:images have failed"
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