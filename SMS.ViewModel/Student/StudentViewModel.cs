using SMS.Models.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.Student
{
    public class StudentViewModel
    {
        public IEnumerable<StudentBO> StudentList { get; set; }
    }
}
