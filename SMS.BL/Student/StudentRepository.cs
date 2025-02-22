﻿/// <summary>
/// 
/// </summary>
/// <author>Sadakshini</author>
using SMS.BL.Student.Interface;
using SMS.Data;
using SMS.Models.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SMS.BL.Student
{
    public  class StudentRepository:IStudentRepository
    {
        private readonly SMS_DBEntities _dbEntities;

        public StudentRepository()
        {
            _dbEntities = new SMS_DBEntities();
        }
        public StudentRepository(SMS_DBEntities dbEntities)
        {
            _dbEntities = dbEntities;
        }
        /// <summary>
        /// Get the student list according to the status
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public IEnumerable<StudentBO> GetStudents(bool? isEnable = null)
        {
            var query = _dbEntities.Students.AsQueryable();

            if (isEnable.HasValue)
            {
                query = query.Where(s => s.IsEnable == isEnable.Value);
            }

            var students = query.ToList();

            return students.Select(s => new StudentBO
            {
                StudentID = s.StudentID,
                StudentRegNo = s.StudentRegNo,
                FirstName = s.FirstName,
                MiddleName = s.MiddleName,
                LastName = s.LastName,
                DisplayName = s.DisplayName,
                Email = s.Email,
                Gender =s.Gender,
                DOB = s.DOB,
                Address = s.Address,
                ContactNo = s.ContactNo,
                IsEnable = s.IsEnable,
                IsAllocated = _dbEntities.Student_Subject_Teacher_Allocation.Any(a => a.StudentID == s.StudentID)
            }).ToList();
        }
        /// <summary>
        /// Get student by id
        /// </summary>
        /// <param name="studentID"></param>
        /// <returns></returns>
        public StudentBO GetStudentByID(long studentID)
        {
            var result =_dbEntities.Students.Select(t => new StudentBO()
            {
                StudentID = t.StudentID,
                StudentRegNo = t.StudentRegNo,
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

            }).Where(t => t.StudentID == studentID).FirstOrDefault();
            return result;
        }
        /// <summary>
        /// Check the existence of student reg no
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="regNo"></param>
        /// <returns></returns>
        public bool StudentRegNoExists(long studentId,string regNo)
        {
            return _dbEntities.Students.Any(s => s.StudentID != studentId && s.StudentRegNo == regNo);
        }
        /// <summary>
        /// check the existence of display name
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public bool StudentDisplayNameExists(long studentId, string displayName)
        {
            return _dbEntities.Students.Any(s => s.StudentID != studentId && s.DisplayName == displayName);
        }
        /// <summary>
        /// save and edit student detials
        /// </summary>
        /// <param name="student"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SaveStudent(StudentBO student, out string msg)
        {
            msg = "";

            var existingStudent = _dbEntities.Students.SingleOrDefault(s => s.StudentID == student.StudentID);
            if (existingStudent != null)
            {
                // Editing existing student
                if (existingStudent.StudentRegNo != student.StudentRegNo && StudentRegNoExists(student.StudentID,student.StudentRegNo))
                {
                    msg = "Registration No already exists";
                    return false;
                }

                if (existingStudent.DisplayName != student.DisplayName && StudentDisplayNameExists(student.StudentID,student.DisplayName))
                {
                    msg = "Display name already exists";
                    return false;
                }

                // Update fields
                existingStudent.StudentRegNo = student.StudentRegNo;
                existingStudent.FirstName = student.FirstName;
                existingStudent.MiddleName = student.MiddleName;
                existingStudent.LastName = student.LastName;
                existingStudent.DisplayName = student.DisplayName;
                existingStudent.Email = student.Email;
                existingStudent.Gender = student.Gender;
                existingStudent.DOB = student.DOB;
                existingStudent.Address = student.Address;
                existingStudent.ContactNo = student.ContactNo;
                existingStudent.IsEnable = student.IsEnable;
                _dbEntities.SaveChanges();
                msg = "Student details updated successfully";
                return true;
            }
            else
            {
                // Adding new student
                if (StudentRegNoExists(student.StudentID,student.StudentRegNo))
                {
                    msg = "Registration No already exists";
                    return false;
                }

                if (StudentDisplayNameExists(student.StudentID,student.DisplayName))
                {
                    msg = "Display name already exists";
                    return false;
                }

                var newStudent = new SMS.Data.Student();

                newStudent.StudentRegNo = student.StudentRegNo;
                newStudent.FirstName = student.FirstName;
                newStudent.MiddleName = student.MiddleName;
                newStudent.LastName = student.LastName;
                newStudent.DisplayName = student.DisplayName;
                newStudent.Email = student.Email;
                newStudent.Gender = student.Gender;
                newStudent.DOB = student.DOB;
                newStudent.Address = student.Address;
                newStudent.ContactNo = student.ContactNo;
                newStudent.IsEnable = student.IsEnable;

                _dbEntities.Students.Add(newStudent);
                _dbEntities.SaveChanges();
                msg = "Student added successfully!";
                return true;

            }

            //_dbEntities.SaveChanges();
            //msg = "Student details saved successfully";
            //return true;
        }
    
        /// <summary>
        /// Check if the student allocated with teacher and subject
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public bool IsStudentReferenced(long studentId)
        {
            return _dbEntities.Student_Subject_Teacher_Allocation.Any(tsa => tsa.StudentID == studentId);
        }
        /// <summary>
        /// Delete the student record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <param name="requiresConfirmation"></param>
        /// <returns></returns>
        public bool DeleteStudent(long id, out string msg, out bool requiresConfirmation)
        {
            msg = "";
            requiresConfirmation = false;

            var student = _dbEntities.Students.SingleOrDefault(s => s.StudentID == id);
            try
            {
                if (student != null)
                {
                    if (IsStudentReferenced(id))
                    {
                        requiresConfirmation = true;
                        msg = $"The student {student.DisplayName} is following a course.";
                        return false;
                    }

                    _dbEntities.Students.Remove(student);
                    _dbEntities.SaveChanges();
                    msg = $"The student {student.DisplayName} is removed successfully.";
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
        /// <summary>
        /// Change the status of a student
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool ToggleStudentEnable(int studentId, out string message)
        {
            var student = _dbEntities.Students.SingleOrDefault(s => s.StudentID == studentId);

            if (student == null)
            {
                message = "Student not found.";
                return false;
            }

            bool currentStatus = student.IsEnable;

            // If the current status is enabled, check if the student is referenced in any related entities
            if (currentStatus && IsStudentReferenced(studentId))
            {
                message = $"Warning: {student.DisplayName} is referenced in other entities. Status changed successfully.";
            }
            else
            {
                message = currentStatus ? "Disabled Successfully" : "Enabled Successfully";
            }

            // Toggle the enable status
            student.IsEnable = !currentStatus;
            _dbEntities.SaveChanges();

            return true;
        }

        public IEnumerable<StudentBO> SearchStudents(string searchText, string searchCategory)
        {
            var students = GetStudents();

            // Perform the search logic based on the selected category
            if (searchCategory == "StudentRegNo")
            {
                students = students.Where(a => a.StudentRegNo.ToUpper().Contains(searchText.ToUpper())).ToList();
            }
            else if (searchCategory == "DisplayName")
            {
                students = students.Where(a => a.DisplayName.ToUpper().Contains(searchText.ToUpper())).ToList(); ;
            }
            else if (searchCategory == "FirstName")
            {
                students = students.Where(a => a.FirstName.ToUpper().Contains(searchText.ToUpper())).ToList(); ;
            }
            else if (searchCategory == "LastName")
            {
                students = students.Where(a => a.LastName.ToUpper().Contains(searchText.ToUpper())).ToList();
            }
            else if (searchCategory == "Address")
            {
                students = students.Where(a => a.Address.ToUpper().Contains(searchText.ToUpper())).ToList(); ;
            }
            

            return students;
        }
    }
}
