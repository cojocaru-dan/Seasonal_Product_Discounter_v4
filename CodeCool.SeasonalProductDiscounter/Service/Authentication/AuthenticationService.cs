using CodeCool.SeasonalProductDiscounter.Model.Users;
using CodeCool.SeasonalProductDiscounter.Service.Users;

namespace CodeCool.SeasonalProductDiscounter.Service.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    public bool Authenticate(User user)
    {
        User findUser = _userRepository.Get(user.UserName);
        if (findUser.UserName == "not exist") return false;
        bool matchName = user.UserName == findUser.UserName;
        return matchName;
    }
}
