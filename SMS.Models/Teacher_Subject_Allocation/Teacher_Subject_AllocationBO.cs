using SMS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Models.Teacher_Subject_Allocation
{
    public class Teacher_Subject_AllocationBO
    {
        public long SubjectAllocationID { get; set; }
        [Required(ErrorMessage ="Teacher is required")]
        public long TeacherID { get; set; }
        [Required(ErrorMessage = "Subject is required")]
        public long SubjectID { get; set; }
        public bool IsAllocated { get; set; }
        public virtual ICollection<Student_Subject_Teacher_Allocation> Student_Subject_Teacher_Allocation { get; set; }
      //  public virtual Subject Subject { get; set; }
        //public virtual Teacher Teacher { get; set; }
    }
}
