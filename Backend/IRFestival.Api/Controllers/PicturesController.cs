using Azure.Storage.Blobs;
using IRFestival.Api.Common;
using IRFestival.Api.Options;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private BlobUtility BlobUtility { get; }

        public PicturesController(BlobUtility blobUtility)
        {
            BlobUtility = blobUtility;
        }

        //[HttpGet]
        //[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string[]))]
        //public async Task<ActionResult> GetAllPictures()
        //{
        //    var container = BlobUtility.GetPicturesContainer();
        //    var result = container.GetBlobs()
        //        .Select(blob => BlobUtility.GetSasUri(container, blob.Name))
        //        .ToArray();

        //    return Ok(result);
        //}

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AppSettingsOptions))]
        public async Task<ActionResult> PostPictures(IFormFile file)
        {
            BlobContainerClient Container = BlobUtility.GetPicturesContainer();
            var filename = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{HttpUtility.UrlPathEncode(file.FileName)}";
            await Container.UploadBlobAsync(filename, file.OpenReadStream());

            return Ok();
        }

        [HttpGet]
        public string[] GetAllPictureUrls()
        {
            var container = BlobUtility.GetThumbsContainer();
            return container.GetBlobs()
                            .Select(blob => BlobUtility.GetSasUri(container, blob.Name))
                            .ToArray();
        }

    }
}
