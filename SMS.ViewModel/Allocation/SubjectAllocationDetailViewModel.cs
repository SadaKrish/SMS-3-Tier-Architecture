using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.Allocation
{
    public class SubjectAllocationDetailViewModel
    {
        [DisplayName("Teacher Registration NO")]
        public string TeacherRegNo { get; set; }
        [DisplayName("Teacher Name")]
        public string DisplayName { get; set; }
        [DisplayName("Subject Code")]

       
        public IEnumerable<SubjectAllocationViewModel>Subjects { get; set; }
    }
}
