 
 
# Currency Exchange Application

A C# Windows Forms application that retrieves daily USD and EUR exchange rates from the Central Bank of the Republic of Türkiye (TCMB), performs currency calculations, and tracks currency transactions using SQL Server.

## Features

- **Live Exchange Rates:** Retrieves USD and EUR banknote buying and selling rates from the TCMB XML feed.
- **Currency Calculations:** Calculates exchange amounts based on the selected rate.
- **Transaction Tracking:** Records USD and EUR purchases and corresponding Turkish Lira transactions in SQL Server.
- **Balance Tracking:** Displays calculated balances for USD, EUR, and TRY.
- **Database Transactions:** Uses SQL transactions to keep related currency transaction records consistent.

## Screenshots

![Currency Exchange Application](images/doviz-uygulamasi.png.PNG)


## Technologies Used

- C#
- .NET Framework
- Windows Forms
- Microsoft SQL Server
- ADO.NET
- XML

## Data Source

Exchange rates are retrieved from the official TCMB daily exchange rate feed:

https://www.tcmb.gov.tr/kurlar/today.xml

## Database Setup

The application uses SQL Server to store currency transactions.

The database and table must be created before running the application. The transaction table is named `Tbl_DovizIslemleri`.

The application currently uses a local SQL Server connection. Update the connection string in the source code to match your local SQL Server instance and database.

## Getting Started

1. Clone or download this repository.
2. Open the solution file (`.sln`) in Visual Studio.
3. Configure the SQL Server connection string.
4. Create the required database and transaction table.
5. Build and run the Windows Forms application.

## What I Learned

- Retrieving XML data from an external service.
- Working with SQL Server using ADO.NET.
- Executing parameterized SQL commands.
- Using SQL transactions to manage related database operations.
- Performing currency calculations and displaying results in a Windows Forms interface.

## Future Improvements

- Add input validation and improved error handling.
- Move the database connection string into configuration settings.
- Improve the user interface and transaction history.
- Separate database operations from the Windows Forms UI.

---

**Project Type:** Desktop Application  
**Status:** Learning Project
