using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.Allocation
{
    public class StudentAllocationViewModel
    {
        public long StudentAllocationID { get; set; }
        public string SubjectCode { get; set; }
        public string Name { get; set; }
        public string TeacherRegNo { get; set; }
        public string TeacherName { get; set; }
        public bool IsEnableStudent { get; set; }
    }
}
