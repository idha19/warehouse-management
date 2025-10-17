Feature: Basket Management
    To verify that user can navigate and create a new basket successfully in Warehouse Management System

    Background:
        Given user is logged in and on Basket Management page

    @Basket @UI @Regression
    Scenario Outline: User navigates to Basket Management and creates a new basket successfully
        When user clicks on Add Basket button
        Then verify that Add Basket page is visible

        When user selects category <Category> and staging <Staging>
        And user clicks on Create Barcode button
        Then verify that Basket ID displayed

        When user clicks on Save button
        Then verify basket is saved successfully

        Examples:
        | Category | Staging     |
        | Basket   | QI to BIN   |
        | Basket   | BIN to PACK |
        | Basket   | GR to QI    |