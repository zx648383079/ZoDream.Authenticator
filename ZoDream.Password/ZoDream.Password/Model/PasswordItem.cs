using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Password.Model
{
    public class PasswordItem
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Number { get; set; }

        public string Password { get; set; }

        public string Mark { get; set; }

        public PasswordItem()
        {
            
        }

        public PasswordItem(string url, string name, string email, string number, string password, string mark)
        {
            Name = name;
            Url = url;
            Email = email;
            Number = number;
            Password = password;
            Mark = mark;
        }

        public PasswordItem(IDataRecord reader)
        {
            Id = reader.GetInt16(0);
            Url = reader[1].ToString();
            Name = reader[2].ToString();
            Email = reader[3].ToString();
            Number = reader[4].ToString();
            Password = reader[5].ToString();
            Mark = reader[6].ToString();
        }
    }
}
