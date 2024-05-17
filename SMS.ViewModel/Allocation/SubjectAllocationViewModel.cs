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
         public long SubjectAllocationID { get; set; }
        public long StudentAllocationID { get; set; }
        public string SubjectCode { get; set; }
        public string Name { get; set; }
        //public IEnumerable<Teacher_Subject_AllocationBO> SubjectAllocation { get; set; }
    }
}
