using SMS.Models.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Teacher.Interface
{
    public interface ITeacherRepository
    {
        IEnumerable<TeacherBO> GetAllTeacher();
        IEnumerable<TeacherBO> GetTeachers(bool? isEnable = null);
        TeacherBO GetTeacherByID(long teacherID);
        bool TeacherRegNoExists(long teacherId, string regNo);
        bool TeacherDisplayNameExists(long teacherId, string regNo);
        bool SaveTeacher(TeacherBO teacher, out string msg);

        bool DeleteTeacher(long id, out string msg);
        bool IsTeacherReferenced(int teacherId);

        bool ToggleTeacherEnable(int teacherId, out string message);
    }
}
