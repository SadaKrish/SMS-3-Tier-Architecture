using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.Allocation
{
    public class TeacherAllocationViewModel
    {
        public string TeacherRegNo { get; set; }
        public string TeacherName { get; set; }
        public string SubjectCode { get; set; }
        public string Name { get; set; }
        public IEnumerable<SubjectAllocationViewModel> Subjects { get; set; }
    }
}
