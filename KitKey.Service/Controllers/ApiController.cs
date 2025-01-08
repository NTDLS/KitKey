using Microsoft.AspNetCore.Mvc;
using NTDLS.KitKey.Server;
using NTDLS.KitKey.Shared;

namespace KitKey.Service.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly KkServer _mqServer;

        public ApiController(KkServer mqServer)
        {
            _mqServer = mqServer;
        }

        [HttpPost("Upsert/{storeName}/{key}")]
        [Consumes("text/plain", "application/json")]
        public IActionResult Upsert(string storeName, string key, [FromBody] dynamic value)
        {
            _mqServer.Upsert(storeName, key, value.ToString());
            return Ok();
        }

        /// <summary>
        /// Creates a key-value store with the default configuration.
        /// </summary>
        /// <param name="storeName"></param>
        /// <returns></returns>
        [HttpPost("CreateStore/{storeName}")]
        public IActionResult CreateStore(string storeName)
        {
            _mqServer.CreateStore(new KkStoreConfiguration
            {
                StoreName = storeName
            });
            return Ok();
        }

        /// <summary>
        /// Creates a key-value store with a custom configuration.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost("CreateStore")]
        public IActionResult CreateStore([FromBody] KkStoreConfiguration config)
        {
            if (config == null || string.IsNullOrEmpty(config.StoreName))
            {
                return BadRequest("StoreName is required.");
            }

            _mqServer.CreateStore(config);

            return Ok(new { Message = "Key-value store created successfully.", config.StoreName });
        }

        [HttpDelete("PurgeStore/{storeName}")]
        public IActionResult Purge(string storeName)
        {
            _mqServer.PurgeStore(storeName);
            return Ok();
        }


        [HttpDelete("DeleteStore/{storeName}")]
        public IActionResult DeleteStore(string storeName)
        {
            _mqServer.DeleteStore(storeName);
            return Ok();
        }
    }
}
