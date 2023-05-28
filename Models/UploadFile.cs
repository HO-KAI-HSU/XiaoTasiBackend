using System;
using System.Web;

namespace travelManagement.Models
{
    // <summary>
    /// API呼叫時，傳回的統一物件
    /// </summary>
    public class UploadFile
    {
         public void _uploadPic(string imgPath, HttpPostedFileBase file)
        {
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0){
                // extract only the filename
                var fileName = System.IO.Path.GetFileName(file.FileName);
                var path = System.IO.Path.Combine(HttpContext.Current.Server.MapPath(imgPath), fileName);
                file.SaveAs(path);
            }
        }
    }
}
