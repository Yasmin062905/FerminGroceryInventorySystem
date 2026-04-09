# 🛒 FerminGroceryInventorySystem

A console-based C# application that manages grocery store inventory by tracking products, categories, suppliers, stock levels, and transactions — designed to ensure efficient and organized store operations.

---

## 📋 Table of Contents

- [Features](#features)
- [Demo Accounts](#demo-accounts)
- [Menu Overview](#menu-overview)
- [Data Models](#data-models)
- [Getting Started](#getting-started)
- [How to Use](#how-to-use)
- [Project Structure](#project-structure)

---

## ✨ Features

- 🔐 **User Authentication** — Role-based login system with Admin and Staff accounts
- 📦 **Product Management** — Add, view, search, update, and delete products
- 🏷️ **Category Management** — Organize products into categories (Admin only)
- 🚚 **Supplier Management** — Track supplier names, contact info, and addresses (Admin only)
- 📈 **Stock Management** — Restock and deduct product quantities with transaction logging
- 📊 **Reports** — View transaction history, low stock alerts, and total inventory value
- ⚠️ **Low Stock Alerts** — Real-time banner on the main menu when items fall below threshold
- ✅ **Input Validation** — All inputs are validated with clear error messages

---

## 🔑 Demo Accounts

| Username | Password   | Role  | Access Level        |
|----------|------------|-------|---------------------|
| `admin`  | `admin123` | Admin | Full access (all 14 features) |
| `staff`  | `staff123` | Staff | Limited access (7 features)   |

---

## 📂 Menu Overview

### Admin Menu (Full Access)

| # | Feature |
|---|---------|
| 1 | Add Category |
| 2 | View Categories |
| 3 | Add Supplier |
| 4 | View Suppliers |
| 5 | Add Product |
| 6 | View All Products |
| 7 | Search Product |
| 8 | Update Product |
| 9 | Delete Product |
| 10 | Restock Product |
| 11 | Deduct Stock |
| 12 | Transaction History |
| 13 | Low Stock Items |
| 14 | Total Inventory Value |
| 0 | Logout |

### Staff Menu (Limited Access)

| # | Feature |
|---|---------|
| 1 | View All Products |
| 2 | Search Product |
| 3 | Restock Product |
| 4 | Deduct Stock |
| 5 | Transaction History |
| 6 | Low Stock Items |
| 7 | Total Inventory Value |
| 0 | Logout |

---

## 🗂️ Data Models

### Category
- `CategoryID` — Unique identifier
- `Name` — Category name (e.g., Dairy, Produce, Beverages)
- `Description` — Short description of the category

### Supplier
- `SupplierID` — Unique identifier
- `Name` — Supplier company name
- `ContactInfo` — Phone number (digits, +, - only)
- `Address` — Supplier location

### Product
- `ProductID` — Unique identifier
- `Name` — Product name
- `CategoryID` — Linked category
- `SupplierID` — Linked supplier
- `Price` — Unit price in Philippine Peso (₱)
- `StockQuantity` — Current stock level
- `LowStockThreshold` — Alert threshold (default: 10)

### TransactionRecord
- `TransactionID` — Unique identifier
- `Timestamp` — Date and time of transaction
- `ProductID` — Linked product
- `ActionType` — e.g., `Initial Stock`, `Restock`, `Deduct`
- `QuantityChanged` — Number of units affected
- `Amount` — Total value of the transaction
- `Notes` — Additional notes (e.g., who performed the action)

### User
- `UserID` — Unique identifier
- `Username` — Login username
- `Password` — Login password
- `Role` — `Admin` or `Staff`

---

## 🚀 Getting Started

### Prerequisites
- [.NET Framework](https://dotnet.microsoft.com/) or .NET SDK installed
- Visual Studio or any C#-compatible IDE

### Run the Project

1. Clone the repository:
   ```bash
   git clone https://github.com/Yasmin062905/FerminGroceryInventorySystem.git
   ```

2. Open the solution in Visual Studio or navigate to the project folder.

3. Build and run:
   ```bash
   dotnet run
   ```

4. Log in using one of the [demo accounts](#demo-accounts).

---

## 🧭 How to Use

1. **Login** — Enter your username and password at the login screen.
2. **Navigate the menu** — Type the number of the action you want and press Enter.
3. **Manage products** — Add new products with a name, price, category, supplier, and stock quantity.
4. **Restock / Deduct** — Update stock quantities; every change is automatically logged as a transaction.
5. **Check reports** — View transaction history, identify low stock items, or calculate total inventory value.
6. **Logout** — Press `0` from the main menu when done.

> ⚠️ A **Low Stock Alert** banner appears on the main menu whenever any product's stock is at or below its threshold.

---

## 🗃️ Project Structure

```
FerminGroceryInventorySystem/
├── Properties/
│   └── AssemblyInfo.cs        # (removed)
├── App.config                 # Application configuration
├── Program.cs                 # Main application logic & all models
├── FerminGroceryInventorySystem.csproj
├── .gitignore
└── README.md
```

---

## 👤 Author

**Yasmin062905** — [GitHub Profile](https://github.com/Yasmin062905)
