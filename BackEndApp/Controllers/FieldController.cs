using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StorageApp;
using System;
using System.IO;
using System.Security.Claims;

namespace BackEndApp.Controllers
{
    [Route("[controller]")]
    public class FieldController : ControllerBase
    {
        private readonly ILogger<FieldController> _logger;
        private IConfiguration Configuration { get; }

        public FieldController(IConfiguration configuration, ILogger<FieldController> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        [HttpGet("GetFile/{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            
            if (fileId == null)
            {
                return BadRequest();
            }

            Guid id;
            if (!Guid.TryParse(fileId, out id))
            {
                return BadRequest();
            }

            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            Storage storage = new Storage(Configuration.GetSection("Storage"), owner);
            MemoryStream stream;
            if (!storage.GetFile(id, out stream))
            {
                return UnprocessableEntity();
            }
            return Ok(Convert.ToBase64String(stream.ToArray()));
        }
    }
}
