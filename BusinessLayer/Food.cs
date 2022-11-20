using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Food
    {
        public string Id { get; private set; }

        public string Name { get; set; }

        public int Quantity { get; set; }
        
        public decimal Price { get; set; }

        public User User { get; set; }

        private Food()
        {

        }

        public Food(string id, string name, int quantity, decimal price, User user)
        {
            this.Id = id;
            this.Name = name;
            this.Quantity = quantity;
            this.Price = price;
            this.User = user;
        }
        public override string ToString()
        {
            return $"| Id:{Id} | Name:{Name} | Quantity:{Quantity} | Price:{Price} | UserInfo:{User} |";
        }
    }
}
