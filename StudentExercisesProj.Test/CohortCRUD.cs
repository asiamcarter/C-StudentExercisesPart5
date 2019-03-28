using Newtonsoft.Json;
using SEWebApi.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StudentExercisesProj.Test
{
    public class CohortCRUD
    {
        [Fact]
        public async Task Test_Get_All_Cohorts()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/cohort");
                string responseBody = await response.Content.ReadAsStringAsync();
                var CohortList = JsonConvert.DeserializeObject<List<Cohort>>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(CohortList.Count > 0);
            }

        }

        [Fact]
        public async Task Test_Get_A_Cohort()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/cohort/3");
                string responseBody = await response.Content.ReadAsStringAsync();
                var Cohort = JsonConvert.DeserializeObject<List<Cohort>>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(Cohort.Count == 1);
            }

        }

        [Fact]
        public async Task Test_Insert_A_Cohort()
        {
            using (var client = new APIClientProvider().Client)
            {

                Cohort Cohort32 = new Cohort
                {
                    Name = "Cohort32"    
                };

                var Cohort32AsJSON = JsonConvert.SerializeObject(Cohort32);
                var response = await client.PostAsync(
                    "/api/cohort",
                    new StringContent(Cohort32AsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var newCohort32 = JsonConvert.DeserializeObject<Cohort>(responseBody);
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Cohort32", newCohort32.Name);

            }
        }

        [Fact]
        public async Task Test_Modify_Cohort()
        {
            // New last name to change to and test
            string newCohortName = "Cohort40";

            using (var client = new APIClientProvider().Client)
            {              
                Cohort modifiedCohort = new Cohort
                {
                    Id = 8,
                    Name = newCohortName
                };
                var modifiedCohortAsJSON = JsonConvert.SerializeObject(modifiedCohort);

                var response = await client.PutAsync(
                    "/api/cohort/8",
                    new StringContent(modifiedCohortAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getCohort = await client.GetAsync("/api/cohort/8");
                getCohort.EnsureSuccessStatusCode();

                string getCohortBody = await getCohort.Content.ReadAsStringAsync();

                Cohort newCohort = JsonConvert.DeserializeObject<Cohort>(getCohortBody);

                Assert.Equal(HttpStatusCode.OK, getCohort.StatusCode);
                Assert.Equal(newCohortName, newCohort.Name);
            }
        }
        [Fact]
        public async Task Test_Delete_Cohort()
        {

            using (var client = new APIClientProvider().Client)
            {

                var response = await client.DeleteAsync("/api/cohort/9");


                string responseBody = await response.Content.ReadAsStringAsync();
                var Cohort = JsonConvert.DeserializeObject<Cohort>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            }

        }

    }
}
