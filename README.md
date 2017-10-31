## Synopsis

This is a project demo for the AltSource Inc. This project acts as a banking ledger for a user.  
The user can login, logout, deposit, withdraw, get balance, and list transactions.
The project contains 3 main components:

### Web API - AltSourceBankAppAPI
A C# .Net Core Web API. This project is the core functionality - it creates an in-memory data store using EntityFramework.
It also implements Identity authentication.  This API exposes endpoints for applications to access via requests which will 
interact with the models created.  Provided a custom API Token system and endpoints are Filtered and checked against that from client.

### Web Application - AltSourceWebBanking
A C# ASP.NET MVC Web Application.  This project provides a frontend that interacts with the Web API.
It utilizes C#, JavaScript, jQuery, jQuery DataTables, Bootstrap, SASS, gulp task runner for compiling SASS,
Razor templating, cookies

### Console Application - AltSourceConsoleApp
A C# Console Application that utilizes reflection to generate commands out of the Controller classes from their Assembly.
These controller classes, when passed arguments from the command line then send out a request to the Web API
and performs the requested action.

#### Console Application Example Usage
Users.Create FirstName LastName some@email.com UserName Password     --- Password requirements (minlength: 8, 1 upper, 1 non-alpha, 1 number)

Users.Login UserName Email Password

Users.Deposit 123.45

Users.Withdraw 156.65

Users.Withdraw 114.23

Users.GetBalance

Users.Transactions

Users.Logout