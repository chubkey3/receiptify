using webapi.Models;

namespace webapi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ReceiptifyContext context)
        {

            if (context.Users.Any()
                && context.Expenses.Any()
                && context.Receipts.Any()
                && context.Suppliers.Any())
            {
                return;   // DB has been seeded
            }

            // seed users
            var userJamal69 = new User { Uid = "q0adLM1pcpdpOOcIGOjXf5P40u23", Username = "Jamal69", Email = "jamal69@gmail.com" };
            var userJamal = new User { Uid = "i01Fwv413PMylKa4LrKNnvBiX8m2", Username = "Jamal", Email = "jamal@gmail.com" };
            var userLink = new User { Uid = "Ok6yVBpq1WcgMac4ycOGtdmmZtZ2", Username = "Link", Email = "linkspirt@gmail.com" };

            // seed suppliers
            var supplierWalmart = new Supplier { SupplierId = 1, SupplierName = "Walmart" };
            var supplierShoppers = new Supplier { SupplierId = 2, SupplierName = "Shoppers Drug Mart" };
            var suppliersStongs = new Supplier { SupplierId = 3, SupplierName = "Stong's Market" };

            var receipt1 = new Receipt { ReceiptId = 1, ReceiptUrl = "https://expressexpense.com/blog/wp-content/uploads/2020/05/generated-walmart-receipt-430x1024.jpg", UploadDate = new DateTime(2024, 6, 13, 15, 30, 0), UploadedBy = "q0adLM1pcpdpOOcIGOjXf5P40u23" };
            var receipt2 = new Receipt { ReceiptId = 2, ReceiptUrl = "https://a.dam-img.rfdcontent.com/cms/006/787/260/6787260_original.jpg", UploadDate = new DateTime(2022, 6, 10, 9, 10, 0), UploadedBy = "i01Fwv413PMylKa4LrKNnvBiX8m2" };
            var receipt3 = new Receipt { ReceiptId = 3, ReceiptUrl = "https://miro.medium.com/v2/resize:fit:400/1*MLRlL9W69PMWAcTF-rV36Q.jpeg", UploadDate = new DateTime(2025, 6, 15, 10, 30, 0) };

            var expense1 = new Expense { ExpenseId = 1, TotalAmount = new decimal(24.95), ExpenseDate = new DateTime(2024, 6, 13, 15, 30, 0), Uid = "q0adLM1pcpdpOOcIGOjXf5P40u23", ReceiptId = 1, SupplierId = 1 };
            var expense2 = new Expense { ExpenseId = 2, TotalAmount = new decimal(49.95), ExpenseDate = new DateTime(2024, 6, 10, 9, 10, 0), Uid = "i01Fwv413PMylKa4LrKNnvBiX8m2", ReceiptId = 2, SupplierId = 2 };
            var expense3 = new Expense { ExpenseId = 3, TotalAmount = new decimal(10.95), ExpenseDate = new DateTime(2024, 6, 15, 10, 30, 0), Uid = "Ok6yVBpq1WcgMac4ycOGtdmmZtZ2", ReceiptId = 3, SupplierId = 3 };

            
            context.Users.Add(userJamal69);
            context.Users.Add(userJamal);
            context.Users.Add(userLink);

            context.Suppliers.Add(supplierWalmart);
            context.Suppliers.Add(supplierShoppers);
            context.Suppliers.Add(suppliersStongs);

            context.Receipts.Add(receipt1);
            context.Receipts.Add(receipt2);
            context.Receipts.Add(receipt3);

            context.Expenses.Add(expense1);
            context.Expenses.Add(expense2);
            context.Expenses.Add(expense3);

            context.SaveChanges();
        }
    }
}