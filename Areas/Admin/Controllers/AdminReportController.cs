using E_project_DVD_Shop.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_project_DVD_Shop.Areas.Admin.Controllers;

public class AdminReportController : AdminBaseController
{
    private readonly ReportService _reportService;

    public AdminReportController(ReportService reportService) => _reportService = reportService;

    public async Task<IActionResult> Index() => View(await _reportService.GetReportAsync());
}
