using SMS.Data;
using SMS.Models.Subject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Subject
{
    public class SubjectBL: SMS_DBEntities
    {
        /// <summary>
        /// Retrieving the subject list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SubjectBO> GetAllSubject()
        {

            var allSubjects = Subjects.Select(s => new SubjectBO()
            {
                SubjectID = s.SubjectID,
                SubjectCode = s.SubjectCode,
                Name = s.Name,
                IsEnable = s.IsEnable

            }).OrderBy(t => t.SubjectID).ToList();

            return allSubjects;
        }
        /// <summary>
        /// Get the subject by its id
        /// </summary>
        /// <param name="subjectID"></param>
        /// <returns></returns>
        public SubjectBO GetSubjectByID(long subjectID)
        {
            var result = Subjects.Select(s => new SubjectBO()
            {
                SubjectID = s.SubjectID,
                SubjectCode = s.SubjectCode,
                Name = s.Name,
                IsEnable = s.IsEnable

            }).Where(s => s.SubjectID == subjectID).FirstOrDefault();

            return result;
        }


        /// <summary>
        /// Subject Add new and edit
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="msg"></param>
        /// <returns></returns>

        public bool SaveSubject(SubjectBO subject, out string msg)
        {
            msg = "";

            bool existingSubject = Subjects.Any(s => s.SubjectID == subject.SubjectID);

            bool subjectInUse = Teacher_Subject_Allocation.Any(a => a.SubjectID == subject.SubjectID);

            try
            {
                if (existingSubject)
                {
                    var editSubject = Subjects.SingleOrDefault(s => s.SubjectID == subject.SubjectID);

                    if (editSubject == null)
                    {
                        msg = "Unable to find the subject for edit";
                        return false;
                    }

                    if (editSubject.IsEnable && subjectInUse)
                    {
                        // If IsEnable is true and referenced, only allow changing SubjectCode and Name
                        editSubject.SubjectCode = subject.SubjectCode;
                        editSubject.Name = subject.Name;
                        SaveChanges();
                        msg = "Subject updated successfully! IsEnable cannot be changed as the subject is referenced.";
                        return true;
                    }
                    else
                    {
                        // If IsEnable is false or not referenced, allow changing all properties
                        editSubject.SubjectCode = subject.SubjectCode;
                        editSubject.Name = subject.Name;
                        editSubject.IsEnable = subject.IsEnable;
                        SaveChanges();
                        msg = "Subject updated successfully!";
                        return true;
                    }
                }
                else
                {
                    // If it's a new subject, add it directly without checks
                    var newSubject = new SMS.Data.Subject();
                    newSubject.SubjectCode = subject.SubjectCode;
                    newSubject.Name = subject.Name;
                    newSubject.IsEnable = subject.IsEnable;
                    Subjects.Add(newSubject);
                    SaveChanges();
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
        /// Delete subject if it is not refernced
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>

        public bool DeleteSubject(long id, out string msg)
        {
            msg = "";
            var subject = Subjects.SingleOrDefault(t => t.SubjectID == id);
            try
            {
                if (subject != null)
                {
                    if (subject.Teacher_Subject_Allocation.Any())
                    {
                        msg = "Cannot delete subject because it is referenced in other entities";
                        return false;
                    }

                    //if (subject.SubjectCode.Any())
                    //{
                    //    msg = "This subject cannot be deleted";
                    //    return false;
                    //}

                    Subjects.Remove(subject);
                    SaveChanges();
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

    }
}
