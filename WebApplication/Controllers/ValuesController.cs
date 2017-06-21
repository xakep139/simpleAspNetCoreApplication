using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/{api-version:apiVersion}/values")]
    public class ValuesController : ControllerBase
    {
        // GET api/values/5/some_version_id
        [HttpGet("{id:int}/{versionId}")]
        public string GetVersion(int id, string versionId)
        {
            return $"value from {id} with version {versionId}";
        }

        // GET api/values/5/versions
        [HttpGet("{id:int}/versions")]
        public string GetVersions(int id)
        {
            return $"value from {id} with all versions";
        }
    }
}
