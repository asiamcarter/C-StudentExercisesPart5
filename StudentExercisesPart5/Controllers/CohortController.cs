using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SEWebApi.Model;

namespace StudentExercisesPart5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CohortController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public CohortController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: api/Cohort
        [HttpGet]
        public List<Cohort> GetAllCohorts(string q)
        {
            using (SqlConnection conn = Connection)

            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT c.id AS CohortId, c.Name as CohortName,
                                        s.Id AS StudentId, s.FirstName AS StudentFirstName, s.LastName                AS StudentLastName, s.SlackHandle as StudentSlackHandle,
                                          i.Id AS InstructorId, i.FirstName AS InstructorFirstName,                     i.LastName AS InstructorLastName, i.SlackHandle as                            InstructorSlackHandle
                                            FROM Cohort c
                                            LEFT JOIN Student as s ON s.CohortId = c.id
                                            LEFT JOIN Instructor as i ON i.CohortId = c.id
                                            WHERE c.Name LIKE '%{q}%';";
                
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
                                //StudentList = new List<Student>(),
                                //InstructorList = new List<Instructor>()
                            };

                            cohorts.Add(CohortId, newCohort);
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("CohortId")))
                        {
                            Cohort currentCohort = cohorts[CohortId];
                            if (!reader.IsDBNull(reader.GetOrdinal("InstructorId")))
                            {
                                if (!currentCohort.StudentList.Exists(x => x.Id == reader.GetInt32(reader.GetOrdinal("StudentId"))))
                                {
                                    currentCohort.StudentList.Add(
                                    new Student
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                    }
                                );
                                }
                            }
                        
                            
                            if (!reader.IsDBNull(reader.GetOrdinal("InstructorId")))
                            {
                                if (!currentCohort.InstructorList.Exists(x => x.Id == reader.GetInt32(reader.GetOrdinal("InstructorId"))))

                                {
                                    currentCohort.InstructorList.Add(
                                        new Instructor
                                        {
                                            Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                                            FirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                                            LastName = reader.GetString(reader.GetOrdinal("InstructorLastName"))
                                        }
                                    );
                                }
                   
                            }
                        }
                    }
                    reader.Close();
                    return cohorts.Values.ToList();
                }
            }
        }

        // GET: api/Cohort/5
        [HttpGet("{id}", Name = "GetCohort")]
        public async Task<IActionResult> GetCohort(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.id AS CohortId, c.Name as CohortName,
                                        s.Id AS StudentId, s.FirstName AS StudentFirstName, s.LastName                AS StudentLastName, s.SlackHandle as StudentSlackHandle,
                                          i.Id AS InstructorId, i.FirstName AS InstructorFirstName,                   i.LastName AS InstructorLastName, i.SlackHandle as                          InstructorSlackHandle
                                        FROM Cohort c
                                        LEFT JOIN Student as s ON s.CohortId = c.id
                                        LEFT JOIN Instructor as i ON i.CohortId = c.id
                                        WHERE c.Id = @Id";
                    cmd.Parameters.Add(new SqlParameter("@Id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Cohort cohort = null;
                    while (reader.Read())
                    {

                        cohort = new Cohort()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Name = reader.GetString(reader.GetOrdinal("CohortName")),
                            StudentList = new List<Student>(),
                            InstructorList = new List<Instructor>()
                        };



                        if (!reader.IsDBNull(reader.GetOrdinal("CohortId")))
                        {

                            if (!reader.IsDBNull(reader.GetOrdinal("StudentId")))
                            {
                                if (!cohort.StudentList.Exists(x => x.Id == reader.GetInt32(reader.GetOrdinal("StudentId"))))
                                {
                                    cohort.StudentList.Add(
                                    new Student
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                        FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("StudentLastName")),
                                    }
                                );
                                }
                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("InstructorId")))
                            {
                                if (!cohort.InstructorList.Exists(x => x.Id == reader.GetInt32(reader.GetOrdinal("InstructorId"))))

                                {
                                    cohort.InstructorList.Add(
                                        new Instructor
                                        {
                                            Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                                            FirstName = reader.GetString(reader.GetOrdinal("InstructorFirstName")),
                                            LastName = reader.GetString(reader.GetOrdinal("InstructorLastName"))
                                        }
                                    );
                                }
                            }
                        }
                    }
                    
                    reader.Close();
                    //return cohorts.Values.ToList();  
                    return Ok(cohort);
                }
            }                  
        }
    
        // POST: api/Cohort
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cohort newCohort)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" INSERT INTO Cohort (Name)
                                         OUTPUT INSERTED.Id
                                         Values(@CohortName)";
                    cmd.Parameters.Add(new SqlParameter("@CohortName", newCohort.Name));

                    int newId = (int)cmd.ExecuteScalar();
                    newCohort.Id = newId;
                    return CreatedAtRoute("GetCohort", new { id = newId }, newCohort);

                }
            }
        }

        // PUT: api/Cohort/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Cohort cohort)
        {
            using (SqlConnection conn = Connection)

            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Cohort 
                                        SET Name = @CohortName
                                        WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@CohortName", cohort.Name));
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    return NoContent();
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Cohort WHERE id = @id;";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
