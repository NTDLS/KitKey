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

        [HttpPost("Enqueue/{queueName}/{assemblyQualifiedTypeName}")]
        [Consumes("text/plain", "application/json")]
        public IActionResult Enqueue(string queueName, string assemblyQualifiedTypeName, [FromBody] dynamic messageJson)
        {
            string jsonText = messageJson.ToString();
            _mqServer.Upsert(queueName, assemblyQualifiedTypeName, jsonText);
            return Ok();
        }

        /// <summary>
        /// Creates a queue with the default configuration.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        [HttpPost("CreateQueue/{queueName}")]
        public IActionResult CreateQueue(string queueName)
        {
            _mqServer.CreateStore(new KkStoreConfiguration
            {
                StoreName = queueName
            });
            return Ok();
        }

        /// <summary>
        /// Creates a queue with a custom configuration.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost("CreateQueue")]
        public IActionResult CreateQueue([FromBody] KkStoreConfiguration config)
        {
            if (config == null || string.IsNullOrEmpty(config.StoreName))
            {
                return BadRequest("QueueName is required.");
            }

            _mqServer.CreateStore(config);

            return Ok(new { Message = "Queue created successfully.", config.StoreName });
        }

        [HttpDelete("Purge/{queueName}")]
        public IActionResult Purge(string queueName)
        {
            _mqServer.PurgeStore(queueName);
            return Ok();
        }


        [HttpDelete("DeleteQueue/{queueName}")]
        public IActionResult DeleteQueue(string queueName)
        {
            _mqServer.DeleteStore(queueName);
            return Ok();
        }
    }
}
