using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementSystem
{
    internal class Task
    {
        public string TaskDescription;
        public DateTime CreationTime;
        public string AssignedTo;
        public DateTime? AssignmentTime;
        public DateTime? CompletionTime;
        public string TaskStatus;
        public string Comments;
        public string VerificationStatus;
        public DateTime? VerificationTime;
        public string VerifierDetails;
        public string VerificationComments;
    }
}
