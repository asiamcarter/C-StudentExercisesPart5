using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using SEWebApi.Model;

namespace SEWebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ExerciseController(IConfiguration configuration)
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

        //2. Code for getting a list of exercises
        [HttpGet]
        public IEnumerable<Exercise> GetExercises(string include, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (include == "student")
                    {
                        cmd.CommandText = @"SELECT e.Name as ExerciseName,
                                            e.Language as ExerciseLanguage,
                                            e.Id as ExerciseId,
                                            s.Id as StudentId,
                                            s.FirstName as StudentFirstName,
                                            s.LastName as StudentLastName
                                            FROM Exercise as e
                                            LEFT JOIN StudentExercise                                          
                                            LEFT JOIN Student as s on se.StudentId = s.id
                                            WHERE e.Name LIKE '%{q}%' OR e.Language LIKE '%{q}%'";
                        SqlDataReader reader = cmd.ExecuteReader();
                        Dictionary<int, Exercise> exercises = new Dictionary<int, Exercise>();

                        while (reader.Read())
                        {
                            int ExerciseId = reader.GetInt32(reader.GetOrdinal("ExerciseId"));
                            if (!exercises.ContainsKey(ExerciseId))
                            {
                                Exercise newExercise = new Exercise
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                    Name = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                    Language = reader.GetString(reader.GetOrdinal("ExerciseLanguage"))
                                };
                                exercises.Add(ExerciseId, newExercise);
                            }

                            Exercise currentExercise = exercises[ExerciseId];
                            if (!reader.IsDBNull(reader.GetOrdinal("StudentId")))
                            {
                                if (!currentExercise.StudentList.Exists(x => x.Id == reader.GetInt32(reader.GetOrdinal("StudentId"))))
                                {

                                    currentExercise.StudentList.Add(
                                            new Student
                                            {
                                                Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                                                FirstName = reader.GetString(reader.GetOrdinal("StudentFirstName")),
                                                LastName = reader.GetString(reader.GetOrdinal("StudentLastName"))
                                            }
                                            );
                                }
                            }
                        }
                        reader.Close();
                        return exercises.Values.ToList();

                    } else 
                    {
                        
                        cmd.CommandText = $@"SELECT Id, Name, Language FROM Exercise 
                                             WHERE Name LIKE '%{q}%' OR Language LIKE '%{q}%'";
                        //C# does not work...something with the '#' symbol?
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<Exercise> exercises = new List<Exercise>();

                        while (reader.Read())
                        {
                            Exercise exercise = new Exercise
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Language = reader.GetString(reader.GetOrdinal("Language"))
                            };

                            exercises.Add(exercise);
                        }
                        reader.Close();
                        return exercises;

                    }
                
                }
            }
        }

        //3. Code for getting a single exercise
        [HttpGet("{id}", Name = "GetExercises")]
        public Exercise Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, Language 
                                        FROM Exercise 
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Exercise exercise = null;

                    if (reader.Read())
                    {
                        exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Language = reader.GetString(reader.GetOrdinal("Language"))
                        };
                    }
                    reader.Close();
                    return exercise;
                }
            }
        }

       // 4. Code for creating an exercise
         [HttpPost]
        public async Task<IActionResult> Post([FromBody] Exercise exercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Exercise (Name, Language)
                                        OUTPUT INSERTED.Id
                                        Values (@name, @language)";
                    cmd.Parameters.Add(new SqlParameter("@name", exercise.Name));
                    cmd.Parameters.Add(new SqlParameter("@language", exercise.Language));
                    int newId = (int)cmd.ExecuteScalar();
                    exercise.Id = newId;
                    return CreatedAtRoute("GetExercise", new { id = newId }, exercise);
                }
            }
        }

        // 5. Code for editing an exercise
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Exercise exercise)
        {
            using (SqlConnection conn = Connection)

            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())

                {
                    cmd.CommandText = @"UPDATE Exercise
                                        SET Name = @ExerciseName, Language =                         @ExerciseLanguage
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@ExerciseName", exercise.Name));
                    cmd.Parameters.Add(new SqlParameter("@ExerciseLanguage", exercise.Language));
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Exercise WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                   
                    cmd.ExecuteNonQuery();
                }
            }

        }
    }
}
