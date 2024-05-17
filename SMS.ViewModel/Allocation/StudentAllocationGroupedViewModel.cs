using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.Allocation
{
    public class StudentAllocationGroupedViewModel
    {
        public string StudentRegNo { get; set; }
        public string DisplayName { get; set; }
        
        public IEnumerable<StudentAllocationViewModel> StudentAllocations { get; set; }
    }
}
