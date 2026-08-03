using LegendaryCruises.Data;
using LegendaryCruises.Interfaces;
using LegendaryCruises.Models;
using LegendaryCruises.Models.DTOs;
using LegendaryCruises.Models.Responses;
using Microsoft.EntityFrameworkCore;

public class CruiseService : ICruiseService
{
    private readonly DataContext _context;

    public CruiseService(DataContext context)
    {
        _context = context;
    }

    // ============================================================
    // ADD CRUISE
    // ============================================================
    public async Task<BaseResponse> AddCruise(AddCruiseForm form)
    {
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
                VideoUrl = form.VideoUrl,
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

            _context.Cruises.Add(cruise);
            await _context.SaveChangesAsync();

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
        try
        {
            var cruise = await _context.Cruises
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

            _context.Cruises.Remove(cruise);
            await _context.SaveChangesAsync();

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


    public async Task<BaseResponse> EditCruise(Cruise cruise)
    {
        try
        {
            var existing = await _context.Cruises
                .Include(c => c.CruiseDates)
                    .ThenInclude(cd => cd.Cabins)
                .Include(c => c.Itinerary)
                .FirstOrDefaultAsync(c => c.Id == cruise.Id);

            if (existing == null)
            {
                return new BaseResponse
                {
                    StatusCode = 404,
                    Message = "Cruise not found"
                };
            }

            // =====================================================
            // SCALAR FIELDS
            // =====================================================
            existing.Title = cruise.Title;
            existing.Description = cruise.Description;
            existing.Destination = cruise.Destination;
            existing.DeparturePort = cruise.DeparturePort;
            existing.ArrivalPort = cruise.ArrivalPort;
            existing.Currency = cruise.Currency;
            existing.IsActive = cruise.IsActive;
            existing.IsFeatured = cruise.IsFeatured;
            existing.Slug = GenerateSlug(cruise.Destination);

            // =====================================================
            // ITINERARY (SAFE SYNC - NO LOSS)
            // =====================================================

            // remove missing
            var itineraryToRemove = existing.Itinerary
                .Where(ei => !cruise.Itinerary.Any(i =>
                    i.DayNumber == ei.DayNumber))
                .ToList();

            _context.ItineraryDays.RemoveRange(itineraryToRemove);

            // update or add
            foreach (var i in cruise.Itinerary)
            {
                var existingDay = existing.Itinerary
                    .FirstOrDefault(ei => ei.DayNumber == i.DayNumber);

                if (existingDay == null)
                {
                    existing.Itinerary.Add(new ItineraryDay
                    {
                        DayNumber = i.DayNumber,
                        Location = i.Location,
                        Description = i.Description
                    });
                }
                else
                {
                    existingDay.Location = i.Location;
                    existingDay.Description = i.Description;
                }
            }

            // =====================================================
            // CRUISE DATES (SAFE SYNC)
            // =====================================================

            var incomingDates = cruise.CruiseDates.ToList();

            var datesToRemove = existing.CruiseDates
                .Where(ed => !incomingDates.Any(d =>
                    d.DepartureDate == ed.DepartureDate &&
                    d.ReturnDate == ed.ReturnDate))
                .ToList();

            _context.CruiseDates.RemoveRange(datesToRemove);

            foreach (var d in incomingDates)
            {
                var existingDate = existing.CruiseDates.FirstOrDefault(ed =>
                    ed.DepartureDate == d.DepartureDate &&
                    ed.ReturnDate == d.ReturnDate);

                if (existingDate == null)
                {
                    // NEW DATE
                    var newDate = new CruiseDate
                    {
                        DepartureDate = d.DepartureDate,
                        ReturnDate = d.ReturnDate,
                        Cabins = d.Cabins.Select(c => new DateCabin
                        {
                            CabinType = c.CabinType,
                            Price = c.Price,
                            Capacity = c.Capacity,
                            Reserved = 0
                        }).ToList()
                    };

                    existing.CruiseDates.Add(newDate);
                }
                else
                {
                    // =================================================
                    // CABINS UPDATE (NO LOSS OF RESERVED)
                    // =================================================

                    foreach (var cabinDto in d.Cabins)
                    {
                        var existingCabin = existingDate.Cabins
                            .FirstOrDefault(c => c.CabinType == cabinDto.CabinType);

                        if (existingCabin == null)
                        {
                            existingDate.Cabins.Add(new DateCabin
                            {
                                CabinType = cabinDto.CabinType,
                                Price = cabinDto.Price,
                                Capacity = cabinDto.Capacity,
                                Reserved = 0
                            });
                        }
                        else
                        {
                            existingCabin.Price = cabinDto.Price;
                            existingCabin.Capacity = cabinDto.Capacity;
                        }
                    }

                    // remove missing cabins safely
                    var cabinsToRemove = existingDate.Cabins
                        .Where(ec => !d.Cabins.Any(dc => dc.CabinType == ec.CabinType))
                        .ToList();

                    _context.DateCabins.RemoveRange(cabinsToRemove);
                }
            }

            // =====================================================
            // SAVE
            // =====================================================
            await _context.SaveChangesAsync();

            return new BaseResponse
            {
                StatusCode = 200,
                Message = "Updated successfully"
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
    // GET SINGLE
    // ============================================================
    public async Task<GetCruiseResponse> GetCruise(int id)
    {
        try
        {
            var cruise = await _context.Cruises
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
        try
        {
            var cruises = await _context.Cruises
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
        return await _context.Cruises
            .Include(c => c.CruiseDates)
                .ThenInclude(cd => cd.Cabins)
            .Include(c => c.Itinerary)
            .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);
    }

    public async Task<Cruise?> GetCruiseByName(string name)
    {
        return await _context.Cruises
            .Include(c => c.CruiseDates)
                .ThenInclude(cd => cd.Cabins)
            .Include(c => c.Itinerary)
            .FirstOrDefaultAsync(c => c.Destination.ToLower() == name.ToLower());
    }

    public async Task<GetCruisesResponse> GetNewCruises()
    {
        try
        {
            var cruises = await _context.Cruises
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