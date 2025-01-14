*** Settings ***
Resource    shared_variables.robot
Library     SeleniumLibrary
Library     EyesLibrary     runner=${RUNNER}      config=../applitools.yaml

*** Test Cases ***
Check Window Suite 1
    Open Browser                              ${URL}      ${BROWSER}        options=add_argument("--headless")
    Eyes Open
    Eyes Check Window
    Eyes Close Async
    Close All Browsers

