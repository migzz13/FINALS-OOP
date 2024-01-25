using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StartTaskManager();
        }

        private static void StartTaskManager()
        {
            var initialTasks = StreamReader("TaskData.csv");
            var taskManager = new TaskManager(initialTasks);

            Console.Write("Loading task manager");
            Thread.Sleep(750);
            Console.Write(".");
            Thread.Sleep(750);
            Console.Write(".");
            Thread.Sleep(750);
            Console.Write(".");
            Thread.Sleep(1250);

            Console.Clear();

            Console.WriteLine("Welcome To Task Manager!\n");

            while (true)
            {
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. Display Tasks");
                Console.WriteLine("2. Add Task");
                Console.WriteLine("3. Delete Task");
                Console.WriteLine("4. Assign Task");
                Console.WriteLine("5. Add Comment");
                Console.WriteLine("6. Complete Task");
                Console.WriteLine("7. Verify Task");
                Console.WriteLine("8. Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        taskManager.DisplayTasks();
                        break;
                    case "2":
                        taskManager.AddTask(); 
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "3":
                        Console.WriteLine("\nEnter Task Description to delete:");
                        string deleteTaskDescription = Console.ReadLine();
                        taskManager.DeleteTask(deleteTaskDescription);
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "4":
                        Console.WriteLine("\nEnter Task Description to assign or replace:");
                        string assignReplaceTaskDescription = Console.ReadLine();
                        taskManager.AssignTask(assignReplaceTaskDescription);
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "5":
                        Console.WriteLine("\nEnter Task Description to add a comment:");
                        string commentTaskDescription = Console.ReadLine();
                        taskManager.AddComment(commentTaskDescription);
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "6":
                        Console.WriteLine("\nEnter Task Description to complete:");
                        string completeTaskDescription = Console.ReadLine();
                        taskManager.CompleteTask(completeTaskDescription, DateTime.Now, string.Empty);
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "7":
                        Console.WriteLine("\nEnter Task Description to verify:");
                        string verifyTaskDescription = Console.ReadLine();
                        taskManager.VerifyTask(verifyTaskDescription);
                        Console.WriteLine("Press any key to continue");
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case "8":
                        Console.Clear();
                        Console.WriteLine("Thank you for using Task Manager\n");
                        taskManager.StreamWriter();
                        Thread.Sleep(1250);
                        Console.Clear();
                        Console.Write("Now Exiting Task Manager");
                        Thread.Sleep(750);
                        Console.Write(".");
                        Thread.Sleep(750);
                        Console.Write(".");
                        Thread.Sleep(750);
                        Console.Write(".");
                        Thread.Sleep(1750);
                        return;

                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        break;
                }
            }
        }

        private static DateTime? ParseDateTime(string dateTimeString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString))
            {
                return null;
            }

            if (DateTime.TryParse(dateTimeString, out DateTime result))
            {
                return result;
            }
            return DateTime.Now;
        }

        private static List<Task> StreamReader(string filePath)
        {
            var tasks = new List<Task>();

            if (!File.Exists(filePath))
            {
                return tasks;
            }

            using (StreamReader sr = new StreamReader(filePath))
            {
                string headerLine = sr.ReadLine();
                string[] headers = headerLine.Split(',');

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var values = line.Split(',');

                    if (values.Length == headers.Length)
                    {
                        var task = new Task();

                        for (int i = 0; i < headers.Length; i++)
                        {
                            switch (headers[i])
                            {
                                case "TaskDescription":
                                    task.TaskDescription = values[i];
                                    break;
                                case "CreationTime":
                                    task.CreationTime = DateTime.Parse(values[i]);
                                    break;
                                case "AssignedTo":
                                    task.AssignedTo = values[i];
                                    break;
                                case "AssignmentTime":
                                    task.AssignmentTime = ParseDateTime(values[i]);
                                    break;
                                case "CompletionTime":
                                    task.CompletionTime = ParseDateTime(values[i]);
                                    break;
                                case "TaskStatus":
                                    task.TaskStatus = values[i];
                                    break;
                                case "Comments":
                                    task.Comments = values[i];
                                    break;
                                case "VerificationStatus":
                                    task.VerificationStatus = values[i];
                                    break;
                                case "VerificationTime":
                                    task.VerificationTime = ParseDateTime(values[i]);
                                    break;
                                case "VerifierDetails":
                                    task.VerifierDetails = values[i];
                                    break;
                                case "VerificationComments":
                                    task.VerificationComments = values[i];
                                    break;
                                default:
                                    break;
                            }
                        }

                        tasks.Add(task);
                    }
                }
            }
            return tasks;
        }
    }
}
