using SMS.BL.Teacher.Interface;
using SMS.Data;
using SMS.Models.Student;
using SMS.Models.Subject;
using SMS.Models.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Teacher
{
    public class TeacherRepository:ITeacherRepository
    {
        private readonly SMS_DBEntities _dbEntities;

        public TeacherRepository()
        {
            _dbEntities = new SMS_DBEntities();
        }
        public TeacherRepository(SMS_DBEntities dbEntities)
        {
            _dbEntities = dbEntities;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TeacherBO> GetAllTeacher()
        {
            var allTeachers = _dbEntities.Teachers.Select(t => new TeacherBO()
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

            }).OrderBy(s => s.TeacherID).ToList();
            return allTeachers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public IEnumerable<TeacherBO> GetTeachers(bool? isEnable = null)
        {
            var query = _dbEntities.Teachers.AsQueryable();

            if (isEnable.HasValue)
            {
                query = query.Where(s => s.IsEnable == isEnable.Value);
            }

            var teachers = query.ToList();

            return teachers.Select(s => new TeacherBO
            {
                TeacherID = s.TeacherID,
                TeacherRegNo = s.TeacherRegNo,
                FirstName = s.FirstName,
                MiddleName = s.MiddleName,
                LastName = s.LastName,
                DisplayName = s.DisplayName,
                Email = s.Email,
                Gender = s.Gender,
                DOB = s.DOB,
                Address = s.Address,
                ContactNo = s.ContactNo,
                IsEnable = s.IsEnable
            }).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teacherID"></param>
        /// <returns></returns>
        public TeacherBO GetTeacherByID(long teacherID)
        {
            var result = _dbEntities.Teachers.Select(t => new TeacherBO()
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
        /// 
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="regNo"></param>
        /// <returns></returns>
        public bool TeacherRegNoExists(long teacherId, string regNo)
        {
            return _dbEntities.Teachers.Any(s => s.TeacherID != teacherId && s.TeacherRegNo == regNo);
        }
        public bool TeacherDisplayNameExists(long teacherId, string displayName)
        {
            return _dbEntities.Teachers.Any(s => s.TeacherID != teacherId && s.DisplayName == displayName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teacher"></param>
        /// <param name="msg"></param>
        /// <returns></returns>

        public bool SaveTeacher(TeacherBO teacher, out string msg)
        {
            msg = "";

            bool existingTeacher = _dbEntities.Teachers.Any(s => s.TeacherID == teacher.TeacherID);

            bool teacherteaches = _dbEntities.Teacher_Subject_Allocation.Any(a => a.TeacherID == teacher.TeacherID);

            try
            {
                if (existingTeacher)
                {
                    var editTeacher = _dbEntities.Teachers.SingleOrDefault(s => s.TeacherID == teacher.TeacherID);

                    if (editTeacher == null)
                    {
                        msg = "Unable to find the teacher for edit";
                        return false;
                    }

                    if (editTeacher.IsEnable && teacherteaches)
                    {
                        // If IsEnable is true and referenced, only allow changing SubjectCode and Name
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
                        _dbEntities.SaveChanges();
                        msg = "Teacher details updated successfully, but IsEnable cannot be changed as the teacher is referenced.";
                        return true;
                    }
                    else
                    {
                        // If IsEnable is false or not referenced, allow changing all properties
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
                        _dbEntities.SaveChanges();
                        return true;
                    }
                }
                else
                {
                    // If it's a new subject, add it directly without checks
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
                    _dbEntities.Teachers.Add(newTeacher);
                    _dbEntities.SaveChanges();
                    msg = "Teacher added successfully!";
                    return true;
                }
            }
            catch (Exception error)
            {
                msg = error.Message;
                return false;
            }
        }

        public bool DeleteTeacher(long id, out string msg)
        {
            msg = "";
            var teacher = _dbEntities.Teachers.SingleOrDefault(t => t.TeacherID == id);
            try
            {
                if (teacher != null)
                {
                    if (teacher.Teacher_Subject_Allocation.Any())
                    {
                        msg = "Cannot delete teacher because it is referenced in other entities";
                        return false;
                    }

                    //if (subject.SubjectCode.Any())
                    //{
                    //    msg = "This subject cannot be deleted";
                    //    return false;
                    //}

                    _dbEntities.Teachers.Remove(teacher);
                    _dbEntities.SaveChanges();
                    msg = "Successfully Removed";
                    return true;
                }
                msg = "Already Removed";
                return false;
            }
            catch (Exception exc)
            {
                msg = exc.Message;
                return false;
            }
        }

        public bool IsTeacherReferenced(int teacherId)
        {
            // Check if the subject is referenced in any teacher-subject allocations
            return _dbEntities.Teacher_Subject_Allocation.Any(tsa => tsa.TeacherID == teacherId);
        }

        public bool ToggleTeacherEnable(int teacherId, out string message)
        {
            var teacher = _dbEntities.Teachers
                                  .SingleOrDefault(s => s.TeacherID == teacherId);

            if (teacher == null)
            {
                message = "Teacher not found.";
                return false;
            }

            bool currentStatus = teacher.IsEnable;

            // If the current status is enabled, check if the subject is referenced in any related entities
            if (currentStatus && IsTeacherReferenced(teacherId))
            {
                message = $"Cannot change status because {teacher.DisplayName} is referenced in other entities.";
                return false;
            }

            // Toggle the enable status
            teacher.IsEnable = !currentStatus;
            _dbEntities.SaveChanges();

            message = currentStatus ? "Disabled Successfully" : "Enabled Successfully";
            return true;
        }
    }
}
