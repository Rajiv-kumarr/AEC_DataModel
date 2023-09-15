using Newtonsoft.Json.Linq;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HubInformation
{
    public class HubInfo
    {
        public static string hubName;

        ////////////////////////Get Hub details//////////////////////////////////////////////////////////
        public static async Task<Dictionary<string, string>> GetHubDetails(string tokenCode)
        {
            Dictionary<string, string> dictOfHubDetails = new Dictionary<string, string>();

            string graphQLQuery = @"  
                                    query GetHubs {
                                      hubs {
                                        pagination {
                                         cursor
                                        }
                                        results {
                                         name
                                         id
                                        }
                                      }
                                    }

        ";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer " + tokenCode);

                var queryData = new
                {
                    query = graphQLQuery
                };

                var jsonContent = new StringContent(
                    Newtonsoft.Json.JsonConvert.SerializeObject(queryData),
                    Encoding.UTF8,
                    "application/json"
                );

                try
                {
                    HttpResponseMessage response = await httpClient.PostAsync(FusionOAuth.OAuth._GraphQlApiUrlV1, jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Hub Details: " + responseBody);

                        JObject jsonResponse = JObject.Parse(responseBody);

                        var hubResults = jsonResponse["data"]["hubs"]["results"];
                        foreach (JToken hub in hubResults)
                        {
                            string hubId = hub["id"].ToString();
                            string hubName = hub["name"].ToString();
                            dictOfHubDetails.Add(hubId, hubName);

                            Console.WriteLine($"Hub ID: {hubId}, Hub Name: {hubName}\n");
                        }
                    }

                    else
                    {
                        Console.WriteLine($"Invalid http response for GetHubDetails: {response.StatusCode}");
                    }
                }

                catch (HttpException ex)
                {
                    Console.WriteLine($"get hub API request failed: {ex.Message}");
                }
            }
            return dictOfHubDetails;
        }

        ////////////////////////Get project details//////////////////////////////////////////////////////
        public static async Task<Dictionary<string, string>> GetProjectDetails(string tokenCode, string hubId)
        {
            Dictionary<string, string> dictOfProjectDetails = new Dictionary<string, string>();

            string graphQLQuery = @"
                                      query projects($hubId: ID!) {
                                        projects(hubId: $hubId) {
                                          pagination {
                                          cursor
                                        }
                                        results {
                                          id
                                          name
                                          alternativeRepresentations{
                                            externalProjectId
                                          }
                                        }
                                      }
                                    }
        ";

            var variables = new
            {
                hubId = hubId
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenCode);

                string serializedVariables = Newtonsoft.Json.JsonConvert.SerializeObject(variables);
                string fullQuery = "{\"query\":\"" + graphQLQuery.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r") + "\",\"variables\":" + serializedVariables + "}";
                StringContent content = new StringContent(fullQuery, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await httpClient.PostAsync(FusionOAuth.OAuth._GraphQlApiUrlV1, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Project Details: " + responseBody);

                        JObject jsonResponse = JObject.Parse(responseBody);
                        var results = jsonResponse["data"]["projects"]["results"];

                        foreach (JToken project in results)
                        {
                            string projectId = project["id"].ToString();
                            string projectName = project["name"].ToString();

                            dictOfProjectDetails.Add(projectId, projectName);

                            Console.WriteLine($"Project ID : {projectId}, project Name: {projectName}\n");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Invalid http response for GetProjectDetails: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"get project API request failed: {ex.Message}");
                }
            }
            return dictOfProjectDetails;
        }
    }
}