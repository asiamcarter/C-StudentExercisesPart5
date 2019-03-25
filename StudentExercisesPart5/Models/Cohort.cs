using System;
using System.Collections.Generic;
using System.Text;

namespace SEWebApi.Model
{
    public class Cohort
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Student> StudentList { get; set; }
        public List<Instructor> InstructorList { get; set; }
    }
}
