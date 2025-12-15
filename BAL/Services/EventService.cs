using AutoMapper;
using CommunityEventsApi.BAL.Interfaces;
using CommunityEventsApi.DAL.Interfaces;
using CommunityEventsApi.DTOs.Events;

namespace CommunityEventsApi.BAL.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public EventService(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync(EventFilterDto filter)
        {
            // TODO: Implement filtering logic
            var events = await _eventRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EventDto>>(events);
        }

        public async Task<EventDto?> GetEventByIdAsync(Guid id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            return eventEntity == null ? null : _mapper.Map<EventDto>(eventEntity);
        }

        public async Task<EventDto> CreateEventAsync(CreateEventDto createDto, Guid userId)
        {
            // TODO: Implement event creation with location and image handling
            throw new NotImplementedException();
        }

        public async Task<EventDto> UpdateEventAsync(Guid id, CreateEventDto updateDto, Guid userId)
        {
            // TODO: Implement event update logic
            throw new NotImplementedException();
        }

        public async Task DeleteEventAsync(Guid id, Guid userId)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);

            if (eventEntity == null)
            {
                throw new KeyNotFoundException("Event not found");
            }

            if (eventEntity.Organizer.Id != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this event");
            }

            await _eventRepository.DeleteAsync(eventEntity);
        }

        public async Task<bool> RegisterForEventAsync(Guid eventId, Guid userId)
        {
            // TODO: Implement event registration logic
            throw new NotImplementedException();
        }

        public async Task<bool> UnregisterFromEventAsync(Guid eventId, Guid userId)
        {
            // TODO: Implement event unregistration logic
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EventDto>> GetNearbyEventsAsync(double latitude, double longitude, double radiusKm)
        {
            var events = await _eventRepository.GetNearbyEventsAsync(latitude, longitude, radiusKm);
            return _mapper.Map<IEnumerable<EventDto>>(events);
        }
    }
}
