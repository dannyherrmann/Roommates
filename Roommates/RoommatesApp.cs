using Roommates.Models;
using Roommates.Repositories;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roommates;

public class RoommatesApp
{
    private const string CONNECTION_STRING = @"server=localhost\SQLExpress;database=Roommates;integrated security=true;Trust Server Certificate=true";
    RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
    ChoreRepository choreRepo = new ChoreRepository(CONNECTION_STRING);
    RoommateRepository roommateRepo = new RoommateRepository(CONNECTION_STRING);

    public void Run()
    {
        bool runProgram = true;

        while (runProgram)
        {
            string selection = GetSpectreSelection();

            switch (selection)
            {
                case ("Show all rooms"):
                    ShowAllRooms();
                    break;
                case ("Search for room"):
                    SearchForRoom();
                    break;
                case ("Add a room"):
                    AddRoom();
                    break;
                case ("Show all chores"):
                    ShowChores();
                    break;
                case ("Search for a chore"):
                    SearchChores();
                    break;
                case ("Add a chore"):
                    AddChore();
                    break;
                case ("Find roommate"):
                    FindRoommate();
                    break;
                case ("Get Unassigned Chores"):
                    GetUnassignedChores();
                    break;
                case ("Exit"):
                    runProgram = false;
                    break;
            }
        }
    }
    
    static string GetSpectreSelection()
    {
        Console.Clear();

        Console.Title = "Roommates";

        AnsiConsole.Write(
            new FigletText("Roommates")
            .LeftJustified()
            .Color(Color.Aquamarine1));

        var spectreSelection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Options")
            .PageSize(10)
            .AddChoices(new[] {
                    "Show all rooms",
                    "Search for room",
                    "Add a room",
                    "Show all chores",
                    "Search for a chore",
                    "Add a chore",
                    "Find roommate",
                    "Get Unassigned Chores",
                    "Exit"
            }));

        return spectreSelection;
    }
    
    public void ShowAllRooms()
    {
        List<Room> rooms = roomRepo.GetAll();
        foreach (Room r in rooms)
        {
            Console.WriteLine($"{r.Name} has an Id of {r.Id} and a max occupancy of {r.MaxOccupancy}");
        }
        Console.Write("Press any key to return to the main menu");
        Console.ReadKey();
    }

    public void SearchForRoom()
    {
        Console.Write("Room Id: ");
        int roomId = int.Parse(Console.ReadLine());

        Room room = roomRepo.GetById(roomId);

        Console.WriteLine($"{room.Id} - {room.Name} Max Occupancy({room.MaxOccupancy})");
        Console.Write("Press any key to continue");
        Console.ReadKey();
    }

    public void AddRoom()
    {
        Console.Write("Room name: ");
        string name = Console.ReadLine();

        Console.Write("Max occupancy: ");
        int max = int.Parse(Console.ReadLine());

        Room roomToAdd = new Room()
        {
            Name = name,
            MaxOccupancy = max
        };

        roomRepo.Insert(roomToAdd);

        Console.WriteLine($"{roomToAdd.Name} has been added and assigned an Id of {roomToAdd.Id}");
        Console.Write("Press any key to continue");
        Console.ReadKey();
    }

    public void ShowChores()
    {
        List<Chore> chores = choreRepo.GetAll();
        foreach (Chore c in chores)
        {
            Console.WriteLine($"{c.Name} has an Id of {c.Id}");
        }
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
    }

    public void SearchChores()
    {
        Console.WriteLine("Chore Id: ");
        int choreId = int.Parse(Console.ReadLine());

        Chore chore = choreRepo.GetById(choreId);

        Console.WriteLine($"{chore.Id} - {chore.Name}");
        Console.Write("Press any key to continue");
        Console.ReadKey();
    }

    public void AddChore()
    {
        Console.WriteLine("Chore name: ");
        string choreName = Console.ReadLine();

        Chore choreToAdd = new Chore()
        {
            Name = choreName,
        };
        choreRepo.Insert(choreToAdd);
        Console.WriteLine($"{choreToAdd.Name} has been added and assigned an Id of {choreToAdd.Id}");
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
    }

    public void FindRoommate()
    {
        Console.Write("Roommate Id: ");
        int roommateId = int.Parse(Console.ReadLine());

        Roommate roommate = roommateRepo.GetById(roommateId);

        Console.WriteLine($@"
First Name: {roommate.FirstName}
Rent Portion: {roommate.RentPortion}
Room Name: {roommate.Room.Name}");
        Console.WriteLine();
        Console.Write("Press any key to continue");
        Console.ReadKey();
    }

    public void GetUnassignedChores()
    {
        List<Chore> unassignedChores = choreRepo.GetUnassignedChores();
        foreach (Chore unassignedChore in unassignedChores)
        {
            Console.WriteLine($"{unassignedChore.Name}");
        }
        Console.Write("Press any key to continue");
        Console.ReadKey();
    }
}
