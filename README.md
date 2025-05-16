# Library Management API

This is a training project where I built a RESTful API to manage a fictitious library database, using **ASP.NET Core 8** and **Entity Framework Core**. The goal was to practice API development and database management concepts, with incremental improvements and new features being added over time.
The database was created using the **Code First** approach, based on a similar [SQL repository](https://github.com/MadalenaAndrade/LibraryDB-SQL-Scripts/blob/main/FullLibraryQuery.sql) I have worked on. Additionally, among new concepts I have used this project to learn **Entity Framework Core**, an **Object-Relational Mapper (ORM)** for .NET, as I was previously unfamiliar with it.

This project supports both **local development** (running with SQL Server in your machine) and deployment to **Azure** (hosted API and database), with the option to use Terraform for automating the infrastructure deployment.

## 📌 Table of Contents

- [🛠 Technologies Used](#-technologies-used)
- [🚀 Current Progress](#-current-progress)
- [📂 Main Project Structure](#-main-project-structure)
- [📡 API Endpoints](#-api-endpoints)
- [🚀 Getting Started (Local, Azure and Terraform)](#-getting-started)
- [🌐 Frontend Project](#-frontend-project)
- [⚠️ Note](#️-note)

## 🛠 Technologies Used

- **Visual Studio** - IDE for project development
- **Visual Studio Code** (optional) – Used for working with Terraform configuration files
- **ASP.NET Core 8** - For building the RESTful API
- **Entity Framework Core** - For database management
- **C#** - Main programming language
- **SQL Server** - For data storage
- **Swagger / Swashbuckle** - For API documentation
- **SQL Server Management Studio(SSMS)** - for database migration to Azure
- **Microsoft Azure** - Cloud hosting (App Service & SQL Database)
- **Terraform** (optional) – For infrastructure automation and deployment

## 🚀 Current Progress

- **Code First approach** - Creating models and DbContext using **data annotations** and **Fluent API**
- **Database migration** configured
- **Basic CRUD operations** implemented
- **DTOs** added for structured data transfer
- **Custom attributes** setup
- **Swagger UI** integrated for API documentation
- API deployed to **Microsoft Azure**
- **Terraform configuration** used (optional) for infrastructure deployment in **Microsoft Azure**

## 📂 Main Project Structure

📦 [**LibraryManagement**](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement) <br>
├─ 📚 [**EFCoreClasses**](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/EFCoreClasses) # Library class project <br>
│ ├─ 📂 [_Configurations_](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/EFCoreClasses) # Fluent API entity configurations <br>
│ │ ├─ 📄 [`BookAuthorConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/BookAuthorConfiguration.cs) <br>
│ │ ├─ 📄 [`BookCategoryConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/BookCategoryConfiguration.cs)  
│ │ ├─ 📄 [`BookConditionConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/BookConditionConfiguration.cs)  
│ │ ├─ 📄 [`BookConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/BookConfiguration.cs) <br>
│ │ ├─ 📄 [`BookCopyConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/BookCopyConfiguration.cs) <br>
│ │ ├─ 📄 [`BookStockConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/BookStockConfiguration.cs) <br>
│ │ ├─ 📄 [`ClientConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/ClientConfiguration.cs) <br>
│ │ ├─ 📄 [`RentConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/RentConfiguration.cs) <br>
│ │ └─ 📄 [`RentReceptionConfiguration.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Configurations/RentReceptionConfiguration.cs) <br>
│ ├─ 📂 [_Migrations_](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/EFCoreClasses/Migrations) # Database migration files <br>
│ ├─ 📂 [_Models_](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/EFCoreClasses/Models) # Database models <br>
│ │ ├─ 📄 [`Author.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/Author.cs) <br>
│ │ ├─ 📄 [`Book.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/Book.cs) <br>
│ │ ├─ 📄 [`BookAuthor.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/BookAuthor.cs) <br>
│ │ ├─ 📄 [`BookCategory.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/BookCategory.cs) <br>
│ │ ├─ 📄 [`BookCondition.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/BookCondition.cs)<br>
│ │ ├─ 📄 [`BookCopy.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/BookCopy.cs) <br>
│ │ ├─ 📄 [`BookStock.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/BookStock.cs) <br>
│ │ ├─ 📄 [`Category.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/Category.cs) <br>
│ │ ├─ 📄 [`Client.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/Client.cs) <br>
│ │ ├─ 📄 [`Publisher.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/Publisher.cs) <br>
│ │ ├─ 📄 [`Rent.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/Rent.cs) <br>
│ │ └─ 📄 [`RentReception.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/Models/RentReception.cs)  
│ └─ 📄 [`LibraryDbContext.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/LibraryDbContext.cs) # Database context <br>
├─ 🌐 [**LibraryManagementAPI**](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/LibraryManagementAPI) # ASP.NET Core Web API project <br>
│ ├─ 📂 [_Controllers_](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/LibraryManagementAPI/Controllers) # API endpoints for CRUD operations <br>
│ │ ├─ 📄 [`AuthorController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/AuthorController.cs) <br>
│ │ ├─ 📄 [`BookController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/BookController.cs) <br>
│ │ ├─ 📄 [`BookCopyController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/BookCopyController.cs) <br>
│ │ ├─ 📄 [`CategoryController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/CategoryController.cs) <br>
│ │ ├─ 📄 [`ClientController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/ClientController.cs) <br>
│ │ ├─ 📄 [`PublisherController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/PublisherController.cs) <br>
│ │ └─ 📄 [`RentController.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Controllers/RentController.cs) <br>
│ ├─ 📂 [_DTOs_](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/LibraryManagementAPI/DTOs) # Data Transfer Objects <br>
│ │ ├─ 📄 [`AuthorDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/AuthorDto.cs) <br>
│ │ ├─ 📄 [`BookDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/BookDto.cs) <br>
│ │ ├─ 📄 [`BookCopyDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/BookCopyDto.cs) <br>
│ │ ├─ 📄 [`CategoryDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/CategoryDto.cs) <br>
│ │ ├─ 📄 [`ClientDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/ClientDto.cs) <br>
│ │ ├─ 📄 [`PublisherDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/PublisherDto.cs) <br>
│ │ ├─ 📄 [`RentDto.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/RentDto.cs) <br>
│ │ └─ 📄 [`CustomAttributes.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/DTOs/CustomAttributes.cs) <br>
│ ├─ 📂 [_Middlewares_](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/LibraryManagementAPI/Middlewares) # Custom middleware components <br>
│ │ └─ 📄 [`AdminSafeListMiddleware.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Middlewares/AdminSafeListMiddleware.cs) # Handles IP whitelisting for admin routes<br>
│ ├─ 📂 [_Properties_](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/LibraryManagementAPI/Properties) # Launch settings <br>
│ ├─ 📄 [`Program.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/Program.cs) # Entry point of the Web API <br>
│ └─ 📂 [_wwwroot_](https://github.com/MadalenaAndrade/LibraryManagementAPI/tree/main/LibraryManagement/LibraryManagementAPI/wwwroot) # Frontend static files <br>
└─ 🧾 [`README.md`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/README.md)<br>

## 📡 API Endpoints

### 📝 Author

| Method | Endpoint     | Description      |
| ------ | ------------ | ---------------- |
| POST   | /Author      | Add a new author |
| GET    | /Author/{id} | Get author       |
| GET    | /Author/list | Get all authors  |
| PUT    | /Author/{id} | Update author    |
| DELETE | /Author/{id} | Delete author    |

### 📚 Book

| Method | Endpoint             | Description             |
| ------ | -------------------- | ----------------------- |
| POST   | /Book                | Add a new book          |
| GET    | /Book/filter         | Get books using filters |
| GET    | /Book/list           | Get all books           |
| PUT    | /Book/{serialNumber} | Update book             |
| DELETE | /Book/{serialNumber} | Delete book             |

### 📖 Book Copy

| Method | Endpoint         | Description                   |
| ------ | ---------------- | ----------------------------- |
| POST   | /BookCopy        | Add a new book copy           |
| GET    | /BookCopy/filter | Get book copies using filters |
| GET    | /BookCopy/list   | Get all book copies           |
| PUT    | /BookCopy/{id}   | Update book copy              |
| DELETE | /BookCopy/{id}   | Delete book copy              |

### 🏷️ Category

| Method | Endpoint       | Description        |
| ------ | -------------- | ------------------ |
| POST   | /Category      | Add a new category |
| GET    | /Category/{id} | Get category       |
| GET    | /Category/list | Get all categories |
| PUT    | /Category/{id} | Update category    |
| DELETE | /Category/{id} | Delete category    |

### 👤 Client

| Method | Endpoint       | Description               |
| ------ | -------------- | ------------------------- |
| POST   | /Client        | Add a new client          |
| GET    | /Client/filter | Get clients using filters |
| GET    | /Client/list   | Get all clients           |
| PUT    | /Client/{id}   | Update client             |
| DELETE | /Client/{id}   | Delete client             |

📌 Note: NIF and contact fields have validation for Portuguese formats.

### 🏢 Publisher

| Method | Endpoint        | Description         |
| ------ | --------------- | ------------------- |
| POST   | /Publisher      | Add a new publisher |
| GET    | /Publisher/{id} | Get publisher       |
| GET    | /Publisher/list | Get all publishers  |
| PUT    | /Publisher/{id} | Update publisher    |
| DELETE | /Publisher/{id} | Delete publisher    |

### 📦 Rent

| Method | Endpoint   | Description    |
| ------ | ---------- | -------------- |
| POST   | /Rent      | Add a new rent |
| GET    | /Rent/{id} | Get rent       |
| GET    | /Rent/list | Get all rents  |

### 📥 Rent Reception

| Method | Endpoint             | Description              |
| ------ | -------------------- | ------------------------ |
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

**3.** Configure Local Database on [`appsetting.Development.json`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/appsettings.Development.json) to match your local SQL Server setup

```sh
"LibraryHubDatabase": "Server=YOUR_SERVER_NAME\\\\SQLEXPRESS;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True"
```

> **Note1:** You can introduce a default connection string on [`LibraryDbContext.cs`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/EFCoreClasses/LibraryDbContext.cs) <br>  **Note2:** For local development, you can also add the AdminSafeList key in your appsetting.Development.json file with the allowed IP addresses separated by commas.

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

**5.** Connect to your **Azure SQL Server** in SSMS _(Make sure to allow your IP in the Azure SQL Server firewall settings.)_ <br>

**6.** Import the **BACPAC file** to create the database in Azure <br>

**7.** Configure authentication with **Microsoft Entra ID**

- In the Azure portal, go to your **App Service** instance and enable authentication with Microsoft Entra ID.
- In Azure SQL Database, run the following **queries in SSMS** to create the API user and assign permissions:

```sh
CREATE USER Libmanagementapi FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER Libmanagementapi;
ALTER ROLE db_datawriter ADD MEMBER Libmanagementapi;
ALTER ROLE db_ddladmin ADD MEMBER Libmanagementapi;
```

**8.** Retrieve the **Connection String** with Microsoft Entra Integrated Authentication from the **Azure SQL Database** settings <br>

- In **Azure**, go to App Service > Settings > Environment Variables and add:

  - Name: `LibraryHubDatabase` <br>
    Value: your connection string
  - Name: `AdminSafeList` <br>
    Value: comma-separated list of allowed IP addresses for admin access

- Replace the **connection string** in [`appsetting.Development.json`](https://github.com/MadalenaAndrade/LibraryManagementAPI/blob/main/LibraryManagement/LibraryManagementAPI/appsettings.json)

```sh
"LibraryHubDatabase": "Server=YOUR_AZURE_SQL_SERVER;Initial Catalog=LibraryCodeFirst;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;"
```

> **Important:** These environment variable are used by the application in production. The appsettings.Development.json file is only used for local development and should not be published or committed to source control.

**9.** Download the **Publish Profile** from the **Azure App Service** <br>

**10.** In **Visual Studio**, right-click on **LibraryManagementAPI** -> **Publish...** <br>

**11.** Import the **Publish Profile** and deploy the API.

### Deploying with Terraform

Alternatively, you can automate the deployment using an infrastructure just as Terraform by following these steps: <br>
**1.** **Download** and **Install** [Terraform](https://www.terraform.io/) <br>

**2.** **Set up** Terraform configuration

- Ensure you have `providers.tf`:

```sh
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }
}

provider "azurerm" {
  features {}
  skip_provider_registration = true
}
```

- Define resources in `main.tf`, such as resource group, dabatase and web app:

```sh
data "azurerm_client_config" "current" {}
data "azuread_user" "current_user" {
  object_id = data.azurerm_client_config.current.object_id
}

resource "azurerm_resource_group" "rg" {
  location = "westeurope"
  name     = "ResourceGroupNameExample-RG"
  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_mssql_server" "sql_server" {
  name                = "SQLServerNameExample"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  version             = "12.0"
  azuread_administrator {
    login_username              = data.azuread_user.current_user.user_principal_name
    object_id                   = data.azurerm_client_config.current.object_id
    azuread_authentication_only = false
  }
}

resource "azurerm_mssql_database" "sql_db" {
  name      = "SQLDatabaseNameExample"
  server_id = azurerm_mssql_server.sql_server.id
}

resource "azurerm_service_plan" "app_service_plan" {
  name                = "ASP-ResourceGroupName"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Windows"
  sku_name            = "F1"
}

resource "azurerm_windows_web_app" "app_service" {
  name                = "ApiNameExample"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_service_plan.app_service_plan.location
  service_plan_id     = azurerm_service_plan.app_service_plan.id
}
```

**3.** Run the commands on the directory containing main.tf to initialize and apply Terraform configurations

```sh
terraform init
terraform apply
```

- Terraform will show you a plan and ask for confirmation before applying changes.

**4.** Ensure existing resources are not recreated

- Terraform will automatically check for existing resources and use them instead of creating duplicates.

**5.** Configure Microsoft Entra ID authentication manually if needed

**6.** Deploy the API

**Additional Tips** <br>
Terraform Extension for Visual Studio Code: For a better development experience, you may install the HashiCorp Terraform extension for Visual Studio Code. It provides syntax highlighting, autocompletion, and other helpful features.

## 🌐 Frontend Project

A separate project was developed to experiment with React and demonstrate how this API can be consumed from a web interface. The frontend interacts with the endpoints provided by this API to fetch and display data in a dynamic, user-friendly way.<br>
📁 [Frontend Repository](https://github.com/MadalenaAndrade/LibraryManagementUI)<br>
🌍 [Live Demo](https://libmanagementapi-ashnc2hsh3gma6fc.westeurope-01.azurewebsites.net)

To support the frontend on the same server as the API, the following adjustment was made:

- A wwwroot folder was added to serve the compiled frontend files.<br>
- Configured the Program.cs to serve static files by enabling middleware with: app.UseDefaultFiles(); app.UseStaticFiles(); app.MapFallbackToFile("index.html");

Note: In the deployed version, the API has IP restrictions in place for security reasons. This means that while the frontend interface may allow actions like creating or editing records, the API will respond with Access Denied for those requests unless the client's IP is authorized. Only GET requests are expected to work successfully in the live demo.

## ⚠️ Note

For security reasons, no sensitive files such as Terraform configuration files or credentials are included in this repository. Please follow the instructions in the Getting Started section to set up your own environment and configure the necessary credentials locally.
