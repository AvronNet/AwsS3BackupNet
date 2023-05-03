using System.Net.Http.Json;
using System.Net;
using AwsS3LifeBackup.Core.Communication.Files;
using AwsS3LifeBackup.API;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Amazon.S3;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Runtime;

namespace AwsS3LifeBackup.IntegrationTests.Scenarios
{
    [Collection("api")]
    public class FilesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public FilesControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAWSService<IAmazonS3>(new AWSOptions
                    {
                        DefaultClientConfig =
                        {
                            ServiceURL = "http://localhost:4566"
                        },
                        Credentials = new BasicAWSCredentials("test", "test")
                    });
                });
            }).CreateClient();

            Task.Run(CreateBucket).Wait();
        }

        private async Task CreateBucket()
        {
            var response = await _client.PostAsJsonAsync("api/bucket/create/testS3Bucket", "testS3Bucket");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to create test S3 bucket");
            }
        }

        [Fact]
        public async Task When_AddFiles_endpoint_is_hit_we_are_returned_ok_status()
        {
            var response = await UploadFileToS3Bucket();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<HttpResponseMessage> UploadFileToS3Bucket()
        {
            const string path = @"IntegrationTest.jpg";

            var file = File.Create(path);
            HttpContent fileStreamContent = new StreamContent(file);

            var formData = new MultipartFormDataContent
            {
                {fileStreamContent, "formFiles", "IntegrationTest.jpg" }
            };

            var response = await _client.PostAsync("api/files/testS3Bucket/add", formData);

            fileStreamContent.Dispose();
            formData.Dispose();

            return response;
        }

        [Fact]
        public async Task When_ListFiles_endpoint_is_hit_our_result_is_not_null()
        {
            await UploadFileToS3Bucket();

            var response = await _client.GetAsync("api/files/testS3Bucket/list");

            ListFilesResponse[] result;
            using (var content = response.Content.ReadAsStringAsync())
            {
                result = System.Text.Json.JsonSerializer.Deserialize<ListFilesResponse[]>(await content);
            }

            Assert.NotNull(result);
        }

        [Fact]
        public async Task When_DownloadFiles_endpoint_is_hit_we_are_returned_ok_status()
        {
            const string filename = @"IntegrationTest.jpg";

            await UploadFileToS3Bucket();

            var response = await _client.GetAsync($"api/files/testS3Bucket/download/{filename}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task When_DeleteFile_endpoint_is_hit_we_are_returned_ok_status()
        {
            const string filename = @"IntegrationTest.jpg";

            await UploadFileToS3Bucket();

            var response = await _client.DeleteAsync($"api/files/testS3Bucket/delete/{filename}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task When_AddJsonObject_endpoint_is_hit_we_are_returned_ok_status()
        {
            var jsonObjectRequest = new AddJsonObjectRequest
            {
                Id = Guid.NewGuid(),
                Data = "Test-Data",
                TimeSent = DateTime.UtcNow
            };

            var response = await _client.PostAsJsonAsync("api/files/testS3Bucket/addjsonobject/", jsonObjectRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
