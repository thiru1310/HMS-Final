using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webapi_sample.Models;

namespace Webapi_sample
{
    public class EmployeeSecurity
    {

        public static bool Login(string userName, string Password)
        {
            using (EmployeesEntities1 entities1 = new EmployeesEntities1())
            {
                return entities1.UserLogins.Any(user => user.UserName.Equals(userName,
                    StringComparison.OrdinalIgnoreCase) && user.Password == Password);            }
        }
    }
}