using SMS.Models.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Student.Interface
{
    public interface IStudentRepository
    {
        /// <summary>
        /// Get All student details
        /// </summary>
        /// <returns></returns>
        IEnumerable<StudentBO> GetAllStudent();
        /// <summary>
        /// get students based on status
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        IEnumerable<StudentBO> GetStudents(bool? isEnable = null);
        /// <summary>
        /// get student by ID
        /// </summary>
        /// <param name="studentID"></param>
        /// <returns></returns>
        StudentBO GetStudentByID(long studentID);
        /// <summary>
        /// Check whether the reg no is available
        /// </summary>
        /// <param name="studentID"></param>
        /// <param name="regNo"></param>
        /// <returns></returns>
        bool StudentRegNoExists(long studentID, string regNo);
        /// <summary>
        /// check whether the display name exists
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        bool StudentDisplayNameExists(long studentId, string displayName);
        /// <summary>
        /// Save and edit student details
        /// </summary>
        /// <param name="student"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool SaveStudent(StudentBO student, out string msg);
        /// <summary>
        /// Check whther the student id is referenced in other entity
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        bool IsStudentReferenced(long studentId);
        /// <summary>
        /// Delete the student record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <param name="requiresConfirmation"></param>
        /// <returns></returns>
        bool DeleteStudent(long id, out string msg, out bool requiresConfirmation);
        /// <summary>
        /// Change status of a student
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        bool ToggleStudentEnable(int studentId, out string message);
        /// <summary>
        /// search students details
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="searchCategory"></param>
        /// <returns></returns>
        IEnumerable<StudentBO> SearchStudents(string searchText, string searchCategory);
    }
}
