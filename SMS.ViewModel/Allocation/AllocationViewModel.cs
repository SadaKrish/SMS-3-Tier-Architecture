using SMS.Models.Student;
using SMS.Models.Student_Allocation;
using SMS.Models.Subject;
using SMS.Models.Teacher;
using SMS.Models.Teacher_Subject_Allocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.Allocation
{
    public class AllocationViewModel
    {
        public IEnumerable<Teacher_Subject_AllocationBO> Teacher_Subject_AllocationList { get; set; }
        public IEnumerable<Student_Subject_Teacher_AllocationBO> Student_Subject_Teacher_AllocationList{ get; set; }

        public IEnumerable<StudentBO> StudentList { get; set; }
        public IEnumerable<TeacherBO> TeacherList { get; set; }
        public IEnumerable<SubjectBO> SubjectList { get; set; }

    }
}
