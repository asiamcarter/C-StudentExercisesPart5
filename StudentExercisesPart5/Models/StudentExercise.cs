using System;
using System.Collections.Generic;
using System.Text;

namespace StudentExercises_v604.Model
{
    class StudentExercise
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }
    }
}
