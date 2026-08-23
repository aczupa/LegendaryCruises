using LegendaryCruises.Data;
using LegendaryCruises.Interfaces;
using LegendaryCruises.Models;
using LegendaryCruises.Models.DTOs;
using LegendaryCruises.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace LegendaryCruises.Services;

public class CruiseService : ICruiseService
{
    private readonly IDbContextFactory<DataContext> _factory;

    public CruiseService(IDbContextFactory<DataContext> factory)
    {
        _factory = factory;
    }

    // ============================================================
    // ADD CRUISE
    // ============================================================
    public async Task<BaseResponse> AddCruise(AddCruiseForm form)
    {
        await using var context = _factory.CreateDbContext();

        try
        {
            if (form.Dates == null || !form.Dates.Any())
            {
                return new BaseResponse
                {
                    StatusCode = 400,
                    Message = "At least one cruise date is required."
                };
            }

            var cruise = new Cruise
            {
                Title = form.Title,
                Description = form.Description,
                Destination = form.Destination,
                DeparturePort = form.DeparturePort,
                ArrivalPort = form.ArrivalPort,
                Currency = form.Currency,
                ImageUrl = form.ImageUrl,
              
                MaxPassengers = form.MaxPassengers,
                IsFeatured = form.IsFeatured,
                IsActive = form.IsActive,
                Slug = GenerateSlug(form.Destination),

                Itinerary = form.Itinerary?.Select(i => new ItineraryDay
                {
                    DayNumber = i.DayNumber,
                    Location = i.Location,
                    Description = i.Description
                }).ToList() ?? new List<ItineraryDay>(),

                CruiseDates = form.Dates.Select(d => new CruiseDate
                {
                    DepartureDate = d.DepartureDate,
                    ReturnDate = d.ReturnDate,
                    Cabins = d.Cabins?.Select(c => new DateCabin
                    {
                        CabinType = c.CabinType,
                        Price = c.Price,
                        Capacity = c.Capacity,
                        Reserved = 0
                    }).ToList() ?? new List<DateCabin>()
                }).ToList()
            };

            context.Cruises.Add(cruise);
            await context.SaveChangesAsync();

            return new BaseResponse
            {
                StatusCode = 201,
                Message = "Cruise added successfully."
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                StatusCode = 500,
                Message = ex.Message
            };
        }
    }

    // ============================================================
    // DELETE CRUISE
    // ============================================================
    public async Task<BaseResponse> DeleteCruise(int id)
    {
        await using var context = _factory.CreateDbContext();

        try
        {
            var cruise = await context.Cruises
                .Include(c => c.CruiseDates)
                    .ThenInclude(cd => cd.Cabins)
                .Include(c => c.Itinerary)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cruise == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Message = "Cruise not found."
                };
            }

            context.Cruises.Remove(cruise);
            await context.SaveChangesAsync();

