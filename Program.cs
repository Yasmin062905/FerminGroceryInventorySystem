using System;
using System.Collections.Generic;
using System.Linq;

namespace FerminGroceryInventorySystem
{
	// ====================== MODELS ======================
	public class Category
	{
		public int CategoryID { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }

		public Category(int id, string name, string description)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Category name cannot be empty.");
			CategoryID = id;
			Name = name.Trim();
			Description = description?.Trim() ?? string.Empty;
		}
		public override string ToString() => $"[{CategoryID}] {Name} - {Description}";
	}

	public class Supplier
	{
		public int SupplierID { get; private set; }
		public string Name { get; private set; }
		public string ContactInfo { get; private set; }
		public string Address { get; private set; }

		public Supplier(int id, string name, string contactInfo, string address)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Supplier name cannot be empty.");
			SupplierID = id;
			Name = name.Trim();
			ContactInfo = contactInfo?.Trim() ?? "N/A";
			Address = address?.Trim() ?? "N/A";
		}
		public override string ToString() => $"[{SupplierID}] {Name} | Contact: {ContactInfo}";
	}

	public class Product
	{
		public int ProductID { get; private set; }
		public string Name { get; set; }
		public int CategoryID { get; set; }
		public int SupplierID { get; set; }
		public decimal Price { get; set; }
		public int StockQuantity { get; private set; }
		public int LowStockThreshold { get; set; }

		public Product(int id, string name, int categoryId, int supplierId, decimal price, int stockQuantity, int lowStockThreshold = 10)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name cannot be empty.");
			if (price <= 0) throw new ArgumentException("Price must be greater than 0.");
			if (stockQuantity < 0) throw new ArgumentException("Stock cannot be negative.");

			ProductID = id;
			Name = name.Trim();
			CategoryID = categoryId;
			SupplierID = supplierId;
			Price = price;
			StockQuantity = stockQuantity;
			LowStockThreshold = lowStockThreshold;
		}

		public void Restock(int quantity)
		{
			if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
			StockQuantity += quantity;
		}

		public void Deduct(int quantity)
		{
			if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
			if (quantity > StockQuantity) throw new InvalidOperationException($"Only {StockQuantity} available!");
			StockQuantity -= quantity;
		}

		public override string ToString() => $"[{ProductID}] {Name} | Price: {Price:C} | Stock: {StockQuantity}";
	}

	public class User
	{
		public int UserID { get; private set; }
		public string Username { get; private set; }
		public string Password { get; private set; }
		public string Role { get; private set; }

		public User(int id, string username, string password, string role)
		{
			UserID = id;
			Username = username;
			Password = password;
			Role = role;
		}
	}

	public class TransactionRecord
	{
		public int TransactionID { get; private set; }
		public DateTime Timestamp { get; private set; }
		public int ProductID { get; private set; }
		public string ActionType { get; private set; }
		public int QuantityChanged { get; private set; }
		public decimal Amount { get; private set; }
		public string Notes { get; private set; }

		public TransactionRecord(int id, int productId, string actionType, int quantityChanged, decimal amount, string notes)
		{
			TransactionID = id;
			Timestamp = DateTime.Now;
			ProductID = productId;
			ActionType = actionType;
			QuantityChanged = quantityChanged;
			Amount = amount;
			Notes = notes ?? string.Empty;
		}

		public override string ToString() => $"{Timestamp:yyyy-MM-dd HH:mm} | {ActionType,-12} | Qty: {QuantityChanged,4} | {Amount:C}";
	}

	// ====================== MAIN PROGRAM ======================
	class Program
	{
		private static List<Category> categories = new List<Category>();
		private static List<Supplier> suppliers = new List<Supplier>();
		private static List<Product> products = new List<Product>();
		private static List<TransactionRecord> transactions = new List<TransactionRecord>();
		private static List<User> users = new List<User>();

		private static int nextCategoryID = 1, nextSupplierID = 1, nextProductID = 1, nextTransactionID = 1;
		private static User currentUser = null;

		static void Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			InitializeDemoData();
			while (true)
			{
				Login();
				RunMainMenu();
			}
		}

		private static void InitializeDemoData()
		{
			users.Add(new User(1, "admin", "admin123", "Admin"));
			users.Add(new User(2, "staff", "staff123", "Staff"));

			categories.Add(new Category(nextCategoryID++, "Dairy", "Milk, cheese, yogurt"));
			categories.Add(new Category(nextCategoryID++, "Produce", "Fruits and vegetables"));
			categories.Add(new Category(nextCategoryID++, "Beverages", "Soft drinks & juices"));

			suppliers.Add(new Supplier(nextSupplierID++, "Golden Farms", "0917-123-4567", "Makati City"));
			suppliers.Add(new Supplier(nextSupplierID++, "FreshCo Inc.", "0918-987-6543", "Quezon City"));

			var p1 = new Product(nextProductID++, "Fresh Milk 1L", 1, 1, 85.50m, 120);
			products.Add(p1);
			AddTransaction(p1.ProductID, "Initial Stock", 120, p1.Price * 120, "Demo");

			var p2 = new Product(nextProductID++, "Red Apples (kg)", 2, 2, 120.00m, 5);
			products.Add(p2);
			AddTransaction(p2.ProductID, "Initial Stock", 5, p2.Price * 5, "Demo");
		}

		private static void Login()
		{
			while (true)
			{
				HardClear();
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
				Console.WriteLine("║              FERMIN GROCERY INVENTORY SYSTEM                 ║");
				Console.WriteLine("║                       LOGIN REQUIRED                         ║");
				Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
				Console.ResetColor();

				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine("Demo Accounts:");
				Console.WriteLine("   Username: admin     Password: admin123     (Full Access)");
				Console.WriteLine("   Username: staff     Password: staff123   (Limited Access)");
				Console.ResetColor();
				Console.WriteLine();

				// Step 1: Validate username first
				Console.Write("Username: ");
				string username = Console.ReadLine()?.Trim();

				if (string.IsNullOrWhiteSpace(username))
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("\n❌ Username cannot be empty.");
					Console.ResetColor();
					Console.WriteLine("\nPress any key to try again...");
					Console.ReadKey();
					continue;
				}

				bool usernameExists = users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
				if (!usernameExists)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"\n❌ Username '{username}' not found.");
					Console.ResetColor();
					Console.WriteLine("\nPress any key to try again...");
					Console.ReadKey();
					continue;
				}

				// Step 2: Username valid — now ask for password
				Console.Write("Password: ");
				string password = Console.ReadLine()?.Trim();

				var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password);

				if (user != null)
				{
					currentUser = user;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"\n✅ Login successful! Welcome, {currentUser.Username} ({currentUser.Role})!");
					Console.ResetColor();
					Console.WriteLine("\nPress any key to continue...");
					Console.ReadKey();
					return;
				}

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("\n❌ Incorrect password. Try again.");
				Console.ResetColor();
				Console.WriteLine("\nPress any key to try again...");
				Console.ReadKey();
			}
		}

		private static void RunMainMenu()
		{
			bool isAdmin = currentUser.Role == "Admin";

			// Admin: choices 0-14  |  Staff: choices 0-6
			// Staff menu mapping: 1=ViewAllProducts, 2=SearchProduct, 3=RestockProduct,
			//                     4=DeductStock, 5=ViewTransactionHistory, 6=ShowLowStockItems
			// (Total Inventory Value is accessible via choice 6 on staff — see display)
			// Actually staff has 7 options: 1-6 + 0 logout. Let's map:
			//   1=ViewAllProducts, 2=SearchProduct, 3=RestockProduct,
			//   4=DeductStock, 5=ViewTransactionHistory, 6=ShowLowStockItems, 7=ComputeTotalInventoryValue
			int maxChoice = isAdmin ? 14 : 7;

			while (true)
			{
				HardClear();
				DisplayMainMenu();

				int choice = GetValidatedIntInput("Enter your choice: ", 0, maxChoice);

				// Remap staff choices to internal action numbers
				int action = choice;
				if (!isAdmin && choice >= 1)
				{
					// Staff choice -> internal action
					// 1 -> ViewAllProducts (was 6)
					// 2 -> SearchProduct (was 7)
					// 3 -> RestockProduct (was 10)
					// 4 -> DeductStock (was 11)
					// 5 -> ViewTransactionHistory (was 12)
					// 6 -> ShowLowStockItems (was 13)
					// 7 -> ComputeTotalInventoryValue (was 14)
					int[] staffMap = { 0, 6, 7, 10, 11, 12, 13, 14 }; // index 0 unused
					action = staffMap[choice];
				}

				if (choice == 0)
				{
					HardClear();
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("👋 Logging out... Thank you!");
					Console.ResetColor();
					System.Threading.Thread.Sleep(1500);
					currentUser = null;
					return;
				}

				HardClear();

				bool skipPause = false;
				try
				{
					switch (action)
					{
						case 1: AddCategory(); skipPause = true; break;
						case 2: ViewCategories(); break;
						case 3: AddSupplier(); skipPause = true; break;
						case 4: ViewSuppliers(); break;
						case 5: AddProduct(); skipPause = true; break;
						case 6: ViewAllProducts(); break;
						case 7: SearchProduct(); break;
						case 8: UpdateProduct(); break;
						case 9: DeleteProduct(); break;
						case 10: RestockProduct(); skipPause = true; break;
						case 11: DeductStock(); skipPause = true; break;
						case 12: ViewTransactionHistory(); break;
						case 13: ShowLowStockItems(); break;
						case 14: ComputeTotalInventoryValue(); break;
					}
				}
				catch (Exception ex)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"\n❌ ERROR: {ex.Message}");
					Console.ResetColor();
					skipPause = false;
				}

				if (!skipPause)
				{
					Console.WriteLine("\nPress any key to return to the main menu...");
					Console.ReadKey();
				}
			}
		}

		private static void DisplayMainMenu()
		{
			bool isAdmin = currentUser.Role == "Admin";

			// Title — Cyan
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
			Console.WriteLine("║                    FERMIN GROCERY INVENTORY SYSTEM                           ║");
			Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");

			// Centered username line
			string userLine = $"Logged in as: {currentUser.Username}";
			int totalWidth = 78; // inner width of the box
			int padding = (totalWidth - userLine.Length) / 2;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(new string(' ', padding) + userLine);
			Console.WriteLine();

			void GreenLine(string line)
			{
				if (line.Length == 0) { Console.WriteLine(); return; }
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write(line[0]);
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(line.Substring(1, line.Length - 2));
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(line[line.Length - 1]);
			}
			void GreenSolid(string line)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(line);
			}

			if (isAdmin)
			{
				// CATEGORY & SUPPLIER — Admin only
				GreenSolid("╔══════════════════════════════════════════════════════════════════════════════╗");
				GreenLine("║ CATEGORY & SUPPLIER ─────────────────────────────────────────────────────────║");
				GreenLine("║  [1] Add Category              [2] View Categories                           ║");
				GreenLine("║  [3] Add Supplier              [4] View Suppliers                            ║");
				GreenSolid("╚══════════════════════════════════════════════════════════════════════════════╝");
				Console.WriteLine();

				// PRODUCTS — Admin full
				GreenSolid("╔══════════════════════════════════════════════════════════════════════════════╗");
				GreenLine("║ PRODUCTS ────────────────────────────────────────────────────────────────────║");
				GreenLine("║  [5] Add Product               [6] View All Products                         ║");
				GreenLine("║  [7] Search Product            [8] Update Product                            ║");
				GreenLine("║  [9] Delete Product                                                          ║");
				GreenSolid("╚══════════════════════════════════════════════════════════════════════════════╝");
				Console.WriteLine();
			}
			else
			{
				// PRODUCTS — Staff view/search only
				GreenSolid("╔══════════════════════════════════════════════════════════════════════════════╗");
				GreenLine("║ PRODUCTS ────────────────────────────────────────────────────────────────────║");
				GreenLine("║  [1] View All Products         [2] Search Product                            ║");
				GreenSolid("╚══════════════════════════════════════════════════════════════════════════════╝");
				Console.WriteLine();
			}

			// STOCK MANAGEMENT — both roles
			GreenSolid("╔══════════════════════════════════════════════════════════════════════════════╗");
			GreenLine("║ STOCK MANAGEMENT ────────────────────────────────────────────────────────────║");
			if (isAdmin)
				GreenLine("║ [10] Restock Product          [11] Deduct Stock                              ║");
			else
				GreenLine("║  [3] Restock Product           [4] Deduct Stock                              ║");
			GreenSolid("╚══════════════════════════════════════════════════════════════════════════════╝");
			Console.WriteLine();

			// REPORTS — both roles
			GreenSolid("╔══════════════════════════════════════════════════════════════════════════════╗");
			GreenLine("║ REPORTS ─────────────────────────────────────────────────────────────────────║");
			if (isAdmin)
			{
				GreenLine("║ [12] Transaction History       [13] Low Stock Items                          ║");
				GreenLine("║ [14] Total Inventory Value                                                   ║");
			}
			else
			{
				GreenLine("║  [5] Transaction History        [6] Low Stock Items                          ║");
				GreenLine("║  [7] Total Inventory Value                                                   ║");
			}
			GreenSolid("╚══════════════════════════════════════════════════════════════════════════════╝");
			Console.WriteLine();

			// SYSTEM — both roles
			GreenSolid("╔══════════════════════════════════════════════════════════════════════════════╗");
			GreenLine("║ SYSTEM ──────────────────────────────────────────────────────────────────────║");
			GreenLine("║  [0] Logout                                                                  ║");
			GreenSolid("╚══════════════════════════════════════════════════════════════════════════════╝");

			// Low stock warning banner
			var lowStockItems = products.Where(p => p.StockQuantity <= p.LowStockThreshold).ToList();
			if (lowStockItems.Count > 0)
			{
				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("╔══════════════════════════════════════════════════════════════════════════════╗");
				Console.Write("║");
				Console.ForegroundColor = ConsoleColor.Red;
				string warning = $" ⚠  LOW STOCK ALERT: {lowStockItems.Count} item(s) need restocking!";
				string warningPadded = warning.PadRight(77);
				Console.Write(warningPadded);
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("║");
				foreach (var item in lowStockItems)
				{
					Console.Write("║");
					Console.ForegroundColor = ConsoleColor.White;
					string itemLine = $"   • {item.Name} — Stock: {item.StockQuantity} (threshold: {item.LowStockThreshold})";
					Console.Write(itemLine.PadRight(77));
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("║");
				}
				Console.WriteLine("╚══════════════════════════════════════════════════════════════════════════════╝");
			}

			Console.ResetColor();
			Console.WriteLine();
		}

		// ====================== HELPERS ======================
		private static void HardClear()
		{
			// \x1b[2J  = clear visible screen
			// \x1b[3J  = clear scrollback buffer (prevents stacking in Windows Terminal)
			// \x1b[H   = move cursor to top-left
			try { Console.Write("\x1b[2J\x1b[3J\x1b[H"); }
			catch { }
			Console.Clear();
			Console.SetCursorPosition(0, 0);
		}

		private static Product AskProductID(string prompt)
		{
			while (true)
			{
				int id = GetValidatedIntInput(prompt);
				var product = products.FirstOrDefault(p => p.ProductID == id);
				if (product != null) return product;

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("❌ Product ID not found.");
				Console.ResetColor();
				Console.Write("Try again? (y = retry / any other key = return to menu): ");
				string answer = Console.ReadLine()?.Trim().ToLower();
				if (answer != "y") return null;
			}
		}

		private static int GetValidatedIntInput(string prompt, int min = int.MinValue, int max = int.MaxValue)
		{
			while (true)
			{
				Console.Write(prompt);
				if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
					return value;

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("❌ Invalid input. Please try again.");
				Console.ResetColor();
			}
		}

		private static decimal GetValidatedDecimalInput(string prompt)
		{
			while (true)
			{
				Console.Write(prompt);
				if (decimal.TryParse(Console.ReadLine(), out decimal value) && value > 0)
					return value;

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("❌ Invalid input.");
				Console.ResetColor();
			}
		}

		private static void AddTransaction(int productId, string actionType, int quantity, decimal amount, string notes)
		{
			transactions.Add(new TransactionRecord(nextTransactionID++, productId, actionType, quantity, amount, notes));
		}

		private static string GetCategoryName(int id) => categories.FirstOrDefault(c => c.CategoryID == id)?.Name ?? "Unknown";
		private static string GetSupplierName(int id) => suppliers.FirstOrDefault(s => s.SupplierID == id)?.Name ?? "Unknown";
		private static string GetProductName(int id) => products.FirstOrDefault(p => p.ProductID == id)?.Name ?? "Unknown";

		// ====================== FEATURES ======================
		private static void AddCategory()
		{
			while (true)
			{
				Console.WriteLine("=== ADD CATEGORY ===");

				string name;
				while (true)
				{
					Console.Write("Enter Category Name: ");
					name = Console.ReadLine()?.Trim();
					if (!string.IsNullOrWhiteSpace(name)) break;

					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("❌ Category name cannot be empty.");
					Console.ResetColor();
					Console.Write("Try again? (y = retry / any other key = return to menu): ");
					if (Console.ReadLine()?.Trim().ToLower() != "y") return;
				}

				Console.Write("Enter Description (optional, press Enter to skip): ");
				string desc = Console.ReadLine()?.Trim();

				var cat = new Category(nextCategoryID++, name, desc);
				categories.Add(cat);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"✅ Category '{cat.Name}' added! ID: {cat.CategoryID}");
				Console.ResetColor();

				Console.Write("\nAdd another category? (y = yes / any other key = return to menu): ");
				if (Console.ReadLine()?.Trim().ToLower() != "y") return;
				Console.WriteLine();
			}
		}

		private static void ViewCategories()
		{
			Console.WriteLine("=== VIEW CATEGORIES ===");
			if (categories.Count == 0) { Console.WriteLine("No categories yet."); return; }
			categories.ForEach(c => Console.WriteLine(c));
		}

		private static void AddSupplier()
		{
			while (true)
			{
				Console.WriteLine("=== ADD SUPPLIER ===");

				string name;
				while (true)
				{
					Console.Write("Supplier Name: ");
					name = Console.ReadLine()?.Trim();
					if (!string.IsNullOrWhiteSpace(name)) break;

					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("❌ Supplier name cannot be empty.");
					Console.ResetColor();
					Console.Write("Try again? (y = retry / any other key = return to menu): ");
					if (Console.ReadLine()?.Trim().ToLower() != "y") return;
				}

				string contact;
				while (true)
				{
					Console.Write("Contact Info: ");
					contact = Console.ReadLine()?.Trim();
					if (!string.IsNullOrWhiteSpace(contact) &&
						System.Text.RegularExpressions.Regex.IsMatch(contact, @"^[0-9\+\-\s]+$"))
						break;

					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("❌ Invalid contact. Enter a valid phone number (digits, +, - only).");
					Console.ResetColor();
				}

				Console.Write("Address (optional, press Enter to skip): ");
				string address = Console.ReadLine()?.Trim();

				var sup = new Supplier(nextSupplierID++, name, contact, address);
				suppliers.Add(sup);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"✅ Supplier '{sup.Name}' added! ID: {sup.SupplierID}");
				Console.ResetColor();

				Console.Write("\nAdd another supplier? (y = yes / any other key = return to menu): ");
				if (Console.ReadLine()?.Trim().ToLower() != "y") return;
				Console.WriteLine();
			}
		}

		private static void ViewSuppliers()
		{
			Console.WriteLine("=== VIEW SUPPLIERS ===");
			if (suppliers.Count == 0) { Console.WriteLine("No suppliers yet."); return; }
			suppliers.ForEach(s => Console.WriteLine(s));
		}

		private static void AddProduct()
		{
			Console.WriteLine("=== ADD PRODUCT ===");

			if (categories.Count == 0 || suppliers.Count == 0)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("❌ Please add at least one Category and one Supplier first!");
				Console.ResetColor();
				return;
			}

			Console.Write("Product Name: ");
			string name = Console.ReadLine()?.Trim();
			if (string.IsNullOrWhiteSpace(name))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("❌ Product name cannot be empty.");
				Console.ResetColor();
				return;
			}

			int catId;
			while (true)
			{
				Console.WriteLine("\nAvailable Categories:");
				foreach (var c in categories)
					Console.WriteLine($"[{c.CategoryID}] {c.Name} - {c.Description}");

				catId = GetValidatedIntInput("Category ID: ");
				if (categories.Any(c => c.CategoryID == catId)) break;

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("❌ Invalid Category ID. Please choose from the list above.");
				Console.ResetColor();
			}

			string catName = GetCategoryName(catId);
			int supId;
			while (true)
			{
				Console.WriteLine("\nAvailable Suppliers:");
				foreach (var s in suppliers)
					Console.WriteLine($"[{s.SupplierID}] {s.Name} | Contact: {s.ContactInfo}");

				supId = GetValidatedIntInput("Supplier ID: ");
				if (suppliers.Any(s => s.SupplierID == supId)) break;

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("❌ Invalid Supplier ID. Please choose from the list above.");
				Console.ResetColor();
			}

			decimal price = GetValidatedDecimalInput("Price (₱): ");
			int stock = GetValidatedIntInput("Initial Stock Quantity: ", 0);
			int threshold = GetValidatedIntInput("Low Stock Threshold: ", 1);

			var product = new Product(nextProductID++, name, catId, supId, price, stock, threshold);
			products.Add(product);
			AddTransaction(product.ProductID, "Initial Stock", stock, price * stock, "New Product");

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"\n✅ Product '{name}' added successfully! ID: {product.ProductID}");
			Console.ResetColor();

			Console.Write("\nAdd another product? (y = yes / any other key = return to menu): ");
			if (Console.ReadLine()?.Trim().ToLower() == "y")
			{
				Console.WriteLine();
				AddProduct();
			}
		}

		private static void PrintProductTable()
		{
			if (products.Count == 0) { Console.WriteLine("No products found."); return; }

			Console.WriteLine($"{"ID",-4}  {"Name",-22}  {"Category",-14}  {"Supplier",-13}  {"Price",9}  {"Stock",6}");
			Console.WriteLine(new string('─', 78));
			foreach (var p in products)
			{
				Console.WriteLine($"{p.ProductID,-4}  {p.Name,-22}  {GetCategoryName(p.CategoryID),-14}  " +
								  $"{GetSupplierName(p.SupplierID),-13}  {p.Price,9:C}  {p.StockQuantity,6}");
			}
		}

		private static void ViewAllProducts()
		{
			Console.WriteLine("=== VIEW ALL PRODUCTS ===");
			Console.WriteLine();
			if (products.Count == 0) { Console.WriteLine("No products found."); return; }
			PrintProductTable();
		}

		private static void SearchProduct()
		{
			Console.WriteLine("=== SEARCH PRODUCT ===");
			Console.Write("Enter name or ID: ");
			string term = Console.ReadLine()?.Trim().ToLower();

			var results = products.Where(p => p.Name.ToLower().Contains(term) || p.ProductID.ToString() == term).ToList();
			if (results.Count == 0) { Console.WriteLine("❌ No product found."); return; }

			Console.WriteLine("\nSearch Results:");
			results.ForEach(p => Console.WriteLine(p));
		}

		private static void UpdateProduct()
		{
			Console.WriteLine("=== UPDATE PRODUCT ===");
			ViewAllProducts();
			var product = AskProductID("\nEnter Product ID to update: ");
			if (product == null) return;

			Console.WriteLine("\n[1] Name  [2] Price  [3] Category  [4] Supplier  [5] Low Stock Threshold");
			int opt = GetValidatedIntInput("Choose (1-5): ", 1, 5);

			switch (opt)
			{
				case 1: Console.Write("New Name: "); string n = Console.ReadLine()?.Trim(); if (!string.IsNullOrEmpty(n)) product.Name = n; break;
				case 2: product.Price = GetValidatedDecimalInput("New Price: "); break;
				case 3: Console.WriteLine("\nCategories:"); categories.ForEach(c => Console.WriteLine(c)); product.CategoryID = GetValidatedIntInput("New ID: "); break;
				case 4: Console.WriteLine("\nSuppliers:"); suppliers.ForEach(s => Console.WriteLine(s)); product.SupplierID = GetValidatedIntInput("New ID: "); break;
				case 5: product.LowStockThreshold = GetValidatedIntInput("New Threshold: ", 1); break;
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("✅ Product updated successfully!");
			Console.ResetColor();
		}

		private static void DeleteProduct()
		{
			Console.WriteLine("=== DELETE PRODUCT ===");
			Console.WriteLine();
			PrintProductTable();
			var p = AskProductID("\nEnter Product ID to delete: ");
			if (p == null) return;

			Console.Write($"Delete '{p.Name}'? (y/n): ");
			if (Console.ReadLine()?.Trim().ToLower() == "y")
			{
				products.Remove(p);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("✅ Product deleted successfully!");
				Console.ResetColor();
			}
		}

		private static void RestockProduct()
		{
			while (true)
			{
				Console.WriteLine("=== RESTOCK PRODUCT ===");
				Console.WriteLine();
				PrintProductTable();
				var p = AskProductID("\nProduct ID: ");
				if (p == null) return;

				int qty = GetValidatedIntInput("Quantity to restock: ", 1);
				p.Restock(qty);
				AddTransaction(p.ProductID, "Restock", qty, p.Price * qty, $"By {currentUser.Username}");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"\u2705 Restocked! New stock: {p.StockQuantity}");
				Console.ResetColor();

				Console.Write("\nRestock another product? (y = yes / any other key = return to menu): ");
				if (Console.ReadLine()?.Trim().ToLower() != "y") return;
				Console.WriteLine();
			}
		}

		private static void DeductStock()
		{
			while (true)
			{
				Console.WriteLine("=== DEDUCT STOCK ===");
				Console.WriteLine();
				PrintProductTable();
				var p = AskProductID("\nProduct ID: ");
				if (p == null) return;

				int qty = GetValidatedIntInput("Quantity to deduct: ", 1);
				p.Deduct(qty);
				AddTransaction(p.ProductID, "Deduct", qty, p.Price * qty, $"By {currentUser.Username}");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"\u2705 Deducted! Remaining: {p.StockQuantity}");
				Console.ResetColor();

				Console.Write("\nDeduct from another product? (y = yes / any other key = return to menu): ");
				if (Console.ReadLine()?.Trim().ToLower() != "y") return;
				Console.WriteLine();
			}
		}

		private static void ViewTransactionHistory()
		{
			Console.WriteLine("=== TRANSACTION HISTORY ===");
			Console.WriteLine();
			if (transactions.Count == 0) { Console.WriteLine("No transactions yet."); return; }

			Console.WriteLine($"{"#",-4}  {"Date & Time",-16}  {"Action",-13}  {"Product",-22}  {"Qty",5}  {"Amount",12}  {"Notes",-20}");
			Console.WriteLine(new string('─', 102));
			foreach (var t in transactions.OrderByDescending(t => t.Timestamp))
			{
				string productName = GetProductName(t.ProductID);
				Console.WriteLine($"{t.TransactionID,-4}  {t.Timestamp:yyyy-MM-dd HH:mm}  {t.ActionType,-13}  {productName,-22}  {t.QuantityChanged,5}  {t.Amount,12:C}  {t.Notes,-20}");
			}
		}

		private static void ShowLowStockItems()
		{
			Console.WriteLine("=== LOW STOCK ITEMS ===");
			var low = products.Where(p => p.StockQuantity <= p.LowStockThreshold).ToList();
			if (low.Count == 0)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("✅ All items are well stocked!");
				Console.ResetColor();
				return;
			}
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("⚠️ Low Stock Items:");
			Console.ResetColor();
			low.ForEach(p => Console.WriteLine(p));
		}

		private static void ComputeTotalInventoryValue()
		{
			Console.WriteLine("=== TOTAL INVENTORY VALUE ===\n");

			if (products.Count == 0)
			{
				Console.WriteLine("No products found. Total Value: ₱0.00");
				return;
			}

			// Column widths
			string header = $"{"ID",-4}  {"Product Name",-22}  {"Category",-14}  {"Price",10}  {"Stock",6}  {"Value",12}";
			string divider = new string('─', header.Length);

			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(header);
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(divider);

			foreach (var p in products)
			{
				decimal lineValue = p.Price * p.StockQuantity;
				string catName = GetCategoryName(p.CategoryID);

				// Low stock rows highlighted in yellow
				if (p.StockQuantity <= p.LowStockThreshold)
					Console.ForegroundColor = ConsoleColor.Yellow;
				else
					Console.ForegroundColor = ConsoleColor.White;

				Console.WriteLine($"{p.ProductID,-4}  {p.Name,-22}  {catName,-14}  {p.Price,10:C}  {p.StockQuantity,6}  {lineValue,12:C}");
			}

			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(divider);

			decimal total = products.Sum(p => p.Price * p.StockQuantity);

			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine($"{"Total Products",-46}  {products.Count,6}");
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine($"{"Total Inventory Value",-46}  {total,12:C}");
			Console.ResetColor();
		}
	}
}