using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace Webapi_sample.Controllers
{
    public class ValuesController : ApiController
    {

        // GET api/values
        SqlConnection con =new SqlConnection("Data Source=tcp:sc2.database.windows.net,1433;Initial Catalog=Employees;User ID=surclr;Password=Test@1234");
        public string Get()
        {
            
            SqlDataAdapter adapter = new SqlDataAdapter("select * from EmployeeDetails", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            if(dt.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(dt);
            }
            else
            {
                return "No data found";
            }
        }

        // GET api/values/5
        public string Get(int id)
        {
            SqlDataAdapter adapter = new SqlDataAdapter("select * from EmployeeDetails where id="+id+"", con);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(dt);
            }
            else
            {
                return "No data found";
            }
        }

        // POST api/values
        public string Post(string FirstName, string LastName, string Gender, float Salary)
        {
           SqlCommand cmd = new SqlCommand(" Insert into EmployeeDetails Values('"+ FirstName + "','" + LastName + "','" + Gender + "','" + Salary + "')", con);
            con.Open();

           int i= cmd.ExecuteNonQuery();
            con.Close();
            if(i == 1)
            {
                return "Record Inserted Successfully "+ FirstName +", "+LastName+", "+ Gender+", "+ Salary;
            }
            else
            {
                return "Try again,no data inserted ";
            }
        }

        // PUT api/values/5
        public string Put(int id, string ColumnName,string ValueForColumn)
        {
            
            SqlCommand cmd = new SqlCommand("update EmployeeDetails set "+ColumnName+"='" + ValueForColumn + "' where id = "+id+"", con);
            con.Open();

            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i == 1)
            {
                return "Record updated Successfully id= "+id+"  "+ ColumnName + " = " + ValueForColumn;
            }
            else
            {
                return "Try again,no data inserted ";
            }

        }

        // DELETE api/values/5
        public string Delete(int id)
        {
            SqlCommand cmd = new SqlCommand("Delete EmployeeDetails where id = " + id + "", con);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            if (i == 1)
            {
                return "Record Deleted Successfully id=" + id;
            }
            else
            {
                return "Try again,no data inserted ";
            }

        }
    }
}
