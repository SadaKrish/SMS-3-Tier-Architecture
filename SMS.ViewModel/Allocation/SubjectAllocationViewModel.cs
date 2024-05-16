using SMS.Models.Teacher_Subject_Allocation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.Allocation
{
    public class SubjectAllocationViewModel
    {
        [DisplayName("Teacher Registration NO")]
        public string TeacherRegNo { get; set; }
        [DisplayName("Teacher Name")]
        public string DisplayName { get; set; }
        [DisplayName("Subject Code")]
        public string SubjectCode { get; set; }
        [DisplayName("Subject")]
        public string Name { get; set; }
        public IEnumerable<Teacher_Subject_AllocationBO> SubjectAllocation { get; set; }
    }
}
