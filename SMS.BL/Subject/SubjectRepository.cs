﻿/// <summary>
/// 
/// </summary>
/// <author>Sadakshini</author>
using SMS.BL.Subject.Interface;
using SMS.Data;
using SMS.Models.Subject;
using SMS.ViewModel.Subject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Subject
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly SMS_DBEntities _dbEntities;

        public SubjectRepository()
        {
            _dbEntities = new SMS_DBEntities();
        }
        public SubjectRepository(SMS_DBEntities dbEntities)
        {
            _dbEntities = dbEntities;
        }

        

        /// <summary>
        /// Get Teacher details based on status
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public IEnumerable<SubjectBO> GetSubjects(bool? isEnable = null)
        {
            var query = _dbEntities.Subjects.AsQueryable();

            if (isEnable.HasValue)
            {
                query = query.Where(s => s.IsEnable == isEnable.Value);
            }

            var subjects = query.ToList();

            return subjects.Select(s => new SubjectBO
            {
                SubjectID = s.SubjectID,
                SubjectCode = s.SubjectCode,
                Name = s.Name,
                IsEnable = s.IsEnable,
                IsAllocated = _dbEntities.Teacher_Subject_Allocation.Any(a => a.SubjectID == s.SubjectID)
            }).OrderBy(t => t.SubjectID).ToList();
        }


       

       /// <summary>
       /// Get subject by its ID
       /// </summary>
       /// <param name="subjectID"></param>
       /// <returns></returns>
        public SubjectBO GetSubjectByID(long subjectID)
        {
            var result = _dbEntities.Subjects.Select(s => new SubjectBO()
            {
                SubjectID = s.SubjectID,
                SubjectCode = s.SubjectCode,
                Name = s.Name,
                IsEnable = s.IsEnable

            }).Where(s => s.SubjectID == subjectID).FirstOrDefault();

            return result;
        }
        /// <summary>
        /// check the existence of SUbject code
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="subjectCode"></param>
        /// <returns></returns>
        public bool SubjectCodeExists(long subjectId, string subjectCode)
        {
            return _dbEntities.Subjects.Any(s => s.SubjectID != subjectId && s.SubjectCode == subjectCode);
        }
        /// <summary>
        /// Check ther exsitence of subject name
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="subjectName"></param>
        /// <returns></returns>
        public bool SubjectNameExists(long subjectId, string subjectName)
        {
            return _dbEntities.Subjects.Any(s => s.SubjectID != subjectId && s.Name == subjectName);
        }


        /// <summary>
        /// Save the subject record
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SaveSubject(SubjectBO subject, out string msg)
        {
            msg = "";

            bool existingSubject = _dbEntities.Subjects.Any(s => s.SubjectID == subject.SubjectID);

            bool subjectInUse = _dbEntities.Teacher_Subject_Allocation.Any(a => a.SubjectID == subject.SubjectID);

            try
            {
                if (existingSubject)
                {
                    var editSubject = _dbEntities.Subjects.SingleOrDefault(s => s.SubjectID == subject.SubjectID);

                    if (editSubject == null)
                    {
                        msg = "Unable to find the subject for edit";
                        return false;
                    }

                    //if (editSubject.IsEnable && subjectInUse)
                    //{
                        
                    //    msg = "Subject updated successfully!Status cannot be changed as the subject is in use.";
                    //    return true;
                    //}
                    //else
                    //{
                        // If IsEnable is false or not referenced, allow changing all properties
                        editSubject.SubjectCode = subject.SubjectCode;
                        editSubject.Name = subject.Name;
                        editSubject.IsEnable = subject.IsEnable;
                        _dbEntities.SaveChanges();
                        msg = "Subject updated successfully!";
                        return true;
                    //}
                }
                else
                {
                    // If it's a new subject, add it directly without checks
                    var newSubject = new SMS.Data.Subject();
                    newSubject.SubjectCode = subject.SubjectCode;
                    newSubject.Name = subject.Name;
                    newSubject.IsEnable = subject.IsEnable;
                    _dbEntities.Subjects.Add(newSubject);
                    _dbEntities.SaveChanges();
                    msg = "Subject added successfully!";
                    return true;
                }
            }
            catch (Exception error)
            {
                msg = error.Message;
                return false;
            }
        }
        /// <summary>
        /// Delete subject by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool DeleteSubject(long id, out string msg)
        {
            msg = "";
            var subject = _dbEntities.Subjects.SingleOrDefault(t => t.SubjectID == id);
            try
            {
                if (subject != null)
                {
                    if (subject.Teacher_Subject_Allocation.Any())
                    {
                        msg = $"The {subject.Name} is taking by a teacher.";
                        return false;
                    }

                    //if (subject.SubjectCode.Any())
                    //{
                    //    msg = "This subject cannot be deleted";
                    //    return false;
                    //}

                    _dbEntities.Subjects.Remove(subject);
                    _dbEntities.SaveChanges();
                    msg = $"The Subject {subject.Name} is removed successfully.";
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
        /// Check whether the subject is taught by a teacher or not
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public bool IsSubjectReferenced(int subjectId)
        {
            // Check if the subject is referenced in any teacher-subject allocations
            return _dbEntities.Teacher_Subject_Allocation.Any(tsa => tsa.SubjectID == subjectId);
        }
        public bool ToggleSubjectEnable(int subjectId, out string message)
        {
            var subject = _dbEntities.Subjects
                                  .SingleOrDefault(s => s.SubjectID == subjectId);

            if (subject == null)
            {
                message = "Subject not found.";
                return false;
            }

            bool currentStatus = subject.IsEnable;

            // If the current status is enabled, check if the subject is referenced in any related entities
            if (currentStatus && IsSubjectReferenced(subjectId))
            {
                message = $"Cannot change status because {subject.Name} is referenced in other entities.";
                return false;
            }

            // Toggle the enable status
            subject.IsEnable = !currentStatus;
            _dbEntities.SaveChanges();

            message = currentStatus ? "Disabled Successfully" : "Enabled Successfully";
            return true;
        }
    }
}
