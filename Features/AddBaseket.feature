Feature: Basket Management
    To verify that user can navigate and create a new basket successfully in Warehouse Management System

    @Basket @UI @Regression
    Scenario: User navigates to Basket Management and creates a new basket successfully
        Given verify that login page is visible successfully
        When user enters valid username and valid password
        And user clicks the login button
        Then user should be redirected to the dashboard page successfully
        
        When user clicks on "BASKET MANAGEMENT" section
        Then user should be redirected to Basket Management page successfully
        When user clicks on "Add Basket" button
        Then verify that Add Basket page is visible

        When user selects category "Basket" and staging "QI to BIN"
        And user clicks on "Create Barcode" button
        Then verify that Basket ID and Barcode are displayed

        When user clicks on "Save" button
        Then verify basket is saved successfully

        And user clicks on last page of basket list
        Then verify that newly created basket appears in the list