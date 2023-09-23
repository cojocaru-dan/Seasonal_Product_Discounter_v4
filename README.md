# Seasonal-Product-Discounter-4-1q2023

### In the fourth and final task of the Seasonal Product Discounter, we are going to simulate the process of buyers coming in, browsing products, and making purchases. But before that, we need to swap all the in-memory databases to tables in an SQLite database, in order to be able to persist the application's state between execution sessions.

### In this final version of the project, the application will do the following:
### 1.	Create the required tables in the SQLite database.
### 2.	Generate randomized products and store them in a table in the SQLite database.
### 3.	Register & authenticate users (buyers of products).
### 4.	Simulate transactions happening in the store.
### 5.	At the end of a simulation round a report about the status of the store is created & displayed.

### To follow the execution flow of the application we are going to create a logging framework. Logging will help us to make sure that the application performs exactly what we want, in the order we want it.

### We will use the 'DB Browser for SQLite' application throughout the project to inspect & manage our database file.

### What are we going to learn?
### •	Work with a multi-table relational database
### •	Manage an SQLite database
### •	Create a logging framework with multiple logging options (console, file)
### •	Apply all SOLID principles in a project
### •	The Repository Pattern
