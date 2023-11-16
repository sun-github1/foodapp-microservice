using Food.Web.Models;

namespace Food.Web.Services.Interfaces
{
    public interface IBaseService
    {
        Task<T> SendAsync<T>(RequestDto requestDto, bool withBearer = true);
    }
}
