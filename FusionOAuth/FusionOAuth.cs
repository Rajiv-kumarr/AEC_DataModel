using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FusionOAuth
{
    public class OAuth
    {
        private const string _clientId = "G1XCfN8VKBuGaXeDLkhZtFh0pXQLTD5H";
        private const string _clientSecret = "BDZGDmyGoGrosjAD";
        private const string _authUrl = "https://developer.api.autodesk.com/authentication/v2/authorize?";  //end point for authorization code generation server //
        private const string _tokenAuthUrl = "https://developer.api.autodesk.com/authentication/v2/token";  //end point for token code generation server //
        private const string _redirectUri = "http://localhost:8080/";
        public const string _GraphQlApiUrlV1 = "https://developer.api.autodesk.com/aeccloudinformationmodel/2022-11/graphql";

        private static async Task<string> waitForCallbackUrlAsync() //wait for user to give access //
        {
            using (var listener = new System.Net.HttpListener())
            {
                listener.Prefixes.Add("http://localhost:8080/");
                listener.Start();

                // Wait for the callback URL //
                var context = await listener.GetContextAsync();
                var request = context.Request;
                return request.Url.ToString();
            }
        }

        //////////////////// Generate the Authorization code(for 3 legged authentication) //////////////////////////////////
        public static async Task<string> GenerateAuthCode()
        {
            string authAppUrl = _authUrl + "response_type=code&" + "client_id=" + _clientId + "&" + "redirect_uri=" + _redirectUri + "&scope=data:read%20data:search%20data:write ";
            string authorizationCode = "";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(authAppUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = authAppUrl,
                            UseShellExecute = true
                        });

                        Console.WriteLine("Waiting for the user to grant permission...\n");
                        string callbackUrl = await waitForCallbackUrlAsync();

                        authorizationCode = HttpUtility.ParseQueryString(new Uri(callbackUrl).Query).Get("code");
                    }
                    else
                    {
                        Console.WriteLine("Request failed with status code: " + response.StatusCode);
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine("An error Occured " + e.Message);
                }
            }
            return authorizationCode;
        }

        //////////////////////// generating the Token code(for 3 legged authentication) ////////////////////////////////////////
        public static async Task<string> GenerateTokenCode3Legged(string authorizationCode)
        {
            string tokenCode = "";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_tokenAuthUrl);

                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}"));

                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);

                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", authorizationCode),
                    new KeyValuePair<string, string>("redirect_uri", _redirectUri)
                });

                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, _tokenAuthUrl)
                    {
                        Content = formData
                    };
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    HttpResponseMessage response = await httpClient.SendAsync(request);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("Access Token Details: " + responseContent);

                    if (responseContent.Contains("access_token"))
                    {
                        JObject responseJson = JObject.Parse(responseContent);

                        tokenCode = (string)responseJson["access_token"];
                        Console.WriteLine($"Access Token1: {tokenCode}");
                    }
                    else
                    {
                        Console.WriteLine("Token is not generated From the given Access Code!");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Request error: " + e.Message);
                }
            }
            return tokenCode;
        }

        //////////////////////// generating the Token code(for 2 legged authentication) ////////////////////////////////////////
        public static async Task<string> GenerateTokenCode2Legged()
        {
            string tokenCode2Legged = null;

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_tokenAuthUrl);

                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}"));

                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);

                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Accept", "application/json"),
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("scope", "data:read data:search data:write data:create")
                });
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, _tokenAuthUrl)
                    {
                        Content = formData
                    };
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    HttpResponseMessage response = await httpClient.SendAsync(request);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("2 legged Access Token Details: " + responseContent);

                    if (responseContent.Contains("access_token"))
                    {
                        JObject responseJson = JObject.Parse(responseContent);

                        tokenCode2Legged = (string)responseJson["access_token"];
                        Console.WriteLine($"2 legged Access Token: {tokenCode2Legged}");

                    }
                    else
                    {
                        Console.WriteLine("2 legged Token code error: Token is not generated Properly!");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Request error: " + e.Message);
                }
            }
            return tokenCode2Legged;
        }
    }
}
