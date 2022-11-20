using BusinessLayer;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        // Microsoft.AspNetCore.SignalR.Client 5.0.17
        // Microsoft.AspNetCore.SignalR.Protocols.NewtonsoftJson 5.0.17

        static HubConnection connection;
        static AutoResetEvent waitHandle = new AutoResetEvent(false);
        
        static bool communicationIsActive = true;
        static User returnedUser;
        static Food returnedFood;

        static async Task Main(string[] args)
        {
            try
            {
                connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:9999/test")
                .Build();

                AddEvents();

                await connection.StartAsync();

                // The server sends a greeting message!
                waitHandle.WaitOne(1000);

                do
                {
                    ShowMenu();
                    await ClientOperation();
                } while (communicationIsActive);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await connection.StopAsync();
                Console.WriteLine("Press any key to stop the application!");
                Console.ReadKey(true);
            }
        }

        #region Methods

        private static void ShowMenu()
        {
            Console.WriteLine(Environment.NewLine + "____________________________");
            Console.WriteLine("Choose one of the options:");
            Console.WriteLine("1) Create User");
            Console.WriteLine("2) Create Food");
            Console.WriteLine("3) View Users");
            Console.WriteLine("4) View Foods");
            Console.WriteLine("5) Update User");
            Console.WriteLine("6) Update Food");
            Console.WriteLine("7) Delete User");
            Console.WriteLine("8) Delete Food");
            Console.WriteLine("9) Exit");
            Console.WriteLine();
        }

        private static async Task ClientOperation()
        {
            try
            {
                Console.Write("Your choice: ");
                int option = int.Parse(Console.ReadLine());

                switch (option)
                {
                    case 1: await CreateUser(); break;
                    case 2: await CreateFood(); break;
                    case 3: await ViewUsers(); break;
                    case 4: await ViewFoods(); break;
                    //case 5: await UpdateUser(); break;
                    case 6: await UpdateFood(); break;
                    //case 7: await DeleteUser(); break;
                    case 8: await DeleteFood(); break;
                    case 9: communicationIsActive = false; return;
                    default:
                        Console.WriteLine("Invalid option => [1:9]!");
                        break;
                }

                // Wait the server for response before showing the menu again!
                // We use Set() in our callbacks to free this thread!
                bool getSignal = waitHandle.WaitOne(3000);
                Console.WriteLine("Get signal from callback? {0}", getSignal);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine();
            }
        }

        private static async Task CreateUser()
        {
            Console.Write("Age: ");
            int age = Convert.ToInt32(Console.ReadLine());

            Console.Write("Name: ");
            string name = Console.ReadLine();

            // If you send User, its construtor will be called after deserealization and will get different GUID!
            await connection.InvokeAsync("CreateUser", age, name);
        }

        private static async Task CreateFood()
        {
            string name;
            int quantity;
            decimal price;
            int userIndex;

            try
            {
                Console.Write("Name: ");
                name = Console.ReadLine();

                Console.Write("Quantity: ");
                quantity = int.Parse(Console.ReadLine());

                Console.Write("Price: ");
                price = decimal.Parse(Console.ReadLine());

                // Should be the authenticated user!
                Console.Write("Index of the User from the Db: ");
                userIndex = int.Parse(Console.ReadLine());
                await connection.InvokeAsync("ReadUser", userIndex);

                waitHandle.WaitOne(1000);

                string jsonData = JsonConvert.SerializeObject(returnedUser);

                await connection.InvokeAsync("CreateFood", name, quantity, price, jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task ViewUsers()
        {
            await connection.InvokeAsync("ReadAllUsers");
        }

        private static async Task ViewFoods()
        {
            await connection.InvokeAsync("ReadAllFoods");
        }

        private static async Task UpdateFood()
        {
            int foodIndex;
            string name;
            int quantity;
            decimal price;
            int userIndex;


            try
            {
                Console.Write("Index of the Food from the Db: ");
                foodIndex = int.Parse(Console.ReadLine());
                await connection.InvokeAsync("ReadFood", foodIndex);
                waitHandle.WaitOne(1000);
                string jsonDataFood = JsonConvert.SerializeObject(returnedFood);

                if(returnedFood != null)
                {
                    Console.Write("Name: ");
                    name = Console.ReadLine();

                    Console.Write("Quantity: ");
                    quantity = int.Parse(Console.ReadLine());

                    Console.Write("Price: ");
                    price = decimal.Parse(Console.ReadLine());

                    Console.Write("Index of the User from the Db: ");
                    userIndex = int.Parse(Console.ReadLine());
                    await connection.InvokeAsync("ReadUser", userIndex);
                    waitHandle.WaitOne(1000);
                    string jsonDataUser = JsonConvert.SerializeObject(returnedUser);

                    returnedFood.Name = name;
                    returnedFood.Quantity = quantity;
                    returnedFood.Price = price;
                    returnedFood.User = returnedUser;

                    jsonDataFood = JsonConvert.SerializeObject(returnedFood);
                    await connection.InvokeAsync("UpdateFood", jsonDataFood);
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static async Task DeleteFood()
        {

        }

        #endregion

        #region Events

        private static void AddEvents()
        {
            //HubConnectionExtensions.On<string>(connection, "ConnectedClient", OnConnectedClient);
            connection.On<string>("ConnectedClient", OnConnectedClient);
            connection.On<string>("CreatedUser", OnCreatedUser);
            connection.On<string>("CreatedFood", OnCreatedFood);
            connection.On<string>("ReadUser", OnReadUser);
            connection.On<string>("ReadFood", OnReadFood);
            connection.On<string>("ReadAllUsers", OnReadAllUsers);
            connection.On<string>("ReadAllFoods", OnReadAllFoods);
            connection.On<string>("UpdatedUser", OnUpdatedUser);
            connection.On<string>("UpdatedFood", OnUpdatedFood);
            connection.On<string>("DeletedUser", OnDeletedUser);
            connection.On<string>("DeletedFood", OnDeletedFood);
            connection.On<string>("CreatedRoom", OnCreatedRoom);
            connection.On<string>("JoinedRoom", OnJoinedRoom);
            connection.On<string>("SentFoodToRoom", OnSentFoodToRoom);
            connection.On<string>("OperationFailed", OnOperationFailed);
        }

        private static async Task OnConnectedClient(string information)
        {
            Console.WriteLine(information);
            waitHandle.Set();

            //await Task.Run(async () => 
            //{
            //    Console.WriteLine(Food);
            //});
        }

        private static async Task OnCreatedUser(string jsonData)
        {
            User createdUser = JsonConvert.DeserializeObject<User>(jsonData);
            Console.WriteLine(createdUser);
            waitHandle.Set();
        }

        private static async Task OnCreatedFood(string jsonData)
        {
            Food createdFood = JsonConvert.DeserializeObject<Food>(jsonData);
            Console.WriteLine(createdFood);
            waitHandle.Set();
        }

        private static async Task OnReadUser(string jsonData)
        {
            returnedUser = JsonConvert.DeserializeObject<User>(jsonData);
            Console.WriteLine("User sent from server:");
            Console.WriteLine(returnedUser);
            waitHandle.Set();
        }

        private static async Task OnReadFood(string jsonData)
        {
            returnedFood = JsonConvert.DeserializeObject<Food>(jsonData);
            Console.WriteLine("Food sent from server:");
            Console.WriteLine(returnedFood);
            
            waitHandle.Set();
        }

        private static async Task OnReadAllUsers(string jsonData)
        {
            List<User> users = JsonConvert.DeserializeObject<List<User>>(jsonData);
            Console.WriteLine("Users information:");
            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine(users[i]);
                Console.WriteLine("________________________");
            }
            waitHandle.Set();
        }

        private static async Task OnReadAllFoods(string jsonData)
        {
            List<Food> foods = JsonConvert.DeserializeObject<List<Food>>(jsonData);
            Console.WriteLine("Foods information:");
            for (int i = 0; i < foods.Count; i++)
            {
                Console.WriteLine(foods[i]);
                Console.WriteLine("________________________");
            }
            waitHandle.Set();
        }

        private static async Task OnUpdatedUser(string information)
        {
            Console.WriteLine(information);
            waitHandle.Set();
        }

        private static async Task OnUpdatedFood(string jsonData)
        {
            Food updatedFood = JsonConvert.DeserializeObject<Food>(jsonData);
            Console.WriteLine(updatedFood);
            waitHandle.Set();
        }

        private static async Task OnDeletedUser(string information)
        {
            Console.WriteLine(information);
            waitHandle.Set();
        }

        private static async Task OnDeletedFood(string information)
        {
            Console.WriteLine(information);
            waitHandle.Set();
        }

        private static async Task OnCreatedRoom(string information)
        {
            Console.WriteLine(information);
            waitHandle.Set();
        }

        private static async Task OnJoinedRoom(string information)
        {
            Console.WriteLine(information);
            waitHandle.Set();
        }

        private static async Task OnSentFoodToRoom(string Food)
        {
            Console.WriteLine("Received food: {0}", Food);
            waitHandle.Set();
        }

        private static async Task OnOperationFailed(string information)
        {
            Console.WriteLine(information);
            waitHandle.Set();
        }
        #endregion

    }
}
