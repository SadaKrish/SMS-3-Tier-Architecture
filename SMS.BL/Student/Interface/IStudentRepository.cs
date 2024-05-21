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
        IEnumerable<StudentBO> GetAllStudent();
        IEnumerable<StudentBO> GetStudents(bool? isEnable = null);
        StudentBO GetStudentByID(long studentID);
        bool StudentRegNoExists(long studentID, string regNo);
        bool StudentDisplayNameExists(long studentId, string displayName);
        bool SaveStudent(StudentBO student, out string msg);
        bool IsStudentReferenced(long studentId);
        bool DeleteStudent(long id, out string msg, out bool requiresConfirmation);

        bool ToggleStudentEnable(int studentId, out string message);
    }
}
