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

    public List<Roommate> GetAllRoommates()
    {
        using (var conn = Connection)
        {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select rm.Id, rm.FirstName, rm.LastName, rm.RentPortion, rm.MoveInDate, rm.RoomId, r.Name as RoomName, r.MaxOccupancy from Roommate rm join Room r on r.Id = rm.RoomId";

                var reader = cmd.ExecuteReader();

                List<Roommate> roommates = new List<Roommate>();

                while(reader.Read())
                {
                    int idColumnPosition = reader.GetOrdinal("Id");
                    int idValue = reader.GetInt32(idColumnPosition);

                    int firstNameColumnPosition = reader.GetOrdinal("FirstName");
                    string firstNameValue = reader.GetString(firstNameColumnPosition);

                    int lastNameColumnPosition = reader.GetOrdinal("LastName");
                    string lastNameValue = reader.GetString(lastNameColumnPosition);

                    int rentPortionPosition = reader.GetOrdinal("RentPortion");
                    int rentPortionValue = reader.GetInt32(rentPortionPosition);

                    int moveInDatePosition = reader.GetOrdinal("MoveInDate");
                    DateTime moveInDateTimeValue = reader.GetDateTime(moveInDatePosition);

                    int roomIdPosition = reader.GetOrdinal("RoomId");
                    int roomIdValue = reader.GetInt32(roomIdPosition);

                    int roomNamePosition = reader.GetOrdinal("RoomName");
                    string roomNameValue = reader.GetString(roomNamePosition);

                    int maxOccupancyPosition = reader.GetOrdinal("MaxOccupancy");
                    int maxOccupancyValue = reader.GetInt32(maxOccupancyPosition);

                    Roommate roommate = new Roommate()
                    {
                        Id = idValue,
                        FirstName = firstNameValue,
                        LastName = lastNameValue,
                        RentPortion = rentPortionValue,
                        MovedInDate = moveInDateTimeValue,
                        Room = new Room
                        {
                            Id = roomIdValue,
                            Name = roomNameValue,
                            MaxOccupancy = maxOccupancyValue
                        }
                    };

                    roommates.Add(roommate);
                }

                reader.Close();
                return roommates;
            }
        }
    }
}
