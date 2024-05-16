using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Models.Student_Allocation
{
    public class Student_Subject_Teacher_AllocationBO
    {
        public long SubjectID;

        public long StudentAllocationID { set; get; }
        public long StudentID { get; set; }

        public long SubjectAllocationID { get; set; }
        //public object SubjectID { get; set; }
    }
}
