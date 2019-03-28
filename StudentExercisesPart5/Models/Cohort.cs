using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SEWebApi.Model
{
    public class Cohort
    {
        public int Id { get; set; }
        [Required]

        [StringLength(11, MinimumLength = 5)]
        //[RegularExpression("^((?!^First Name$)[a-zA-Z '])+$", 
        // ErrorMessage = "Cohort name should be in the format of [Day|Evening] [number]")]
        public string Name { get; set; }
        public List<Student> StudentList { get; set; } = new List<Student>();
        public List<Instructor> InstructorList { get; set; } = new List<Instructor>();
    }
}