            return new BaseResponse
            {
                StatusCode = 200,
                Message = "Cruise deleted successfully."
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse
            {
                StatusCode = 500,
                Message = $"Error deleting cruise: {ex.Message}"
            };
        }
    }

    // ============================================================
    // EDIT CRUISE
    // ============================================================
    public async Task<BaseResponse> EditCruise(Cruise cruise)
    {
        await using var context = _factory.CreateDbContext();

        try
        {
            var existing = await context.Cruises
                .Include(c => c.CruiseDates)
                    .ThenInclude(cd => cd.Cabins)
                .Include(c => c.Itinerary)
                .FirstOrDefaultAsync(c => c.Id == cruise.Id);

            if (existing == null)
                return new BaseResponse { StatusCode = 404, Message = "Cruise not found." };

            // 1. Update scalar properties
            existing.Title = cruise.Title;
            existing.Description = cruise.Description;
            existing.Destination = cruise.Destination;
            existing.DeparturePort = cruise.DeparturePort;
            existing.ArrivalPort = cruise.ArrivalPort;
            existing.Currency = cruise.Currency;
            existing.ImageUrl = cruise.ImageUrl;
           
            existing.MaxPassengers = cruise.MaxPassengers;
            existing.IsActive = cruise.IsActive;
            existing.IsFeatured = cruise.IsFeatured;
            existing.Slug = GenerateSlug(cruise.Destination);

            // 2. Synchronize Itinerary
            var incomingItineraryIds = cruise.Itinerary.Where(i => i.Id != 0).Select(i => i.Id).ToList();
            var itineraryToRemove = existing.Itinerary.Where(ei => !incomingItineraryIds.Contains(ei.Id)).ToList();
            context.ItineraryDays.RemoveRange(itineraryToRemove);

            foreach (var day in cruise.Itinerary)
            {
                var existingDay = existing.Itinerary.FirstOrDefault(ei => ei.Id == day.Id && ei.Id != 0);
                if (existingDay == null)
                {
                    existing.Itinerary.Add(new ItineraryDay
                    {
                        DayNumber = day.DayNumber,
                        Location = day.Location,
                        Description = day.Description
                    });
                }
                else
                {
                    existingDay.DayNumber = day.DayNumber;
                    existingDay.Location = day.Location;
                    existingDay.Description = day.Description;
                }
            }

            // 3. Synchronize CruiseDates & Cabins
            var incomingDateIds = cruise.CruiseDates.Where(d => d.Id != 0).Select(d => d.Id).ToList();
            var datesToRemove = existing.CruiseDates.Where(ed => !incomingDateIds.Contains(ed.Id)).ToList();
            context.CruiseDates.RemoveRange(datesToRemove);

            foreach (var date in cruise.CruiseDates)
            {
                var existingDate = existing.CruiseDates.FirstOrDefault(ed => ed.Id == date.Id && ed.Id != 0);
                if (existingDate == null)
                {
                    // New date with cabins
                    existing.CruiseDates.Add(new CruiseDate
                    {
                        DepartureDate = date.DepartureDate,
                        ReturnDate = date.ReturnDate,
                        Cabins = date.Cabins?.Select(c => new DateCabin
                        {
                            CabinType = c.CabinType,
                            Price = c.Price,
                            Capacity = c.Capacity,
                            Reserved = c.Reserved
                        }).ToList() ?? new List<DateCabin>()
                    });
                }
                else
                {
                    // Update existing date
                    existingDate.DepartureDate = date.DepartureDate;
                    existingDate.ReturnDate = date.ReturnDate;

                    // Synchronize Cabins for existing date
                    var incomingCabinIds = date.Cabins.Where(c => c.Id != 0).Select(c => c.Id).ToList();
                    var cabinsToRemove = existingDate.Cabins.Where(ec => !incomingCabinIds.Contains(ec.Id)).ToList();
                    context.DateCabins.RemoveRange(cabinsToRemove);

                    foreach (var cabin in date.Cabins)
                    {
                        var existingCabin = existingDate.Cabins.FirstOrDefault(ec => ec.Id == cabin.Id && ec.Id != 0);
                        if (existingCabin == null)
                        {
                            existingDate.Cabins.Add(new DateCabin
                            {
                                CabinType = cabin.CabinType,
                                Price = cabin.Price,
                                Capacity = cabin.Capacity,
                                Reserved = cabin.Reserved
                            });
                        }
                        else
                        {
                            existingCabin.CabinType = cabin.CabinType;
                            existingCabin.Price = cabin.Price;
                            existingCabin.Capacity = cabin.Capacity;
                            existingCabin.Reserved = cabin.Reserved;
                        }
                    }
                }
            }

            await context.SaveChangesAsync();
            return new BaseResponse { StatusCode = 200, Message = "Updated successfully." };
        }
        catch (Exception ex)
        {
            return new BaseResponse { StatusCode = 500, Message = ex.Message };
        }
    }

    // ============================================================
    // GET SINGLE
    // ============================================================
    public async Task<GetCruiseResponse> GetCruise(int id)
    {
        await using var context = _factory.CreateDbContext();

        try
        {
            var cruise = await context.Cruises
                .Include(c => c.CruiseDates)
                    .ThenInclude(cd => cd.Cabins)
                .Include(c => c.Itinerary)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cruise == null)
            {
                return new GetCruiseResponse
                {
                    StatusCode = 404,
                    Message = "Cruise not found.",
                    Cruise = null
                };
            }

            return new GetCruiseResponse
            {
                StatusCode = 200,
                Message = "Cruise retrieved successfully.",
                Cruise = cruise
            };
        }
        catch (Exception ex)
        {
            return new GetCruiseResponse
            {
                StatusCode = 500,
                Message = ex.Message,
                Cruise = null
            };
        }
    }

    // ============================================================
    // GET ALL
    // ============================================================
    public async Task<GetCruisesResponse> GetCruises()
    {
        await using var context = _factory.CreateDbContext();

        try
        {
            var cruises = await context.Cruises
                .Include(c => c.CruiseDates)
                    .ThenInclude(cd => cd.Cabins)
                .Include(c => c.Itinerary)
                .AsSplitQuery()
                .ToListAsync();

            return new GetCruisesResponse
            {
                StatusCode = 200,
                Message = "Cruises retrieved successfully.",
                Cruises = cruises
            };
        }
        catch (Exception ex)
        {
            return new GetCruisesResponse
            {
                StatusCode = 500,
                Message = ex.Message,
                Cruises = new List<Cruise>()
            };
        }
    }

    // ============================================================
    // SLUG
    // ============================================================
    private string GenerateSlug(string input)
    {
        return input
            .ToLower()
            .Trim()
            .Replace(" ", "-")
            .Replace("î", "i")
            .Replace("é", "e")
            .Replace("è", "e")
            .Replace("ê", "e");
    }

    // ============================================================
    // GET BY SLUG
    // ============================================================
    public async Task<Cruise?> GetCruiseBySlug(string slug)
    {
        await using var context = _factory.CreateDbContext();

        return await context.Cruises
            .Include(c => c.CruiseDates)
                .ThenInclude(cd => cd.Cabins)
            .Include(c => c.Itinerary)
            .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);
    }

    public async Task<Cruise?> GetCruiseByName(string name)
    {
        await using var context = await _factory.CreateDbContextAsync();

        return await context.Cruises
            .Include(c => c.CruiseDates)
                .ThenInclude(cd => cd.Cabins)
            .Include(c => c.Itinerary)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c =>
                c.Destination.ToLower() == name.ToLower());
    }

    public async Task<GetCruisesResponse> GetNewCruises()
    {
        await using var context = _factory.CreateDbContext();

        try
        {
            var cruises = await context.Cruises
                .Where(c => c.Id >= 10 && c.IsActive)
                .Include(c => c.CruiseDates)
                    .ThenInclude(cd => cd.Cabins)
                .Include(c => c.Itinerary)
                .OrderByDescending(c => c.Id)
                .AsSplitQuery()
                .ToListAsync();

            return new GetCruisesResponse
            {
                StatusCode = 200,
                Message = "New cruises retrieved successfully.",
                Cruises = cruises
            };
        }
        catch (Exception ex)
        {
            return new GetCruisesResponse
            {
                StatusCode = 500,
                Message = ex.Message,
                Cruises = new List<Cruise>()
            };
        }
    }
}