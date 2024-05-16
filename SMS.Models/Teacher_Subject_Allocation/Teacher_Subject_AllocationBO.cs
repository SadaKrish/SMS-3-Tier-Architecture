using SMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Models.Teacher_Subject_Allocation
{
    public class Teacher_Subject_AllocationBO
    {
        public long SubjectAllocationID { get; set; }
        public long TeacherID { get; set; }
        public long SubjectID { get; set; }
        public virtual ICollection<Student_Subject_Teacher_Allocation> Student_Subject_Teacher_Allocation { get; set; }
      //  public virtual Subject Subject { get; set; }
        //public virtual Teacher Teacher { get; set; }
    }
}
