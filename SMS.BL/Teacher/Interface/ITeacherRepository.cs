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
        /// <summary>
        /// Get tecahers list according to the status
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        IEnumerable<TeacherBO> GetTeachers(bool? isEnable = null);
        /// <summary>
        /// Get teacher ID
        /// </summary>
        /// <param name="teacherID"></param>
        /// <returns></returns>
        TeacherBO GetTeacherByID(long teacherID);
        /// <summary>
        ///Check the existence of teacher registration no
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="regNo"></param>
        /// <returns></returns>
        bool TeacherRegNoExists(long teacherId, string regNo);
        /// <summary>
        /// check the existence of display name
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="regNo"></param>
        /// <returns></returns>
        bool TeacherDisplayNameExists(long teacherId, string regNo);
        /// <summary>
        /// Save the tecaher details
        /// </summary>
        /// <param name="teacher"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool SaveTeacher(TeacherBO teacher, out string msg);
        /// <summary>
        /// Delete the teacher record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool DeleteTeacher(long id, out string msg);
        /// <summary>
        /// Check whether teacher is allcoated for any subject
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        bool IsTeacherReferenced(int teacherId);
        /// <summary>
        /// Change the status of the teacher
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        bool ToggleTeacherEnable(int teacherId, out string message);
    }
}
