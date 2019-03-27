using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEWebApi.Model
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public List<Student> StudentList { get; set; } = new List<Student>();
    }
}


