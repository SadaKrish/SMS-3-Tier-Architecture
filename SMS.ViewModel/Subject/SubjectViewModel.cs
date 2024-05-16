using SMS.Models.Subject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.ViewModel.Subject
{
    public class SubjectViewModel
    {
        public IEnumerable<SubjectBO> SubjectList { get; set; }
    }
}
