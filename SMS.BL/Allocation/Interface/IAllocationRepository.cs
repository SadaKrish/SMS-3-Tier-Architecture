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
        /// <summary>
        /// Get all subject allocations
        /// </summary>
        /// <returns></returns>
        IEnumerable<SubjectAllocationDetailViewModel> GetAllSubjectAllocations();
        /// <summary>
        /// Get subject allocation by id
        /// </summary>
        /// <param name="subjectAllocationId"></param>
        /// <returns></returns>
        Teacher_Subject_AllocationBO GetSubjectAllocationById(long subjectAllocationId);
        /// <summary>
        /// Get enabled teacher list from teacher table
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetEnabledTeachersList();
        /// <summary>
        /// Get Enables subject list from subject table
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetEnabledSubjectList();
        /// <summary>
        /// Save the subject allocation
        /// </summary>
        /// <param name="allocation"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        bool SaveSubjectAllocation(Teacher_Subject_AllocationBO allocation, out string message);
        /// <summary>
        /// Delete the subject allocation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool DeleteSubjectAllocation(long id, out string msg);
        /// <summary>
        /// Search in subject allocation
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="searchCategory"></param>
        /// <returns></returns>
        IEnumerable<SubjectAllocationDetailViewModel> SearchSubjectAllocations(string searchText, string searchCategory);
        /// <summary>
        /// Get all allocated students
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        IEnumerable<StudentAllocationGroupedViewModel> GetAllStudentAllocation(bool? status = null);
        /// <summary>
        /// Get student allocation by id
        /// </summary>
        /// <param name="studentAllocationId"></param>
        /// <returns></returns>
        Student_Subject_Teacher_AllocationBO GetStudentAllocationById(long studentAllocationId);
        /// <summary>
        /// Get teachers list by subject allocated
        /// </summary>
        /// <param name="subjectID"></param>
        /// <returns></returns>
        IEnumerable<object> GetTeachersBySubjectID(int subjectID);
        /// <summary>
        /// Get Enabled students list
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetStudentList();
        /// <summary>
        /// Get allocated teacher list
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetTeacherList();
        /// <summary>
        /// Get alla subject from allocation
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetAllSubjectsFromAllocation();

        /// <summary>
        /// Save the student allocation
        /// </summary>
        /// <param name="studentAllocation"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool SaveStudentAllocation(Student_Subject_Teacher_AllocationBO studentAllocation, out string msg);
        /// <summary>
        /// Delete the student allocation by subject
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool DeleteStudentAllocation(long id, out string msg);
        /// <summary>
        /// Search in student allocation
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="searchCategory"></param>
        /// <returns></returns>
        IEnumerable<StudentAllocationGroupedViewModel> SearchStudentAllocations(string searchText, string searchCategory);

    }
}
