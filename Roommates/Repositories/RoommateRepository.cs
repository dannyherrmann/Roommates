using Microsoft.Data.SqlClient;
using Roommates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roommates.Repositories;

public class RoommateRepository : BaseRepository
{
    public RoommateRepository(string connectionString) : base(connectionString) { }

    public Roommate? GetById(int id)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select rm.FirstName,rm.RentPortion,r.Id as roomId, r.Name, r.MaxOccupancy from Roommate rm join room r on r.Id = rm.RoomId where rm.Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                Roommate? roommate = null;

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        roommate = new Roommate
                        {
                            Id = id,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            Room = new Room
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("roomId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy"))
                            }
                        };
                    }
                }
                return roommate;
            }
        }
    }
}
