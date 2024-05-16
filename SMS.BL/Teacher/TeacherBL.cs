using SMS.Data;
using SMS.Models.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Teacher
{
    public class TeacherBL: sms_dbEntities
    {
        /// <summary>
        /// Get the Teacher list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TeacherBO> GetAllTeacher()
        {
            var allTeachers = Teachers.Select(t => new TeacherBO()
            {
                TeacherID=t.TeacherID,
                TeacherRegNo=t.TeacherRegNo,
                FirstName=t.FirstName,
                MiddleName=t.MiddleName,
                LastName=t.LastName,
                DisplayName=t.DisplayName,
                Email=t.Email,
                Gender=t.Gender,
                DOB=t.DOB,
                Address=t.Address,
                ContactNo=t.ContactNo,
                IsEnable=t.IsEnable

            }).OrderBy(s => s.TeacherID).ToList();
            return allTeachers;
        }
        /// <summary>
        /// Get Tecaher details by its ID
        /// </summary>
        /// <param name="teacherID"></param>
        /// <returns></returns>
        public TeacherBO GetTeacherByID(long teacherID)
        {
            var result = Teachers.Select(t => new TeacherBO()
            {
                TeacherID = t.TeacherID,
                TeacherRegNo = t.TeacherRegNo,
                FirstName = t.FirstName,
                MiddleName = t.MiddleName,
                LastName = t.LastName,
                DisplayName = t.DisplayName,
                Email = t.Email,
                Gender = t.Gender,
                DOB = t.DOB,
                Address = t.Address,
                ContactNo = t.ContactNo,
                IsEnable = t.IsEnable

            }).Where(t => t.TeacherID == teacherID).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Add new teacher and edit the existing one
        /// can't edit the status if it is refernced to any other entities
        /// </summary>
        /// <param name="teacher"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SaveTeacher(TeacherBO teacher, out string msg)
        {
            msg = "";
            bool existingTeacher = Teachers.Any(s => s.TeacherID == teacher.TeacherID);
            bool teacherWhoTeaches = Teacher_Subject_Allocation.Any(a => a.TeacherID == teacher.TeacherID);
            try
            {
                if (existingTeacher)
                {
                   

                    var editTeacher = Teachers.SingleOrDefault(s => s.TeacherID == teacher.TeacherID);
                    if (editTeacher == null)
                    {
                        msg = "Unable to find the teacher for edit";
                        return false;
                    }

                    // Allow changing all properties except IsEnable if the teacher is referenced
                    if (editTeacher.IsEnable && teacherWhoTeaches)
                    {
                        editTeacher.TeacherRegNo = teacher.TeacherRegNo;
                        editTeacher.FirstName = teacher.FirstName;
                        editTeacher.MiddleName = teacher.MiddleName;
                        editTeacher.LastName = teacher.LastName;
                        editTeacher.DisplayName = teacher.DisplayName;
                        editTeacher.Email = teacher.Email;
                        editTeacher.Gender = teacher.Gender;
                        editTeacher.DOB = teacher.DOB;
                        editTeacher.Address = teacher.Address;
                        editTeacher.ContactNo = teacher.ContactNo;
                        msg = "Teacher details updated successfully, but IsEnable cannot be changed as the teacher is referenced.";
                    }
                    else
                    {
                        // If the teacher is not referenced, allow changing all properties
                        editTeacher.TeacherRegNo = teacher.TeacherRegNo;
                        editTeacher.FirstName = teacher.FirstName;
                        editTeacher.MiddleName = teacher.MiddleName;
                        editTeacher.LastName = teacher.LastName;
                        editTeacher.DisplayName = teacher.DisplayName;
                        editTeacher.Email = teacher.Email;
                        editTeacher.Gender = teacher.Gender;
                        editTeacher.DOB = teacher.DOB;
                        editTeacher.Address = teacher.Address;
                        editTeacher.ContactNo = teacher.ContactNo;
                        editTeacher.IsEnable = teacher.IsEnable;
                        msg = "Teacher details updated successfully.";
                    }

                    SaveChanges();
                    return true;
                }
                else
                {
                    // If it's a new teacher, add it directly without checks
                    var newTeacher = new SMS.Data.Teacher();
                    newTeacher.TeacherRegNo = teacher.TeacherRegNo;
                    newTeacher.FirstName = teacher.FirstName;
                    newTeacher.MiddleName = teacher.MiddleName;
                    newTeacher.LastName = teacher.LastName;
                    newTeacher.DisplayName = teacher.DisplayName;
                    newTeacher.Email = teacher.Email;
                    newTeacher.Gender = teacher.Gender;
                    newTeacher.DOB = teacher.DOB;
                    newTeacher.Address = teacher.Address;
                    newTeacher.ContactNo = teacher.ContactNo;
                    newTeacher.IsEnable = teacher.IsEnable;
                    Teachers.Add(newTeacher);
                    SaveChanges();
                    msg = "Teacher added successfully!";
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// delete tecaher if it is not referenced
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool DeleteTeacher(long id, out string msg)
        {
            msg = "";
            var teacher = Teachers.SingleOrDefault(t => t.TeacherID == id);
            try
            {
                if (teacher != null)
                {
                    if (teacher.Teacher_Subject_Allocation.Any())
                    {
                        msg = "Cannot delete teacher because it is referenced in other entities";
                        return false;
                    }

                    Teachers.Remove(teacher);
                    SaveChanges();
                    msg = "Successfully Removed";
                    return true;
                }
                msg = "Already Removed";
                return false;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }
        
    }
}
