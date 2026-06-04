using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminDashboardController : AdminBaseController
{
    private readonly DashboardService _dashboardService;

    public AdminDashboardController(DashboardService dashboardService) =>
        _dashboardService = dashboardService;

    public async Task<IActionResult> Index() =>
        View(await _dashboardService.GetAdminDashboardAsync());
}
