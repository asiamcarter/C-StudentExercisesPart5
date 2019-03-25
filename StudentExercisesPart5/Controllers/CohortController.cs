﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEWebApi.Model;

namespace StudentExercisesPart5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CohortController : ControllerBase
    {
        public SqlConnection Connection

        {
            get
            {
                string connectionSTring = "Server=DESKTOP-7FFQBEO\\SQLEXPRESS; Database=StudentExerciseDB; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
                return new SqlConnection(connectionSTring);
            }
        }
        // GET: api/Cohort
        [HttpGet]
        public List<Cohort> GetCohorts()
        {
            using (SqlConnection conn = Connection)

            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"select s.id as StudentId,
                                       s.FirstName,
                                       s.LastName,
                                       s.SlackHandle,
                                       s.CohortId,
                                       c.[Name] as CohortName,
                                       e.id as ExerciseId,
                                       e.[name] as ExerciseName,
                                       e.[Language]
                                       from student s
                                       left join Cohort c on s.CohortId = c.id
                                       left join StudentExercise se on s.id = se.studentid
                                       left join Exercise e on se.exerciseid = e.id;";
                    SqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, Cohort> cohorts = new Dictionary<int, Cohort>();
                    while (reader.Read())
                    {
                        int CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"));
                        if (!cohorts.ContainsKey(CohortId))
                        {
                            Cohort newCohort = new Cohort
                            {
                                Id = CohortId,
                                Name = reader.GetString(reader.GetOrdinal("CohortName")),
                                StudentList = new List<Student>()
                                
                            };

                            cohorts.Add(CohortId, newCohort);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("StudentId")))
                        {
                            Cohort currentStudent = cohorts[CohortId];
                            currentStudent.StudentList.Add(
                                new Student
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),                 
                                }
                            );
                        }else
                        {

                        }
                    }

                    reader.Close();

                    return cohorts.Values.ToList();
                }
            }
        }

        // GET: api/Cohort/5
        [HttpGet("{id}", Name = "GetCohorts")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Cohort
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Cohort/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
