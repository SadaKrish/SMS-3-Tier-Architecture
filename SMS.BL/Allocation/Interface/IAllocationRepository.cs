using SMS.Models.Student_Allocation;
using SMS.Models.Teacher_Subject_Allocation;
using SMS.ViewModel.Allocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.BL.Allocation.Interface
{
    public interface IAllocationRepository
    {
        IEnumerable<SubjectAllocationDetailViewModel> GetAllSubjectAllocations();
        Teacher_Subject_AllocationBO GetSubjectAllocationById(long subjectAllocationId);
        IEnumerable<SelectListItem> GetEnabledTeachersList();
        IEnumerable<SelectListItem> GetEnabledSubjectList();
        bool SaveSubjectAllocation(Teacher_Subject_AllocationBO allocation, out string message);

        bool DeleteSubjectAllocation(long id, out string msg);

        IEnumerable<SubjectAllocationDetailViewModel> SearchSubjectAllocations(string searchText, string searchCategory);

        IEnumerable<StudentAllocationGroupedViewModel> GetAllStudentAllocation(bool? status = null);
        Student_Subject_Teacher_AllocationBO GetStudentAllocationById(long studentAllocationId);
        IEnumerable<object> GetTeachersBySubjectID(int subjectID);
        IEnumerable<SelectListItem> GetStudentList();
        IEnumerable<SelectListItem> GetTeacherList();
        IEnumerable<SelectListItem> GetSubjectList();
        IEnumerable<SelectListItem> GetAllSubjectsFromAllocation();

        bool SaveStudentAllocation(Student_Subject_Teacher_AllocationBO studentAllocation, out string msg);

        bool DeleteStudentAllocation(long id, out string msg);

        IEnumerable<StudentAllocationGroupedViewModel> SearchStudentAllocations(string searchText, string searchCategory);

    }
}
