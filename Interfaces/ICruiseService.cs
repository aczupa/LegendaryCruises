using LegendaryCruises.Models;
using LegendaryCruises.Models.DTOs;
using LegendaryCruises.Models.Responses;

namespace LegendaryCruises.Interfaces
{
    public interface ICruiseService
    {
        Task<GetCruisesResponse> GetCruises();
        Task<BaseResponse> AddCruise(AddCruiseForm form);

        Task<GetCruiseResponse> GetCruise(int id);
        Task<BaseResponse> DeleteCruise(int id);

        Task<BaseResponse> EditCruise(Cruise cruise);
        Task<Cruise?> GetCruiseBySlug(string slug);
        Task<Cruise?> GetCruiseByName(string name);
       
        Task<GetCruisesResponse> GetNewCruises();

    }
}
