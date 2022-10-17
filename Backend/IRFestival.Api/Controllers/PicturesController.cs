using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using IRFestival.Api.Common;
using IRFestival.Api.Model;
using IRFestival.Api.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private BlobUtility BlobUtility { get; }

        public PicturesController(BlobUtility blobUtility, IConfiguration configuration)
        {
            BlobUtility = blobUtility;
            Configuration = configuration;
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

        [HttpPost("Upload")]
        [Authorize]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AppSettingsOptions))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> PostPictures(IFormFile file)
        {
            BlobContainerClient Container = BlobUtility.GetPicturesContainer();
            var filename = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{HttpUtility.UrlPathEncode(file.FileName)}";
            await Container.UploadBlobAsync(filename, file.OpenReadStream());

            await using (var client = new ServiceBusClient(Configuration.GetConnectionString("ServiceBusSenderConnection")))
            {
                // create a sender for the queue
                ServiceBusSender sender = client.CreateSender(Configuration.GetValue<string>("QueueNameMails"));

                // create a message that we can send
                ServiceBusMessage message = new ServiceBusMessage($"The picture {filename} was uploaded! Send a fictional mail to me@you.us");

                //ResponseModel response = new ResponseModel()
                //{
                //    MailAddress = "me@you.us",
                //    Message = $"The picture {filename} was uploaded!"
                //};

                // send the message
                await sender.SendMessageAsync(message);
            }

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
