using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tatoo.Azure.BlobStorage;

namespace CustomFeet.Controllers
{
    [Route("api/[controller]")]
    public class ImagesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }


        //[HttpPost]
        //[Route("api/[controller]/images")]
        //public async Task<IActionResult> Post(string bucketName, IList<IFormFile> files)
        //{
        //    var urls = new List<string>();
        //    var imageManager = new ImageManager();
        //    foreach (var file in files)
        //    {
                
        //        using (var fileStream = file.OpenReadStream())
        //        {
        //            var fileUrl = await imageManager.UploadImage(fileStream, bucketName, file.FileName);
        //           urls.Add(fileUrl);
        //        }


        //    }

        //    return Ok(new {FileUrl = urls });

        //}

        [HttpPost]
        [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
        public async Task<IActionResult> Post(string bucketName)
        {
            var urls = new List<string>();
            var imageManager = new ImageManager();
            foreach (var file in Request.Form.Files)
            {
                using (var fileStream = file.OpenReadStream())
                {
                    var fileUrl = await imageManager.UploadImage(fileStream, bucketName, file.FileName);
                    urls.Add(fileUrl);
                }
            }
            
            return Ok(new { FileUrl = urls });

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}