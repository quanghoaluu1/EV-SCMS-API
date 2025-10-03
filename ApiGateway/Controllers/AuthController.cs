using Auth.Grpc;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthGrpc.AuthGrpcClient _authClient;

    public AuthController(AuthGrpc.AuthGrpcClient authClient)
    {
        _authClient = authClient;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var reply = await _authClient.LoginAsync(request);
        return Ok(new
        {
            reply.Token,
            reply.Message
        });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyRequest request)
    {
        var reply = await _authClient.VerifyTokenAsync(request);
        return Ok(new
        {
            reply.IsValid,
            reply.UserId
        });
    }
}