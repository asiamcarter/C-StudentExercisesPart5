﻿using SEWebApi.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWebApi.Model
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SlackHandle { get; set; }
        public int CohortId { get; set; }
        public Cohort cohort { get; set; }
        public List<Exercise> ExerciseList { get; set; } = new List<Exercise>();
    }
}
