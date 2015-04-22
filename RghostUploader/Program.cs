using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace RghostUploader
{
    class Program
    {
        static void Main()
        {
            const string path = "testtesttest.txt";
            File.WriteAllText(path, "Hello world from RghostUploader. Generated at " + DateTime.Now.ToString("u"));
            Uri fileUri = UploadFile(path);
            Console.WriteLine("Uploaded file URI = {0}", fileUri);
        }

        private static Uri UploadFile(string path)
        {
            using (var client = new HttpClient {BaseAddress = new Uri("http://kaon.rghost.ru/")})
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");

                using (var requestContent = new MultipartFormDataContent())
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    requestContent.Add(new ByteArrayContent(bytes), "file", Path.GetFileName(path));

                    HttpResponseMessage postResponse = client.PostAsync("files", requestContent).Result;
                    if (postResponse.StatusCode != HttpStatusCode.OK)
                        throw new Exception("Invalid response code = " + postResponse.StatusCode);
                    if (postResponse.RequestMessage.RequestUri == new Uri("http://rghost.ru/"))
                        throw new Exception("Server rejected upload request");
                    return postResponse.RequestMessage.RequestUri;
                }
            }
        }
    }
}
