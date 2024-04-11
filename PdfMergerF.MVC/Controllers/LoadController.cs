using Microsoft.AspNetCore.Mvc;


namespace PdfMergerF.MVC.Controllers
{
    public class LoadController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
       

        public LoadController(IHttpClientFactory clFactory)
        {
            _clientFactory = clFactory;
            
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> filesToUpload)
        {
            HttpClient client= _clientFactory.CreateClient();
            MultipartFormDataContent datas = new MultipartFormDataContent();
            List<string> extensions=new List<string>();
                foreach (IFormFile file in filesToUpload)
                {
                StreamContent fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "ifiles",
                        FileName = file.FileName
                    };
                    datas.Add(fileContent);
                    string actext=Path.GetExtension(file.FileName);
                if (actext.Equals(".jpeg") || actext.Equals(".jpg")||actext.Equals(".png"))
                {
                    extensions.Add(".imgext");
                }
                else if(actext.Equals(".pdf"))
                    extensions.Add(Path.GetExtension(file.FileName));

                }
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7078/api/PDF/UploadFiles") 
            { 
                Content = datas
            };
            HttpResponseMessage response=await client.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                bool allPdfs = extensions.All(v => v.Equals(".pdf"));
                bool allImg = extensions.All(v => v.Equals(".imgext"));
                bool mixed = extensions.Contains(".pdf") && extensions.Contains(".imgext");

                if (allImg)
                {//img
                    string url = "https://localhost:7078/api/PDF/IfYouHadImages";
                    return await MakeReq(url, client);
                }
                else if(allPdfs)
                {//pdf
                    string url = "https://localhost:7078/api/PDF/IfYouHadPDFS";
                    return await MakeReq(url, client);
                }
                else if(mixed)
                {
                    string url = "https://localhost:7078/api/PDF/IfYouHadMix";
                    return await MakeReq(url, client);
                }
                else
                {
                    return View();
                }   
            }
            else
            {
                //bad resp
                return View();
            }
            
            //https://localhost:7078/api/PDF
        }

        [HttpGet]
        public IActionResult UploadFiles()
        {

            return View();
        }
        private async Task<IActionResult> MakeReq(string url,HttpClient client)
        {
            HttpRequestMessage req2 = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage resp2 = await client.SendAsync(req2);
            if (resp2.IsSuccessStatusCode)
            {
                Stream fileStream = await resp2.Content.ReadAsStreamAsync();
                string fileExtension = ".pdf";
                return File(fileStream, "application/octet-stream", "merged" + fileExtension);
            }
            else
            {
                return View();
            }
        }
    }
}
