Feature: Login Functionality
    To verify that the login feature og Warehouse Management System work correctly

    Background:
        Given verify that login page is visible successfully

    @Login @UI @Positive @Regression
    Scenario: User logs in successfully with valid credentials
        When user enters valid username
        And user enters valid password
        And user clicks the login button
        Then user should be redirected to the dashboard page successfully

    @Login @UI @Negative @Regression
    Scenario: User fails to login with invalid username
        When user enters invalid username "wrongUser"
        And user enters valid password
        And user clicks the login button
        Then user should see an error message

    @Login @UI @Negative @Regression
    Scenario: User fails to login with invalid password
        When user enters valid username
        And user enters invalid password "wrongPassword"
        And user clicks the login button
        Then user should see an error message

    @Login @UI @Negative @Regression
    Scenario: User fails to login with empty username and password
        When user leaves username and password fields empty
        And user clicks the login button
        Then user should see a validation message
