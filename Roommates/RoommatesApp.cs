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
            string spectreSelection = GetSpectreSelection();

            switch (spectreSelection)
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
                case ("Assign Unassigned Chores"):
                    AssignUnassignedChore();
                    break;
                case ("Assign chore to roommate"):
                    AssignChore();
                    break;
                case ("Exit"):
                    runProgram = false;
                    break;
            }
        }

        Console.Clear();

        AnsiConsole.Write(
            new FigletText("Goodbye!")
            .Centered()
            .Color(Color.Aquamarine1));

    }

    static string GetSpectreSelection()
    {
        Console.Clear();

        Console.Title = "ROOMIES";

        AnsiConsole.Write(
            new FigletText("ROOMIES")
            .LeftJustified()
            .Color(Color.Aquamarine1));

        var spectreSelection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Menu Options:")
            .PageSize(10)
            .HighlightStyle("deeppink2")
            .AddChoices(new[] {
                    "Show all rooms",
                    "Search for room",
                    "Add a room",
                    "Show all chores",
                    "Search for a chore",
                    "Add a chore",
                    "Find roommate",
                    "Assign Unassigned Chores",
                    "Assign chore to roommate",
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

    public string UnassignedChoresPrompt()
    {
        List<Chore> unassignedChores = choreRepo.GetUnassignedChores();
        
        var prompt = new SelectionPrompt<string>()
            .Title("Select unassigned chore:")
            .PageSize(10)
            .HighlightStyle("deeppink2");

        foreach (Chore unassignedChore in unassignedChores)
        {
            prompt.AddChoices(new[] { $"{unassignedChore.Id}-{unassignedChore.Name}" });
        }
        return AnsiConsole.Prompt(prompt);
    }

    public string ChorePrompt()
    {
        List<Chore> chores = choreRepo.GetAll();

        var prompt = new SelectionPrompt<string>()
            .Title("Select a chore:")
            .PageSize(10)
            .HighlightStyle("deeppink2");

        foreach (Chore chore in chores)
        {
            prompt.AddChoices(new[] { $"{chore.Id}-{chore.Name}" });
        }

        return AnsiConsole.Prompt(prompt);
    }

    public string RoommatePrompt()
    {
        List<Roommate> roommates = roommateRepo.GetAllRoommates();

        var prompt = new SelectionPrompt<string>()
            .Title("Select a roommate:")
            .PageSize(10)
            .HighlightStyle("deeppink2");

        foreach (Roommate roommate in roommates)
        {
            prompt.AddChoices(new[] { $"{roommate.Id}-{roommate.FirstName}" });
        }

        return AnsiConsole.Prompt(prompt);
    }

    public void AssignChore()
    {
        var choreSelection = ChorePrompt();
        var choreIdString = choreSelection.Split('-')[0];
        var choreId = Convert.ToInt32(choreIdString);
        Console.Clear();
        var roommateSelection = RoommatePrompt();
        var roommateIdString = roommateSelection.Split('-')[0];
        var roommateId = Convert.ToInt32(roommateIdString);
        choreRepo.AssignChore(new RoommateChore { RoommateId = roommateId, ChoreId = choreId });
        Console.Clear();
        Console.WriteLine($"{roommateRepo.GetById(roommateId)?.FirstName} has been assigned --> {choreRepo.GetById(choreId).Name}");
        Console.WriteLine();
        Console.Write("Press any key to return to the main menu");
        Console.ReadKey();
    }

    public void AssignUnassignedChore()
    {
        var unassignedChoreSelection = UnassignedChoresPrompt();
        var unassignedChoreIdString = unassignedChoreSelection.Split('-')[0];
        var unassignedChoreId = Convert.ToInt32(unassignedChoreIdString);
        Console.Clear();
        var roommateSelection = RoommatePrompt();
        var roommateIdString = roommateSelection.Split('-')[0];
        var roommateId = Convert.ToInt32(roommateIdString);
        choreRepo.AssignChore(new RoommateChore { RoommateId = roommateId, ChoreId = unassignedChoreId });
        Console.Clear();
        Console.WriteLine($"{roommateRepo.GetById(roommateId)?.FirstName} has been assigned --> {choreRepo.GetById(unassignedChoreId).Name}");
        Console.WriteLine();
        Console.Write("Press any key to return to the main menu");
        Console.ReadKey();
    }
}
