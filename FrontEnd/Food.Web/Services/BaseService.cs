using Food.Web.Models;
using Food.Web.Services.Interfaces;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using static Food.Web.StartingDetails;

namespace Food.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private ResponseDto apiResponseDto { get; set; }
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            apiResponseDto =new ResponseDto();
        }
        public async Task<T> SendAsync<T>(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("FoodAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri=new Uri(requestDto.Url);
                client.DefaultRequestHeaders.Clear();
                if(requestDto.Data != null)
                {
                    message.Content = new StringContent(
                        JsonConvert.SerializeObject(requestDto.Data),
                        Encoding.UTF8, "application/json");
                }

                if(!string.IsNullOrEmpty(requestDto?.AccessToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                        requestDto.AccessToken);
                }

                HttpResponseMessage response = null;
                switch (requestDto.ApiType)
                {
                    case ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    case ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
                response = await client.SendAsync(message);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        apiResponseDto.IsSuccess = false;
                        apiResponseDto.Message = "Not Found";
                        break;
                    case HttpStatusCode.Forbidden:
                        apiResponseDto.IsSuccess = false;
                        apiResponseDto.Message = "Access Denied";
                        break;
                    case HttpStatusCode.Unauthorized:
                        apiResponseDto.IsSuccess = false;
                        apiResponseDto.Message = "Unauthorized";
                        break;
                    case HttpStatusCode.InternalServerError:
                        apiResponseDto.IsSuccess = false;
                        apiResponseDto.Message = "Internal Server Error";
                        break;
                    default:
                        var apiContent = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(apiContent);
                        //return apiResponseDto;
                }
                return (T)((object)apiResponseDto);
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return (T)((object)dto);
            }
        }
    }
}
