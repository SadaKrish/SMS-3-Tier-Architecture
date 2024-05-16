using SMS.Data;
using SMS.Models.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Student
{
    public class StudentBL: sms_dbEntities
    {
        private bool requiresConfirmation;
        /// <summary>
        /// Get the studnet list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StudentBO> GetAllStudent()
        {
            var allStudents = Students.Select(t => new StudentBO()
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

            }).OrderBy(s => s.StudentID).ToList();
            return allStudents;
        }
        /// <summary>
        /// Get the student details by its id
        /// </summary>
        /// <param name="studentID"></param>
        /// <returns></returns>
        public StudentBO GetStudentByID(long studentID)
        {
            var result = Students.Select(t => new StudentBO()
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
        /// add new student and edit the existing student
        /// As studnet is independent it the status can be changed
        /// </summary>
        /// <param name="student"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SaveStudent(StudentBO student, out string msg)
        {
            msg = "";
            bool existingStudent = Students.Any(s => s.StudentID == student.StudentID);
            //bool StudentWhoStudies = Student_Subject_Teacher_Allocation.Any(a => a.StudentID == student.StudentID);
            try
            {
                if (existingStudent)
                {
                    //if (StudentWhoStudies)
                    //{
                    //    msg = "Student " + student.DisplayName + " studies subject";
                    //    return false;
                    //}
                    var editStudent = Students.SingleOrDefault(s => s.StudentID == student.StudentID);
                    if (editStudent == null)
                    {
                        msg = "Unable to find the student for edit";
                        return false;
                    }
                    editStudent.StudentRegNo = student.StudentRegNo;
                    editStudent.FirstName = student.FirstName;
                    editStudent.MiddleName = student.MiddleName;
                    editStudent.LastName = student.LastName;
                    editStudent.DisplayName = student.DisplayName;
                    editStudent.Email = student.Email;
                    editStudent.Gender = student.Gender;
                    editStudent.DOB = student.DOB;
                    editStudent.Address = student.Address;
                    editStudent.ContactNo = student.ContactNo;
                    editStudent.IsEnable = student.IsEnable;
                    SaveChanges();
                    msg = "Student Details Updated Successfully";
                    return true;

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
                Students.Add(newStudent);
                SaveChanges();
                msg = "Student Added Successfully!";
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// Delet a student details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool DeleteStudent(long id, out string msg)
        {
            msg = "";
            requiresConfirmation = false;
            var student = Students.SingleOrDefault(t => t.StudentID == id);
            try
            {
                if (student != null)
                {
                    if (student.Student_Subject_Teacher_Allocation.Any())
                    {
                        requiresConfirmation = true;
                        msg = "The student is following a course.";
                        return false;
                    }

                    Students.Remove(student);
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
