using System.Collections.Generic;
using System.Web.Mvc;
using travelManagement.Models;

namespace travelManagement.Controllers
{
    public class CodeApiController : Controller
    {
        [HttpPost]
        public ActionResult EncryptString(string encKey, string rawText)
        {
            return Json(new ApiResult<byte[]>(
                CodeModule.EncrytString(encKey, rawText)));
        }

        public class DecryptParameter
        {
            public string EncKey { get; set; }
            public List<byte[]> EncData { get; set; }
        }

        [HttpPost]
        public ActionResult BatchDecryptData(DecryptParameter decData)
        {
            return Json(new ApiResult<List<string>>(
                CodeModule.DecryptData(decData.EncKey, decData.EncData)));
        }
    }
}
