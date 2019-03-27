using System;
using System.Collections.Generic;
using System.Text;

namespace SEWebApi.Model
{
    public class StudentExercise
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }
        public List<Exercise> ExerciseList { get; set; } = new List<Exercise>();
    }
}
