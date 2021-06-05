## General info
<p>Excel type project that allows many user to work on the same sheet.</p>

## Table of contents
* [General info](#general-info)
* [Technologies](#technologies)
* [Screenshots](#Screenshots)
* [Setup](#Setup)
* [Project Status](#Project-Status)
* [Room for Improvement](#Room-for-Improvement)

## Project features
* register/login for users
* creating new sheets
* option to be added to other sheets
* sheet are pernament and keept in dbs until user with permision to sheet decide to delete sheet
* spreadsheet can interprete matemathical expresion including operators like( +, -, *, /, '(', ')' ) through to Reverse Polish notation alghorithm 
* cells in spreadsheet can refer to ohter cells
*  unlogged user can check other sheets without permision to eddit them<

## Technologies
* C#
* ASP.NET Core 5.0
* Entity Framework core
* ASP.NET Identity
* HTML&CSS
* Bootstrap

## Screenshots
view for all sheets that cal logged or unlogged user see

![](https://github.com/JakubBehring/Spreadsheet/blob/main/Spreadsheet/wwwroot/images/AllSheets.png)

view for logged user for all his sheets

![](https://github.com/JakubBehring/Spreadsheet/blob/main/Spreadsheet/wwwroot/images/UserSheets.png)

view for logged user for his sheet

![](https://github.com/JakubBehring/Spreadsheet/blob/main/Spreadsheet/wwwroot/images/Expresions.png)

## Setup
To run project
* provide connection string and set it in apppsettings.json file
* call update-database command in package manager console

## Project Status
no longer working on it due to swap to other projects

## Room for Improvement
* better ux
* add signalR to let users see changes on the same sheetr without refreshing 



