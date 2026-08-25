using LegendaryCruises.Models;
using LegendaryCruises.Models.DTOs;
using LegendaryCruises.Models.Responses;
using System.Security.Claims;

namespace LegendaryCruises.Interfaces
{
    public interface ICruiseService
    {
        Task<GetCruisesResponse> GetCruises();
        Task<BaseResponse> AddCruise(AddCruiseForm form, ClaimsPrincipal currentUser);
        Task<GetCruiseResponse> GetCruise(int id);
        Task<BaseResponse> DeleteCruise(int id, ClaimsPrincipal currentUser);
        Task<BaseResponse> EditCruise(Cruise cruise, ClaimsPrincipal currentUser);
        Task<Cruise?> GetCruiseBySlug(string slug);
        Task<Cruise?> GetCruiseByName(string name);

        Task<GetCruisesResponse> GetNewCruises();
    }
}