using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessLayer
{
    public class User
    {
        public string Id { get; private set; }

        public string Name { get; set; }

        public int Age { get; set; }

        // Prevent loops if you are using Net Core Json Serializer!
        //[JsonIgnore]
        public IList<Food> Foods { get; set; }

        private User()
        {

        }

        public User(string id, int age, string name)
        {
            this.Id = id;
            this.Age = age;
            this.Name = name;
            this.Foods = new List<Food>();
        }

        public override string ToString()
        {
            return string.Format($"| ID:{Id} | Name:{Name} | Age:{Age} |");
        }

    }
}
