using LinqTest01.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqTest01
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestLinq();
            //TestDataAdapter();
            TestDataReader();
        }

        private static void TestLinq()
        {
            LinqDatabaseClassesDataContext dbContext = new LinqDatabaseClassesDataContext();
            dbContext.Log = Console.Out;
            var userResults = from per in dbContext.Persons
                              join plc in dbContext.Places on per.PlaceId equals plc.Id
                              select per;

            List<Person> persons = userResults.ToList();
            persons.First().Age = 55;

            Person newPerson = new Person()
            {
                Id = 3,
                FirstName = "Damir",
                LastName = "Kontrec",
                Age = 28,
                PlaceId = 2
            };
            dbContext.Persons.InsertOnSubmit(newPerson);

            dbContext.Persons.DeleteOnSubmit(persons.Single(p => p.Id == 2));

            //foreach (Person row in persons)
            //{
            //    row.Age = 55;
            //}

            //persons.Single(x => x.Id == persons.First().Id).Age = 55;

            //dbContext.Persons.InsertAllOnSubmit(userResults);

            dbContext.SubmitChanges();
        }

        private static void TestDataAdapter()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LinqDatabase"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            using (SqlDataAdapter da = new SqlDataAdapter())
            using (DataSet ds = new DataSet())
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM Person INNER JOIN Place ON Person.PlaceId = Place.Id";

                da.SelectCommand = cmd;

                conn.Open();
                da.Fill(ds);
            }
        }

        private static void TestDataReader()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LinqDatabase"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM Person INNER JOIN Place ON Person.PlaceId = Place.Id";

                conn.Open();

                List<Person> persons = new List<Person>();
                SqlDataReader reader = cmd.ExecuteReader();
                int numColumns = reader.FieldCount;

                while (reader.Read())
                {
                    object[] personData = new object[numColumns];
                    reader.GetValues(personData);
                    Person person = new Person
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Age = reader.GetInt16(3),
                        PlaceId = reader.GetInt32(4),
                        Place = new Place
                        {
                            Id = reader.GetInt32(5),
                            Name = reader.GetString(6),
                            ZipCode = reader.GetInt32(7)
                        }
                    };

                    persons.Add(person);
                }

                int x = 0;
            }
        }
    }
}
