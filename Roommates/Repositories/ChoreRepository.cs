using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;
using Roommates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roommates.Repositories;

public class ChoreRepository : BaseRepository
{
    public ChoreRepository(string connectionString) : base(connectionString) { }

    public List<Chore> GetAll()
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, Name FROM Chore";

                SqlDataReader reader = cmd.ExecuteReader();

                List<Chore> chores = new List<Chore>(); 

                while(reader.Read())
                {
                    int idColumnPosition = reader.GetOrdinal("id");

                    int idValue = reader.GetInt32(idColumnPosition);

                    int nameColumnPosition = reader.GetOrdinal("Name");
                    string nameValue = reader.GetString(nameColumnPosition);

                    Chore chore = new Chore()
                    {
                        Id = idValue,
                        Name = nameValue,
                    };

                    chores.Add(chore);
                }

                reader.Close();

                return chores;
            }
        }
    }

    public Chore GetById(int id)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                Chore chore = null;

                if (reader.Read())
                {
                    chore = new Chore
                    {
                        Id = id,
                        Name = reader.GetString(reader.GetOrdinal("Name"))
                    };
                }

                reader.Close();

                return chore;
            }
        }
    }

    public void Insert(Chore chore)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Chore (Name)
                                      OUTPUT INSERTED.Id
                                      VALUES (@name)";
                cmd.Parameters.AddWithValue("@name", chore.Name);
                int id = (int)cmd.ExecuteScalar();

                chore.Id = id;
            }
        }
    }

    public List<Chore> GetUnassignedChores()
    {
        using (var conn = Connection)
        {
            conn.Open();
            List<Chore> chores = new List<Chore>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"select c.Id, c.Name as Chore, rc.Id as rcId from Chore c left join RoommateChore rc on rc.ChoreId = c.Id where rc.Id is null";
                
                using (var reader = cmd.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {
                        int unassignedChoreIdPosition = reader.GetOrdinal("Id");
                        int unassignedChoreIdValue = reader.GetInt32(unassignedChoreIdPosition);
                        int unassignedChoreNamePosition = reader.GetOrdinal("Chore");
                        string unassignedChore = reader.GetString(unassignedChoreNamePosition);

                        Chore chore = new Chore()
                        {
                            Id = unassignedChoreIdValue,
                            Name = unassignedChore,
                        };

                        chores.Add(chore);
                    }
                }
            }
            return chores;
        }
    }

    //Next create a method in the ChoreRepository named AssignChore.
    //It should accept 2 parameters--a roommateId and a choreId.

    public void AssignChore(RoommateChore roommateChore)
    {
        using (var conn = Connection)
        {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"insert into RoommateChore (RoommateId, ChoreId) values (@roommateId, @choreId)";
                cmd.Parameters.AddWithValue("@roommateId", roommateChore.RoommateId);
                cmd.Parameters.AddWithValue("@choreId", roommateChore.ChoreId);

                cmd.ExecuteNonQuery(); 
            }
        }
    }
}
