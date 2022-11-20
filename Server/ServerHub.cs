using BusinessLayer;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ServerHub : Hub<IClientOperations>
    {
        #region Properties and Constructors

        public static List<string> ConnectionsIds { get; set; }

        // Simulating DB:
        public static List<Room> Rooms { get; set; }
        public static List<User> Users { get; set; }
        public static List<Food> Foods { get; set; }

        static ServerHub()
        {
            ConnectionsIds = new List<string>();
            Rooms = new List<Room>();
            Users = new List<User>();
            Foods = new List<Food>();
        }

        #endregion

        #region Events

        public override async Task OnConnectedAsync()
        {
            string information = string.Empty;

            try
            {
                PrintInfo(Context.ConnectionId, "OnConnectedAsync()!");
                
                if (!ConnectionsIds.Contains(Context.ConnectionId))
                {
                    information = "Server greets you!";
                    ConnectionsIds.Add(Context.ConnectionId);
                    await Clients.Caller.ConnectedClient(information);
                }
            }
            catch (Exception ex)
            {
                information = ex.Message;
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Better use await Task.Run() if we have thousands of rooms!
            for (int i = 0; i < Rooms.Count; i++)
            {
                string foundConnection = Rooms[i].Connections.FirstOrDefault(c => c == Context.ConnectionId);

                if (foundConnection != null)
                {
                    Rooms[i].Connections.Remove(foundConnection);
                }
            }

            ConnectionsIds.Remove(Context.ConnectionId);
            PrintInfo(Context.ConnectionId, string.Format("Disconnected! {0} clients remained!", ConnectionsIds.Count));
            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        #region CRUD

        public async Task CreateUser(int age, string name)
        {
            PrintInfo(Context.ConnectionId, "CreateUser()!");

            string information = string.Empty;

            try
            {
                // Add user to the db
                User user = new User(Guid.NewGuid().ToString(), age, name);
                Users.Add(user);

                // Send message to the client
                information = "User added successfully!";
                string jsonData = JsonConvert.SerializeObject(user);
                await Clients.Caller.CreatedUser(jsonData);
            }
            catch (Exception ex)
            {
                information = ex.Message;
                await Clients.Caller.OperationFailed(information);
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }
        }

        public async Task CreateFood(string name, int quantity, decimal price, string jsonUserSender)
        {
            PrintInfo(Context.ConnectionId, "CreateFood()!");

            string information = string.Empty;

            try
            {
                User sender = JsonConvert.DeserializeObject<User>(jsonUserSender);
                Food food = new Food(Guid.NewGuid().ToString(), name, quantity, price, sender);
                Foods.Add(food);
                information = "Food added successfully!";
                string jsonData = JsonConvert.SerializeObject(food);
                await Clients.Caller.CreatedFood(jsonData);
            }
            catch (Exception ex)
            {
                information = ex.Message;
                await Clients.Caller.OperationFailed(information);
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }
        }

        public async Task ReadUser(int indexOfArray)
        {
            string information = string.Empty;

            try
            {
                if (Users.Count <= indexOfArray)
                {
                    information = string.Format("Index should be between 0 and {1}", Users.Count - 1);
                    await Clients.Caller.OperationFailed(information);
                }
                else
                {
                    information = string.Format("ReadUser() => {0}", Users[indexOfArray]);
                    string jsonData = JsonConvert.SerializeObject(Users[indexOfArray]);
                    await Clients.Caller.ReadUser(jsonData);
                }
            }
            catch (Exception ex)
            {
                information = ex.Message;
                await Clients.Caller.OperationFailed(information);
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }           
        }

        public async Task ReadFood(int indexOfArray)
        {
            string information = string.Empty;

            try
            {
                if (Foods.Count <= indexOfArray)
                {
                    information = string.Format("Index should be between 0 and {1}", Users.Count - 1);
                    await Clients.Caller.OperationFailed(information);
                }
                else
                {
                    information = string.Format("ReadFood() => {0}", Foods[indexOfArray]);
                    string jsonData = JsonConvert.SerializeObject(Foods[indexOfArray]);
                    await Clients.Caller.ReadFood(jsonData);
                }
            }
            catch (Exception ex)
            {
                information = ex.Message;
                await Clients.Caller.OperationFailed(information);
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }
        }

        public async Task ReadAllUsers()
        {
            PrintInfo(Context.ConnectionId, "ReadAllUsers()!");

            string information = string.Empty;

            try
            {
                information = "ReadAllUsers() completed successfully!";
                string jsonData = JsonConvert.SerializeObject(Users);
                await Clients.Caller.ReadAllUsers(jsonData);
            }
            catch (Exception ex)
            {
                information = ex.Message;
                await Clients.Caller.OperationFailed(information);
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }
        }

        public async Task ReadAllFoods()
        {
            PrintInfo(Context.ConnectionId, "ReadAllFoods()!");

            string information = string.Empty;

            try
            {
                information = "ReadAllFoods() completed successfully!";
                string jsonData = JsonConvert.SerializeObject(Foods);
                await Clients.Caller.ReadAllFoods(jsonData);
            }
            catch (Exception ex)
            {
                information = ex.Message;
                await Clients.Caller.OperationFailed(information);
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }
        }

        public async Task UpdateUser(string jsonUser)
        {
            PrintInfo(Context.ConnectionId, "UpdateUser()!");

            string information = string.Empty;

            try
            {
                User user = JsonConvert.DeserializeObject<User>(jsonUser);
                User userFromDb = Users.Single(u => u.Id == user.Id);

                userFromDb.Name = user.Name;
                userFromDb.Age = user.Age;

                information = "User updated successfully!";
                await Clients.Caller.UpdatedUser(information);
            }
            catch (Exception ex)
            {
                information = ex.Message;
                await Clients.Caller.OperationFailed(information);
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }
        }

        public async Task UpdateFood(string jsonFood)
        {
            PrintInfo(Context.ConnectionId, "UpdateFood()!");

            string information = string.Empty;

            try
            {
                Food food = JsonConvert.DeserializeObject<Food>(jsonFood);
                Food foodFromDb = Foods.Single(m => m.Id == food.Id);

                foodFromDb.Name = food.Name;
                foodFromDb.Quantity = food.Quantity;
                foodFromDb.Price = food.Price;
                foodFromDb.User = food.User;

                /*if (food.Receiver != null)
                {
                    foodFromDb.Receiver = food.Receiver;
                }*/

                information = "Food updated successfully!";
                await Clients.Caller.UpdatedFood(information);
            }
            catch (Exception ex)
            {
                information = ex.Message;
                await Clients.Caller.OperationFailed(information);
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }
        }

        public async Task DeleteUser(int indexOfArray)
        {
            PrintInfo(Context.ConnectionId, "DeleteUser()!");

            string information = string.Empty;

            try
            {
                if (Users.Count <= indexOfArray)
                {
                    information = string.Format("Index should be between 0 and {1}", Users.Count - 1);
                    await Clients.Caller.OperationFailed(information);
                }
                else
                {
                    information = "User deleted successfully!";
                    Users.Remove(Users[indexOfArray]);
                    await Clients.Caller.DeletedUser(information);
                }
            }
            catch (Exception ex)
            {
                information = ex.Message;
                await Clients.Caller.OperationFailed(information);
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }
        }

        public async Task DeleteFood(int indexOfArray)
        {
            PrintInfo(Context.ConnectionId, "DeleteFood()!");

            string information = string.Empty;

            try
            {
                if (Foods.Count <= indexOfArray)
                {
                    information = string.Format("Index should be between 0 and {1}", Users.Count - 1);
                    await Clients.Caller.OperationFailed(information);
                }
                else
                {
                    information = "Food deleted successfully!";
                    Foods.Remove(Foods[indexOfArray]);
                    await Clients.Caller.DeletedFood(information);
                }
            }
            catch (Exception ex)
            {
                information = ex.Message;
                await Clients.Caller.OperationFailed(information);
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }
        }

        #endregion

        #region Groups

        public async Task CreateRoom(string name)
        {
            PrintInfo(Context.ConnectionId, "CreateRoom()!");

            string information = string.Empty;

            try
            {
                if (Rooms.FirstOrDefault(r => r.Name == name) != null)
                {
                    information = "Room exists! Choose another name!";
                    await Clients.Caller.OperationFailed(information);
                }
                else
                {
                    Rooms.Add(new Room(Guid.NewGuid().ToString(), name));
                    information = "Room created successfully!";
                    await Clients.Caller.CreatedRoom(information);

                    await JoinRoom(name);
                }
            }
            catch (Exception ex)
            {
                information = ex.Message;
                await Clients.Caller.OperationFailed(information);
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }
        }

        public async Task JoinRoom(string name)
        {
            string information = string.Empty;

            try
            {
                Room room = Rooms.FirstOrDefault(r => r.Name == name);

                if (room != null)
                {
                    string userConnection = ConnectionsIds.Single(c => c == Context.ConnectionId);
                    room.Connections.Add(userConnection);
                    information = $"Connection {userConnection} added successfully to {name} room! " +
                        $"{room.Connections.Count} people in the room!";
                    await Clients.Caller.JoinedRoom(information);
                }
                else
                {
                    information = "There is no room with that name!";
                    await Clients.Caller.OperationFailed(information);
                }
            }
            catch (Exception ex)
            {
                information = ex.Message;
                await Clients.Caller.OperationFailed(information);
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }
        }

        public async Task SendFoodToRoom(string room, string Food)
        {
            string information = Food;

            try
            {
                Room roomFromDb = Rooms.FirstOrDefault(r => r.Name == room);

                if (roomFromDb == null)
                {
                    information = "There is no room with that name!";
                    await Clients.Caller.OperationFailed(information);
                }
                else
                {
                    for (int i = 0; i < roomFromDb.Connections.Count; i++)
                    {
                        // Don't send Food to yourself!
                        if (roomFromDb.Connections[i] != Context.ConnectionId)
                        {
                            await Clients.Client(roomFromDb.Connections[i]).SentFoodToRoom(Food);
                        }
                        
                    }
                    PrintInfo(Context.ConnectionId, $"sent {roomFromDb.Connections.Count - 1} Foods!");
                }
            }
            catch (Exception ex)
            {
                information = ex.Message;
                await Clients.Caller.OperationFailed(information);
            }
            finally
            {
                PrintInfo(Context.ConnectionId, information);
            }
        }

        #endregion

        #region Helper Methods

        private void PrintInfo(string clientId, string information)
        {
            Console.WriteLine($"Client: {clientId} => {information}");
        }

        #endregion

    }
}
