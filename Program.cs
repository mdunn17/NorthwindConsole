using System;
using NLog.Web;
using System.IO;
using System.Linq;
using NorthwindConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NorthwindConsole
{
    class Program
    {
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");

            try
            {
                string choice;
                do
                {
                    Console.WriteLine("Please select an option:");
                    Console.WriteLine("1) Display Categories");
                    Console.WriteLine("2) Add Category");
                    Console.WriteLine("3) Display Category and related products");
                    Console.WriteLine("4) Display all Categories and their related products");
                    Console.WriteLine("5) Display Products");
                    Console.WriteLine("6) Display a Specific Product");
                    Console.WriteLine("7) Add a Product");
                    Console.WriteLine("8) Delete a Product");
                    Console.WriteLine("9) Edit a Product");
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    //Console.Clear();
                    logger.Info($"Option {choice} selected");
                    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
                    if (choice == "1")
                    {
                        var db = new NWConsole_96_medContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{query.Count()} records returned");
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------

                    else if (choice == "2")
                    {
                        Categories category = new Categories();
                        Console.WriteLine("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Enter the Category Description:");
                        category.Description = Console.ReadLine();
                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            logger.Info("Validation passed");
                            var db = new NWConsole_96_medContext();
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                db.AddCategory(category);
                                logger.Info("Category added - {name}", category.CategoryName);
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
                    else if (choice == "3")
                    {
                        var db = new NWConsole_96_medContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        int id = int.Parse(Console.ReadLine());
                        //Console.Clear();
                        logger.Info($"CategoryId {id} selected");
                        Categories category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        foreach (Products p in category.Products)
                        {
                            Console.WriteLine(p.ProductName);
                        }
                    }
                    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
                    else if (choice == "4")
                    {
                        var db = new NWConsole_96_medContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName}");
                            foreach (Products p in item.Products)
                            {
                                Console.WriteLine($"\t{p.ProductName}");
                            }
                        }
                    }
                    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
                    else if (choice == "5")
                    {
                        Console.WriteLine("Please select an option:");
                        Console.WriteLine("1) Display All Products");
                        Console.WriteLine("2) Display Active Products");
                        Console.WriteLine("3) Display Discontinued Products");
                        string productSelect = Console.ReadLine();
                        logger.Info($"Option {productSelect} selected");

                        if (productSelect == "1")
                        {
                            var db = new NWConsole_96_medContext();
                            var queryActive = db.Products.Where(p => p.Discontinued.Equals(false)).OrderBy(p => p.ProductName);
                            var queryDiscontinued = db.Products.Where(p => p.Discontinued.Equals(true)).OrderBy(p => p.ProductName);

                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.WriteLine($"{queryActive.Count()} active records returned");
                            Console.WriteLine($"{queryDiscontinued.Count()} discontinued records returned");
                            Console.ForegroundColor = ConsoleColor.Green;
                            foreach (var item in queryActive)
                            {
                                Console.WriteLine($"{item.ProductName}");
                            }
                            Console.ForegroundColor = ConsoleColor.Red;
                            foreach (var item in queryDiscontinued)
                            {
                                Console.WriteLine($"{item.ProductName}");
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (productSelect == "2")
                        {
                            var db = new NWConsole_96_medContext();
                            var query = db.Products.Where(p => p.Discontinued.Equals(false)).OrderBy(p => p.ProductName);

                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.WriteLine($"{query.Count()} records returned");
                            Console.ForegroundColor = ConsoleColor.Green;
                            foreach (var item in query)
                            {
                                Console.WriteLine($"{item.ProductName}");
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (productSelect == "3")
                        {
                            var db = new NWConsole_96_medContext();
                            var query = db.Products.Where(p => p.Discontinued.Equals(true)).OrderBy(p => p.ProductName);

                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.WriteLine($"{query.Count()} records returned");
                            Console.ForegroundColor = ConsoleColor.Green;
                            foreach (var item in query)
                            {
                                Console.WriteLine($"{item.ProductName}");
                            }
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
                    else if (choice == "6")
                    {
                        Console.WriteLine("Enter the Product Name you wish to search for:");
                        string nameSearch = Console.ReadLine();
                        
                        var db = new NWConsole_96_medContext();
                        var query = db.Products.Where(p => p.ProductName.Contains(nameSearch)).OrderBy(p => p.ProductName);

                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine($"There is {query.Count()} product(s) that match your search.");
                        Console.ForegroundColor = ConsoleColor.Green;
                        foreach (var item in query)
                        {
                            Console.WriteLine("--------------------------------");
                            Console.WriteLine($"Product: {item.ProductId} - {item.ProductName}");
                            Console.WriteLine($"Supplier ID: {item.SupplierId}");
                            Console.WriteLine($"Category ID: {item.CategoryId}");
                            Console.WriteLine($"Quantity per Unit: {item.QuantityPerUnit}");
                            Console.WriteLine($"Unit Price: {item.UnitPrice}");
                            Console.WriteLine($"Units in Stock: {item.UnitsInStock}");
                            Console.WriteLine($"Units on Order: {item.UnitsOnOrder}");
                            Console.WriteLine($"Redorder Level: {item.ReorderLevel}");
                            Console.WriteLine($"Discontinued(?): {item.Discontinued}");
                            Console.WriteLine("--------------------------------");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    
                    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
                    
                    else if (choice == "7")
                    {
                        Products product = new Products();
                        Console.WriteLine("Enter Product Name:");
                        product.ProductName = Console.ReadLine();
                        
                        Console.WriteLine("Enter the Product Quantity per Unit:");
                        product.QuantityPerUnit = Console.ReadLine();
                        Console.WriteLine("Enter the Product Unit Price:");
                        product.UnitPrice = decimal.Parse(Console.ReadLine());
                        Console.WriteLine("Enter the Product Units in Stock:");
                        product.UnitsInStock = short.Parse(Console.ReadLine());
                        Console.WriteLine("Enter the Product Units on Order:");
                        product.UnitsOnOrder = short.Parse(Console.ReadLine());
                        Console.WriteLine("Enter the Product Reorder Level:");
                        product.ReorderLevel = short.Parse(Console.ReadLine());
                        product.Discontinued = false;
                        
                        //saves category to db
                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(product, context, results, true);
                        if (isValid)
                        {
                            logger.Info("Validation passed");
                            var db = new NWConsole_96_medContext();
                            // check for unique name
                            if (db.Products.Any(c => c.ProductName == product.ProductName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                db.AddProduct(product);
                                logger.Info("Product Added - {name}", product.ProductName);
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    }
                    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
                    else if (choice == "8")
                    {
                        var db = new NWConsole_96_medContext();
                        Console.WriteLine("Choose the Product to delete:");
                        var product = GetProduct(db);
                        if (product != null)
                        {
                            // delete product
                            db.DeleteProduct(product);
                            logger.Info($"Product (id: {product.ProductId}) deleted");
                        }
                        
                    }
                    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
                    else if (choice == "9")
                    {
                        var db = new NWConsole_96_medContext();
                        
                    }
                    Console.WriteLine();

                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }

        public static Products GetProduct(NWConsole_96_medContext db)
        {
            // display all products
            var products = db.Products.OrderBy(b => b.ProductId);
            foreach (Products p in products)
            {
                Console.WriteLine($"{p.ProductId}: {p.ProductName}");
            }
            if (int.TryParse(Console.ReadLine(), out int ProductId))
            {
                Products product = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
                if (product != null)
                {
                    return product;
                }
            }
            logger.Error("Invalid Product Id");
            return null;
        }

        public static Products InputProduct(NWConsole_96_medContext db)
        {
            Products product = new Products();
            Console.WriteLine("Enter the Product name");
            product.ProductName = Console.ReadLine();

            ValidationContext context = new ValidationContext(product, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(product, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Products.Any(b => b.ProductName == product.ProductName))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Product name exists", new string[] { "Name" }));
                }
                else
                {
                    logger.Info("Validation passed");
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
                return null;
            }

            return product;
        }
    }
}