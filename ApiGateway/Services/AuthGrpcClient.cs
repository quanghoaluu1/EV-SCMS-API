using AuthService.Grpc;

namespace ApiGateway.Services
{
    public interface IAuthGrpcClient
    {
        Task<LoginResponse> LoginAsync(string email, string password);
        Task<RegisterResponse> RegisterAsync(string email, string password, string confirmPassword, string fullName, string phoneNumber, int role);
        Task<ValidateTokenResponse> ValidateTokenAsync(string token);
        Task<GetUserByEmailResponse> GetUserByEmailAsync(string email);
    }

    public class AuthGrpcClient : IAuthGrpcClient
    {
        private readonly AuthService.Grpc.AuthService.AuthServiceClient _client;
        private readonly ILogger<AuthGrpcClient> _logger;

        public AuthGrpcClient(
            AuthService.Grpc.AuthService.AuthServiceClient client,
            ILogger<AuthGrpcClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            try
            {
                _logger.LogInformation("Calling Auth Service - Login for {Email}", email);

                var request = new LoginRequest
                {
                    Email = email,
                    Password = password
                };

                return await _client.LoginAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call Auth Service - Login");
                throw;
            }
        }

        public async Task<RegisterResponse> RegisterAsync(string email, string password, string confirmPassword, string fullName, string phoneNumber, int role)
        {
            try
            {
                _logger.LogInformation("Calling Auth Service - Register for {Email}", email);

                var request = new RegisterRequest
                {
                    Email = email,
                    Password = password,
                    ConfirmPassword = confirmPassword,
                    FullName = fullName,
                    PhoneNumber = phoneNumber,
                    Role = role
                };

                return await _client.RegisterAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call Auth Service - Register");
                throw;
            }
        }

        public async Task<ValidateTokenResponse> ValidateTokenAsync(string token)
        {
            try
            {
                _logger.LogInformation("Calling Auth Service - ValidateToken");

                var request = new ValidateTokenRequest
                {
                    Token = token
                };

                return await _client.ValidateTokenAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call Auth Service - ValidateToken");
                throw;
            }
        }

        public async Task<GetUserByEmailResponse> GetUserByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation("Calling Auth Service - GetUserByEmail for {Email}", email);

                var request = new GetUserByEmailRequest
                {
                    Email = email
                };

                return await _client.GetUserByEmailAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call Auth Service - GetUserByEmail");
                throw;
            }
        }
    }
}