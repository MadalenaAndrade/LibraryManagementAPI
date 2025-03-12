# Library Management API (Ongoing Development)
This is an ongoing training project where I am building a RESTful API to manage a fictitious library database, using **ASP.NET Core 8** and **Entity Framework Core**. The goal is to practice API development and database management concepts, with incremental improvements and new features being added over time.
The database was created using the **Code First** approach, based on a similar [SQL repository](https://github.com/MadalenaAndrade/LibraryDB-SQL-Scripts/blob/main/FullLibraryQuery.sql) I have worked on. Additionally, among new concepts I am using this project to learn **Entity Framework Core**, an **Object-Relational Mapper (ORM)** for .NET, as I was previously unfamiliar with it.

This project supports both **local development** (running with SQL Server in your machine) and deployment to **Azure** (hosted API and database)

## 📌 Table of Contents
- [🛠 Technologies Used](#-technologies-used)
- [🚀 Current Progress](#-current-progress)
- [📂 Main Project Structure](#-main-project-structure)
- [📡 API Endpoints](#-api-endpoints)
- [🚀 Getting Started (Local and Azure)](#-getting-started)
- [⚠️ Note](#️-note)



## 🛠 Technologies Used
- **Visual Studio** - IDE for project development
- **ASP.NET Core 8** - For building the RESTful API
- **Entity Framework Core** - For database management
- **C#** - Main programming language
- **SQL Server** - For data storage
- **Swagger / Swashbuckle** - For API documentation
- **SQL Server Management Studio(SSMS)** - for database migration to Azure
- **Microsoft Azure** - Cloud hosting (App Service & SQL Database)

## 🚀 Current Progress
- **Code First approach** - Creating models and DbContext using **data annotations** and **Fluent API**
- **Database migration** configured
- **Basic CRUD operations** implemented 
- **DTOs** added for structured data transfer
- **Custom attributes** setup
- **Swagger UI** integrated for API documentation
- API deployed to **Microsoft Azure**

## 📂 Main Project Structure
📦 [**LibraryManagement**](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement) <br>
├─ 📚 [**EFCoreClasses**](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/EFCoreClasses) # Library class project <br>
│  ├─ 📂 [*Configurations*](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/EFCoreClasses) # Fluent API entity configurations <br>
│  │  ├─ 📄 [`BookAuthorConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/BookAuthorConfiguration.cs) <br>
│  │  ├─ 📄 [`BookCategoryConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/BookCategoryConfiguration.cs)   
│  │  ├─ 📄 [`BookConditionConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/BookConditionConfiguration.cs)  
│  │  ├─ 📄 [`BookConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/BookConfiguration.cs)     
│  │  ├─ 📄 [`BookCopyConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/BookCopyConfiguration.cs)  
│  │  ├─ 📄 [`BookStockConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/BookStockConfiguration.cs)    
│  │  ├─ 📄 [`ClientConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/ClientConfiguration.cs)   
│  │  ├─ 📄 [`RentConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/RentConfiguration.cs) <br> 
│  │  └─ 📄 [`RentReceptionConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/RentReceptionConfiguration.cs)           
│  ├─ 📂 [*Migrations*](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/EFCoreClasses/Migrations) # Database migration files <br>
│  ├─ 📂 [*Models*](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/EFCoreClasses/Models) # Database models <br>
│  │  ├─ 📄 [`Author.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/Author.cs)      
│  │  ├─ 📄 [`Book.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/Book.cs)    
│  │  ├─ 📄 [`BookAuthor.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/BookAuthor.cs)   
│  │  ├─ 📄 [`BookCategory.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/BookCategory.cs)      
│  │  ├─ 📄 [`BookCondition.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/BookCondition.cs)
│  │  ├─ 📄 [`BookCopy.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/BookCopy.cs)   
│  │  ├─ 📄 [`BookStock.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/BookStock.cs)     
│  │  ├─ 📄 [`Category.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/Category.cs)  
│  │  ├─ 📄 [`Client.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/Client.cs) <br> 
│  │  ├─ 📄 [`Publisher.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/Publisher.cs) <br> 
│  │  ├─ 📄 [`Rent.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/Rent.cs) <br> 
│  │  └─ 📄 [`RentReception.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/RentReception.cs)  
│  └─ 📄 [`LibraryDbContext.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/LibraryDbContext.cs) # Database context <br>
├─ 🌐 [**LibraryManagementAPI**](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/LibraryManagementAPI) # ASP.NET Core Web API project <br>
│  ├─ 📂 [*Controllers*](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/LibraryManagementAPI/Controllers) # API endpoints for CRUD operations <br>
│  │  ├─ 📄 [`AuthorController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/AuthorController.cs) <br>
│  │  ├─ 📄 [`BookController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/BookController.cs) <br>
│  │  ├─ 📄 [`BookCopyController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/BookCopyController.cs) <br>
│  │  ├─ 📄 [`CategoryController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/CategoryController.cs) <br>
│  │  ├─ 📄 [`ClientController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/ClientController.cs) <br>
│  │  ├─ 📄 [`PublisherController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/PublisherController.cs) <br>
│  │  └─ 📄 [`RentController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/RentController.cs) <br>
│  ├─ 📂 [*DTOs*](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/LibraryManagementAPI/DTOs) # Data Transfer Objects <br>
│  │  ├─ 📄 [`AuthorDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/AuthorDto.cs) <br>
│  │  ├─ 📄 [`BookDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/BookDto.cs) <br>
│  │  ├─ 📄 [`BookCopyDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/BookCopyDto.cs) <br>
│  │  ├─ 📄 [`CategoryDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/CategoryDto.cs) <br>
│  │  ├─ 📄 [`ClientDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/ClientDto.cs) <br>
│  │  ├─ 📄 [`PublisherDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/PublisherDto.cs) <br>
│  │  ├─ 📄 [`RentDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/RentDto.cs) <br>
│  │  └─ 📄 [`CustomAttributes.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/CustomAttributes.cs)            
│  ├─ 📂 [*Properties*](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/LibraryManagementAPI/Properties) # Launch settings <br>
│  ├─ 📄 [`Program.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Program.cs) # Entry point of the Web API <br>
│  ├─ 📝 [`appsetting.Development.json`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/appsettings.Development.json) # Development environment settings <br>
│  └─ 📝 [`appsetting.json`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/appsettings.json) # Main settings (connection strings) <br>
└─ 🧾 [`README.md`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/README.md)                             

## 📡 API Endpoints
### 📝 Author
| Method | Endpoint      | Description      |
|--------|---------------|------------------|
| POST   | /Author       | Add a new author |
| GET    | /Author/{id}  | Get author       |
| GET    | /Author/list  | Get all authors  |
| PUT    | /Author/{id}  | Update author    |
| DELETE | /Author/{id}  | Delete author    |

### 📚 Book
| Method | Endpoint              | Description             |
|--------|-----------------------|-------------------------|
| POST   | /Book                 | Add a new book          |
| GET    | /Book/filter          | Get books using filters |
| GET    | /Book/list            | Get all books           |
| PUT    | /Book/{serialNumber}  | Update book             |
| DELETE | /Book/{serialNumber}  | Delete book             |

### 📖 Book Copy
| Method | Endpoint          | Description                   |
|--------|-------------------|-------------------------------|
| POST   | /BookCopy         | Add a new book copy           |
| GET    | /BookCopy/filter  | Get book copies using filters |
| GET    | /BookCopy/list    | Get all book copies           |
| PUT    | /BookCopy/{id}    | Update book copy              |
| DELETE | /BookCopy/{id}    | Delete book copy              |

### 🏷️ Category
| Method | Endpoint        | Description        |
|--------|-----------------|--------------------|
| POST   | /Category       | Add a new category |
| GET    | /Category/{id}  | Get category       |
| GET    | /Category/list  | Get all categories |
| PUT    | /Category/{id}  | Update category    |
| DELETE | /Category/{id}  | Delete category    |

### 👤 Client
| Method | Endpoint       | Description               |
|--------|----------------|---------------------------|
| POST   | /Client        | Add a new client          |
| GET    | /Client/filter | Get clients using filters |
| GET    | /Client/list   | Get all clients           |
| PUT    | /Client/{id}   | Update client             |
| DELETE | /Client/{id}   | Delete client             |
📌 Note: NIF and contact fields have validation for Portuguese formats.

### 🏢 Publisher
| Method | Endpoint        | Description          |
|--------|-----------------|----------------------|
| POST   | /Publisher      | Add a new publisher  |
| GET    | /Publisher/{id} | Get publisher        |
| GET    | /Publisher/list | Get all publishers   |
| PUT    | /Publisher/{id} | Update publisher     |
| DELETE | /Publisher/{id} | Delete publisher     |

### 📦 Rent
| Method | Endpoint   | Description    |
|--------|------------|----------------|
| POST   | /Rent      | Add a new rent |
| GET    | /Rent/{id} | Get rent       |
| GET    | /Rent/list | Get all rents  |

### 📥 Rent Reception
| Method | Endpoint             | Description              |
|--------|----------------------|--------------------------|
| POST   | /Rent/reception      | Add a new rent reception |
| GET    | /Rent/{id}/reception | Get rent reception       |
| GET    | /Rent/ReceptionList  | Get all rent receptions  |



## 🚀 Getting Started
### Prerequisites
Ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)


### Running Locally
**1.** Clone the repository
```sh
git clone https://github.com/MadalenaAndrade/LibraryManagementAPI.git
cd LibraryManagementAPI
```

**2.** Set up you database on SSMS to get server domain

**3.** Configure Local Database on [`appsetting.json`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/appsettings.json) to match your local SQL Server setup
```sh
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME\\\\SQLEXPRESS;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```
Note: You can introduce a default connection string on [`LibraryDbContext.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/LibraryDbContext.cs)

**4.** Add a Project Reference to EFCoreClasses on LibraryManagementAPI 

**5.** Set LibraryManagementAPI as Startup Project

**6.** Apply Migrations (Code-First Approach) on Package Manager Console
```sh
Add-Migration InitialCreate
Update-Database
```

**7.** Run and test the API locally on Swagger

### How to Deploy to Azure 
This project can be deployed to Microsoft Azure by following these steps:

**1.** Create a **Resource Group** in Azure <br>
**2.** Set up an **App Service** to host the API in Azure <br>
**3.** Export a **BACPAC file** of the database in SSMS to migrate it To Azure <br>
**4.** Create a **SQL Server Instance** In Azure <br>
**5.** Connect to your **Azure SQL Server** in SSMS *(Make sure to allow your IP in the Azure SQL Server firewall settings.)* <br>
**6.** Import the **BACPAC file** to create the database in Azure <br>
**7.** Retrieve the **connection string** from the **Azure SQL Database** settings <br>
**8.** Replace the **connection string** in [`appsetting.json`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/appsettings.json)
```sh
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_AZURE_SQL_SERVER_DOMAIN;Initial Catalog=LibraryCodeFirst;Persist Security Info=False;User ID={your user};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}
```
**9.** Download the **Publish Profile** from the **Azure App Service** <br>
**10.** In **Visual Studio**, right-click on **LibraryManagementAPI** -> **Publish...** <br>
**11.** Import the **Publish Profile** and deploy the API.

## ⚠️ Note
This project is still in active development, and its structure and features may change as improvements are made.
