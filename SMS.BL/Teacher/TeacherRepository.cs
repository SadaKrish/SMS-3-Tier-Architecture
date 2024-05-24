/// <summary>
/// 
/// </summary>
/// <author>Sadakshini</author>
using SMS.BL.Student;
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
        /// get teachers according to the status
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
                IsEnable = s.IsEnable,
                IsAllocated = _dbEntities.Teacher_Subject_Allocation.Any(a => a.TeacherID==s.TeacherID)
            }).ToList();
        }

        /// <summary>
        /// Get tecaher by a TeacherID
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
        /// Check teacher registration number esits or not
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="regNo"></param>
        /// <returns></returns>
        public bool TeacherRegNoExists(long teacherId, string regNo)
        {
            return _dbEntities.Teachers.Any(s => s.TeacherID != teacherId && s.TeacherRegNo == regNo);
        }
        /// <summary>
        /// Check teacher display name exists or not
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public bool TeacherDisplayNameExists(long teacherId, string displayName)
        {
            return _dbEntities.Teachers.Any(s => s.TeacherID != teacherId && s.DisplayName == displayName);
        }

        /// <summary>
        /// Save the teacher details
        /// </summary>
        /// <param name="teacher"></param>
        /// <param name="msg"></param>
        /// <returns></returns>

        public bool SaveTeacher(TeacherBO teacher, out string msg)
        {
            msg = "";

            var existingTeacher = _dbEntities.Teachers.SingleOrDefault(s => s.TeacherID == teacher.TeacherID);
            if (existingTeacher != null)
            {
                // Editing existing student
                if (existingTeacher.TeacherRegNo != teacher.TeacherRegNo && TeacherRegNoExists(teacher.TeacherID, teacher.TeacherRegNo))
                {
                    msg = "Registration No already exists";
                    return false;
                }

                if (existingTeacher.DisplayName != teacher.DisplayName && TeacherDisplayNameExists(teacher.TeacherID, teacher.DisplayName))
                {
                    msg = "Display name already exists";
                    return false;
                }

                // Update fields
                existingTeacher.TeacherRegNo = teacher.TeacherRegNo;
                existingTeacher.FirstName = teacher.FirstName;
                existingTeacher.MiddleName = teacher.MiddleName;
                existingTeacher.LastName = teacher.LastName;
                existingTeacher.DisplayName = teacher.DisplayName;
                existingTeacher.Email = teacher.Email;
                existingTeacher.Gender = teacher.Gender;
                existingTeacher.DOB = teacher.DOB;
                existingTeacher.Address = teacher.Address;
                existingTeacher.ContactNo = teacher.ContactNo;
                existingTeacher.IsEnable = teacher.IsEnable;
                _dbEntities.SaveChanges();
                msg = "Teacher details updated successfully";
                return true;
            }
            else
            {
                // Adding new student
                if (TeacherRegNoExists(teacher.TeacherID, teacher.TeacherRegNo))
                {
                    msg = "Registration No already exists";
                    return false;
                }

                if (TeacherDisplayNameExists(teacher.TeacherID, teacher.DisplayName))
                {
                    msg = "Display name already exists";
                    return false;
                }

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

            //_dbEntities.SaveChanges();
            //msg = "Student details saved successfully";
            //return true;
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
                        msg = $"The teacher {teacher.DisplayName} is teaching a subject.";
                        return false;
                    }

                    //if (subject.SubjectCode.Any())
                    //{
                    //    msg = "This subject cannot be deleted";
                    //    return false;
                    //}

                    _dbEntities.Teachers.Remove(teacher);
                    _dbEntities.SaveChanges();
                    msg = $"The teacher {teacher.DisplayName} is removed successfully.";
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
        /// <summary>
        /// Check whether teacher is referenced
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>
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
                message = $"Cannot change status because {teacher.DisplayName} is teaching subject.";
                return false;
            }

            // Toggle the enable status
            teacher.IsEnable = !currentStatus;
            _dbEntities.SaveChanges();

            message = currentStatus ? "Disabled Successfully" : "Enabled Successfully";
            return true;
        }

        public IEnumerable<TeacherBO> SearchTeachers(string searchText, string searchCategory)
        {
            var teachers = GetTeachers();

            // Perform the search logic based on the selected category
            if (searchCategory == "TeacherRegNo")
            {
                teachers = teachers.Where(a => a.TeacherRegNo.ToUpper().Contains(searchText.ToUpper())).ToList();
            }
            else if (searchCategory == "DisplayName")
            {
                teachers = teachers.Where(a => a.DisplayName.ToUpper().Contains(searchText.ToUpper())).ToList(); ;
            }
            else if (searchCategory == "FirstName")
            {
                teachers = teachers.Where(a => a.FirstName.ToUpper().Contains(searchText.ToUpper())).ToList(); ;
            }
            else if (searchCategory == "LastName")
            {
                teachers = teachers.Where(a => a.LastName.ToUpper().Contains(searchText.ToUpper())).ToList();
            }
            else if (searchCategory == "Address")
            {
                teachers = teachers.Where(a => a.Address.ToUpper().Contains(searchText.ToUpper())).ToList(); ;
            }


            return teachers;
        }
    }
}

