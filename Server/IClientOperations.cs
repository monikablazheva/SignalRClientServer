using BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface IClientOperations
    {
        Task ConnectedClient(string information);

        Task CreatedUser(string jsonData);

        Task CreatedFood(string jsonData);

        Task ReadUser(string jsonData);

        Task ReadFood(string jsonData);

        Task ReadAllUsers(string jsonData);
        
        Task ReadAllFoods(string jsonData);

        Task UpdatedUser(string information);

        Task UpdatedFood(string information);

        Task DeletedUser(string information);

        Task DeletedFood(string information);

        Task CreatedRoom(string information);

        Task JoinedRoom(string information);

        Task SentFoodToRoom(string jsonData);

        Task OperationFailed(string information);
    }
}
