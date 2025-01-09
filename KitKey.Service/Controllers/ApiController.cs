using Microsoft.AspNetCore.Mvc;
using NTDLS.KitKey.Server;
using NTDLS.KitKey.Shared;
using Serilog;
using System.Text;

namespace KitKey.Service.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private readonly KkServer _keyServer;

        public ApiController(KkServer keyServer)
        {
            _keyServer = keyServer;
        }

        [HttpPost("Set/{storeName}/{key}")]
        public async Task<IActionResult> Set(string storeName, string key)
        {
            try
            {
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                var bodyValue = await reader.ReadToEndAsync();
                _keyServer.StringSet(storeName, key, bodyValue);
                return Ok("value stored");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpDelete("Delete/{storeName}/{key}")]
        public IActionResult Upsert(string storeName, string key)
        {
            try
            {
                _keyServer.Delete(storeName, key);
                return Ok("value deleted");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpGet("Get/{storeName}/{key}")]
        public IActionResult Get(string storeName, string key)
        {
            try
            {
                var value = _keyServer.StringGet(storeName, key);
                if (value == null)
                {
                    return NoContent();
                }
                return Ok(value);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Creates a key-value store with the default configuration.
        /// </summary>
        /// <param name="storeName"></param>
        /// <returns></returns>
        [HttpPost("StoreCreate/{storeName}")]
        public IActionResult StoreCreate(string storeName)
        {
            try
            {
                _keyServer.StoreCreate(new KkStoreConfiguration
                {
                    StoreName = storeName
                });
                return Ok("store created");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Creates a key-value store with a custom configuration.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost("StoreCreate")]
        public IActionResult StoreCreate([FromBody] KkStoreConfiguration config)
        {
            try
            {
                if (config == null || string.IsNullOrEmpty(config.StoreName))
                {
                    return BadRequest("StoreName is required.");
                }

                _keyServer.StoreCreate(config);

                return Ok("store created");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpDelete("StorePurge/{storeName}")]
        public IActionResult Purge(string storeName)
        {
            try
            {
                _keyServer.StorePurge(storeName);
                return Ok("store purged");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }


        [HttpDelete("StoreDelete/{storeName}")]
        public IActionResult StoreDelete(string storeName)
        {
            try
            {
                _keyServer.StoreDelete(storeName);
                return Ok("store deleted");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
