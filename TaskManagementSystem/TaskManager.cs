using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementSystem
{
    internal class TaskManager
    {
        private List<Task> tasks;

        public TaskManager(List<Task> initialTasks)
        {
            tasks = initialTasks;
        }

        public void DisplayTasks()
        {
            Console.WriteLine("Task List:");
            Console.WriteLine("-----------------------------");

            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    Console.WriteLine($"Task: {task.TaskDescription}");
                    Console.WriteLine($"Creation Time: {task.CreationTime}");
                    Console.WriteLine($"Assigned To: {task.AssignedTo}");

                    if (task.AssignmentTime != null)
                    {
                        Console.WriteLine($"Assignment Time: {task.AssignmentTime}");
                    }
                    else
                    {
                        Console.WriteLine("Assignment Time: (Not assigned)");
                    }

                    if (task.CompletionTime != null)
                    {
                        Console.WriteLine($"Completion Time: {task.CompletionTime}");
                    }
                    else
                    {
                        Console.WriteLine("Completion Time: (Not completed)");
                    }

                    Console.WriteLine($"Comment/s: {(string.IsNullOrEmpty(task.Comments) ? "No comment" : task.Comments)}");
                    Console.WriteLine($"Task Status: {task.TaskStatus}");

                    if (!string.IsNullOrEmpty(task.VerificationStatus) && task.VerificationStatus != "Not Verified")
                    {
                        if (task.VerificationTime.HasValue)
                        {
                            Console.WriteLine($"Verification Time: {task.VerificationTime}");
                        }
                        else
                        {
                            Console.WriteLine("Verification Time: (Not verified)");
                        }

                        Console.WriteLine($"Verifier Name: {task.VerifierDetails}");
                    }

                    Console.WriteLine($"Verification Status: {task.VerificationStatus}");
                    Console.WriteLine($"Verification Comments: {(string.IsNullOrEmpty(task.VerificationComments) ? "No comment" : task.VerificationComments)}");
                    Console.WriteLine("-----------------------------");
                }
            }
            else
            {
                Console.WriteLine("No tasks found.");
            }
        }

        public void AddTask()
        {
            Console.WriteLine("\nEnter Task Description:");
            string description = Console.ReadLine();

            Console.WriteLine("\nEnter Assigned To (leave blank for 'Not assigned'):");
            string assignedTo = Console.ReadLine();
            assignedTo = string.IsNullOrEmpty(assignedTo) ? "Not assigned" : assignedTo;

            Console.WriteLine("\nEnter Assignment Time (Format: M/DD/YY HH:MM:SS AM/PM or leave blank for current time):");
            string assignmentTimeString = Console.ReadLine();
            DateTime? assignmentTime = string.IsNullOrEmpty(assignmentTimeString)
                ? (DateTime?)null
                : DateTime.Parse(assignmentTimeString);

            if (!assignmentTime.HasValue && !string.IsNullOrEmpty(assignedTo))
            {
                assignmentTime = DateTime.Now;
            }

            Console.WriteLine("\nIs the task completed? (y/n):");
            bool isCompleted = Console.ReadLine().ToLower() == "y";

            DateTime? completionTimeNullable = isCompleted ? GetCompletionTime() : null;

            string completionTimeString = completionTimeNullable.HasValue
                ? completionTimeNullable.Value.ToString()
                : string.Empty;

            var newTask = new Task
            {
                TaskDescription = description,
                CreationTime = DateTime.Now,
                AssignedTo = assignedTo,
                AssignmentTime = assignmentTime,
                CompletionTime = isCompleted ? GetCompletionTime() : null,
                TaskStatus = isCompleted ? "For Verification" : (string.IsNullOrEmpty(assignedTo) || assignedTo.ToLower() == "Not assigned") ? "Open" : "Assigned",
                VerificationStatus = isCompleted ? "For Verification" : "Not Verified"
            };

            tasks.Add(newTask);
        }

        public void DeleteTask(string taskDescription)
        {
            var task = FindTask(taskDescription);

            if (task != null)
            {
                tasks.Remove(task);
                Console.WriteLine("Task deleted successfully.");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }

            StreamWriter();
        }

        public void AssignTask(string taskDescription)
        {
            var task = FindTask(taskDescription);

            if (task != null)
            {
                Console.WriteLine("\nEnter new Assigned To (Leave blank for 'Not assigned'):");
                string newAssignedTo = Console.ReadLine();
                newAssignedTo = string.IsNullOrEmpty(newAssignedTo) ? "Not assigned" : newAssignedTo;

                task.AssignedTo = newAssignedTo;
                task.TaskStatus = string.IsNullOrEmpty(newAssignedTo) || newAssignedTo.ToLower() == "Not assigned"
                ? "Open"
                : "Assigned";

                Console.WriteLine("Assigned or replaced successfully.");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }

            StreamWriter();
        }
        public void AddComment(string taskDescription)
        {
            var task = FindTask(taskDescription);

            if (task != null)
            {
                Console.WriteLine("Select comment type:");
                Console.WriteLine("1. Task-related comment");
                Console.WriteLine("2. Verification Comment");

                string commentTypeChoice = Console.ReadLine();

                Console.WriteLine("Enter Comment (Leave blank for no comment):");
                string comment = Console.ReadLine();

                if (string.IsNullOrEmpty(comment))
                {
                    Console.WriteLine("No comment added.");
                    return;
                }

                switch (commentTypeChoice)
                {
                    case "1":
                        AddTaskRelatedComment(task, comment);
                        Console.WriteLine("Task-related comment added successfully.");
                        break;
                    case "2":
                        AddVerificationComment(task, comment);
                        Console.WriteLine("Verification comment added successfully.");
                        break;
                    default:
                        Console.WriteLine("Invalid comment type. No comment added.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Task not found.");
            }

            StreamWriter();
        }

        private void AddTaskRelatedComment(Task task, string comment)
        {
            if (string.IsNullOrEmpty(task.Comments) || task.Comments.ToLower() == "No comment")
            {
                task.Comments = comment;
            }
            else
            {
                task.Comments += $" | {comment}";
            }
        }

        private void AddVerificationComment(Task task, string comment)
        {
            if (string.IsNullOrEmpty(task.VerificationComments) || task.VerificationComments.ToLower() == "No comment")
            {
                task.VerificationComments = comment;
            }
            else
            {
                task.VerificationComments += $" | {comment}";
            }
        }

        public void CompleteTask(string taskDescription, DateTime? completionTime, string completionComment)
        {
            var task = FindTask(taskDescription);

            if (task != null)
            {
                if (task.TaskStatus == "Assigned" || task.TaskStatus == "Open")
                {
                    if (completionTime.HasValue)
                    {
                        task.CompletionTime = completionTime;
                        task.TaskStatus = "For Verification";
                        task.VerificationStatus = "Not Verified";


                        task.Comments += $"{(string.IsNullOrEmpty(completionComment) ? "No comment" : completionComment)}";

                        Console.WriteLine("Task completed successfully and waiting for verification.");
                    }
                    else
                    {
                        Console.WriteLine("Task cannot be completed without completion time.");
                    }
                }
                else if (task.TaskStatus == "For Verification")
                {
                    Console.WriteLine("Task already completed. Cannot complete again.");
                }
                else
                {
                    Console.WriteLine("Task cannot be completed. It is not assigned or already completed.");
                }
            }
            else
            {
                Console.WriteLine("Task not found.");
            }

            StreamWriter();
        }

        private DateTime? GetCompletionTime()
        {
            Console.WriteLine("\nEnter Completion Time (Format: M/DD/YY HH:MM:SS AM/PM or leave blank for current time):");
            string completionTimeString = Console.ReadLine();

            if (string.IsNullOrEmpty(completionTimeString))
            {
                return DateTime.Now;
            }

            return DateTime.Parse(completionTimeString);
        }

        public void VerifyTask(string taskDescription)
        {
            var task = FindTask(taskDescription);

            if (task != null)
            {
                if (task.TaskStatus == "For Verification")
                {
                    if (task.CompletionTime.HasValue)
                    {
                        (task.VerificationTime, task.VerifierDetails) = GetVerificationDetails();

                        Console.WriteLine("\nSelect Verification Status:");
                        Console.WriteLine("1. Verified");
                        Console.WriteLine("2. For Revision");

                        string verificationStatusChoice = Console.ReadLine();

                        switch (verificationStatusChoice)
                        {
                            case "1":
                                task.VerificationStatus = "Verified";
                                task.TaskStatus = "Closed";
                                Console.WriteLine("Task verified successfully and marked as closed.");
                                break;
                            case "2":
                                task.VerificationStatus = "For Revision";
                                task.TaskStatus = "For Revision";
                                Console.WriteLine("Task marked for revision.");
                                break;
                            default:
                                Console.WriteLine("Invalid verification status. Task not verified.");
                                return;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Task not yet completed. Cannot verify yet.");
                    }
                }
                else
                {
                    Console.WriteLine("Task cannot be verified. It is not in verification status.");
                }
            }
            else
            {
                Console.WriteLine("Task not found.");
            }

            StreamWriter();
        }


        public void VerifyUncompletedTask(string taskDescription)
        {
            var task = FindTask(taskDescription);

            if (task != null)
            {
                Console.WriteLine("Task not yet completed. Cannot verify yet.");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }
        public Task FindTask(string taskDescription)
        {
            return tasks.Find(t => t.TaskDescription.Equals(taskDescription, StringComparison.OrdinalIgnoreCase));
        }

        private (DateTime?, string) GetVerificationDetails()
        {
            Console.WriteLine("\nEnter Verification Time (Format: M/DD/YY HH:MM:SS AM/PM or leave blank for current time):");
            string verificationTimeString = Console.ReadLine();
            DateTime? verificationTime = string.IsNullOrEmpty(verificationTimeString)
                ? DateTime.Now
                : DateTime.Parse(verificationTimeString);

            Console.WriteLine("\nEnter Verifier's Name:");
            string verifierName = Console.ReadLine();

            return (verificationTime, verifierName);
        }
        public void StreamWriter()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("TaskData.csv"))
                {
                    sw.WriteLine("TaskDescription,CreationTime,AssignedTo,AssignmentTime,CompletionTime,TaskStatus,Comments,VerificationStatus,VerificationTime,VerifierDetails,VerificationComments");

                    foreach (var task in tasks)
                    {
                        string assignmentTimeStr = task.AssignmentTime.HasValue
                            ? task.AssignmentTime.Value.ToString()
                            : string.Empty;

                        string comments = task.Comments != null ? task.Comments.Replace("\n", "\\n") : string.Empty;

                        sw.WriteLine($"{task.TaskDescription},{task.CreationTime},{task.AssignedTo},{assignmentTimeStr},{task.CompletionTime},{task.TaskStatus},{comments},{task.VerificationStatus},{task.VerificationTime},{task.VerifierDetails},{task.VerificationComments}");
                    }
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tasks to CSV: {ex.Message}");
            }
        }
    }
}
