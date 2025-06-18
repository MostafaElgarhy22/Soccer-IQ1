using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/standings-sync")]
public class StandingsSyncController : ControllerBase
{
    private readonly StandingsSyncService _svc;
    public StandingsSyncController(StandingsSyncService svc) => _svc = svc;
   
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Sync() =>
        await _svc.SyncAsync()
        ? Ok("Standings updated 🎉")
        : StatusCode(502, "Failed to fetch data from football-data.org");
}