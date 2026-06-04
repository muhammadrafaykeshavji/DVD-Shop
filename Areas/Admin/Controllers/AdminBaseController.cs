using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public abstract class AdminBaseController : Controller
{
    protected void SetMessage(string message, bool success = true)
    {
        TempData[success ? "Success" : "Error"] = message;
    }
}
