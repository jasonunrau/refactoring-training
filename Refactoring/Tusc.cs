using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    public class Tusc
    {
        private static User loggedInUser;

        public static void Start(List<User> users, List<Product> products)
        {
            ShowWelcomeMessage();

            if (logInUser(users))
            {
                DisplayRemainingBalance();
                purchaseGoods(users, products);
            }
            else
            {
                DisplayInvalidUserMessage();
            }

            Exit();
        }

        #region Methods

        private static void ShowWelcomeMessage()
        {
            Console.WriteLine("Welcome to TUSC");
            Console.WriteLine("---------------");
        }

        private static bool logInUser(List<User> users)
        {
            bool isValidUser = false;
            string userName = PromptUser("Enter Username:");

            if ((!string.IsNullOrEmpty(userName)) && ValidUserName(users, userName))
            {
                if (ValidPassword(users, userName))
                {
                    isValidUser = true;
                }
            }
            return isValidUser;
        }

        private static string PromptUser(string text)
        {
            Console.WriteLine();
            Console.WriteLine(text);
            return Console.ReadLine();
        }

        private static bool ValidUserName(List<User> users, string userName)
        {
            bool isUserValid = false;

            for (int i = 0; i < users.Count; i++)
            {
                User user = users[i];
                // Check that name matches
                if (user.Name == userName)
                {
                    isUserValid = true;
                }
            }


            return isUserValid;
        }

        private static bool ValidPassword(List<User> users, string userName)
        {
            bool isPasswordValid = false;
            string userPassword = PromptUser("Enter Password:");



            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Name == userName && users[i].Pwd == userPassword)
                {
                    loggedInUser = users[i];
                    isPasswordValid = true;
                }
            }

            if (isPasswordValid)
            {
                DisplayLoginSuccessMessage();
            }
            else
            {
                DisplayLoginFailureMessage();
            }

            return isPasswordValid;
        }

        private static void DisplayLoginSuccessMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Login successful! Welcome " + loggedInUser.Name + "!");
            Console.ResetColor();
        }

        private static void DisplayLoginFailureMessage()
        {

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("You entered an invalid password.");
            Console.ResetColor();
        }

        private static void DisplayRemainingBalance()
        {
            Console.WriteLine();
            Console.WriteLine("Your balance is " + loggedInUser.Bal.ToString("C"));
        }

        private static void DisplayProductList(List<Product> products)
        {
            Console.WriteLine();
            Console.WriteLine("What would you like to buy?");

            for (int i = 0; i < products.Count; i++)
            {
                Product product = products[i];
                Console.WriteLine(i + 1 + ": " + product.Name + " (" + product.Price.ToString("C") + ")");
            }

            Console.WriteLine(products.Count + 1 + ": Exit");
        }

        private static void purchaseGoods(List<User> users, List<Product> products)
        {

            while (true)
            {
                DisplayProductList(products);
                string userInput = PromptUser("Enter a number:");
                int productItem = Convert.ToInt32(userInput) - 1;

                if (productItem == products.Count)
                {
                    UpdateUsers(users);
                    UpdateProducts(products);
                    return;
                }
                else
                {
                    DisplayProductSelection(products, productItem);
                    int productQuantity = PromptForProductQuantity();

                    if (productQuantity <= 0)
                    {
                        DisplayProductCancelledMessage();
                        break;
                    }
                    else
                    {
                        //Check if they have sufficient funds
                        if (loggedInUser.Bal - products[productItem].Price * productQuantity < 0)
                        {
                            DisplayLowBalanceMessage();
                            continue;
                        }
                        //check if there is enough stock
                        if (products[productItem].Qty > productQuantity && productQuantity > 0)
                        {
                            PurchaseProduct(products, productItem, productQuantity);
                            DisplayProductPurchase(products, productItem, productQuantity);
                            continue;
                        }
                        else
                        {
                            DisplayLowStockMessage(products, productItem);
                            continue;
                        }
                    }
                }
            }


        }

        private static void UpdateUsers(List<User> users)
        {
            // Write out new balance
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(@"Data/Users.json", json);
        }

        private static void UpdateProducts(List<Product> products)
        {
            // Write out new quantities
            string json2 = JsonConvert.SerializeObject(products, Formatting.Indented);
            File.WriteAllText(@"Data/Products.json", json2);
        }

        private static void DisplayProductSelection(List<Product> products, int productItem)
        {
            Console.WriteLine();
            Console.WriteLine("You want to buy: " + products[productItem].Name);
            Console.WriteLine("Your balance is " + loggedInUser.Bal.ToString("C"));
        }

        private static int PromptForProductQuantity()
        {
            string userInput = PromptUser("Enter amount to purchase:");
            return Convert.ToInt32(userInput);
        }

        private static void DisplayLowBalanceMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("You do not have enough money to buy that.");
            Console.ResetColor();
        }

        private static void DisplayLowStockMessage(List<Product> products, int productItem)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("Sorry, " + products[productItem].Name + " is out of stock");
            Console.ResetColor();
        }

        private static void PurchaseProduct(List<Product> products, int productItem, int productQuantity)
        {
            loggedInUser.Bal = loggedInUser.Bal - products[productItem].Price * productQuantity;
            products[productItem].Qty = products[productItem].Qty - productQuantity;
        }

        private static void DisplayProductPurchase(List<Product> products, int productItem, int productQuantity)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("You bought " + productQuantity + " " + products[productItem].Name);
            Console.WriteLine("Your new balance is " + loggedInUser.Bal.ToString("C"));
            Console.ResetColor();
        }

        private static void DisplayProductCancelledMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("Purchase cancelled");
            Console.ResetColor();
        }

        private static void DisplayInvalidUserMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("You entered an invalid user.");
            Console.ResetColor();
        }

        private static void Exit()
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter key to exit");
            Console.ReadLine();
        }

        #endregion

    }
}
                                                      

