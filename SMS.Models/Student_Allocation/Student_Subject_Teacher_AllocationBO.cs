using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Models.Student_Allocation
{
    public class Student_Subject_Teacher_AllocationBO
    {
       // [Required(ErrorMessage = "Subject is required")]
        public long SubjectID;
        [Required(ErrorMessage = "Teacher is required")]
        public long TeacherID;

        public long StudentAllocationID { set; get; }
        //[Required(ErrorMessage ="Student is required")]
        public long StudentID { get; set; }

        public long SubjectAllocationID { get; set; }
        //public object SubjectID { get; set; }
    }
}
