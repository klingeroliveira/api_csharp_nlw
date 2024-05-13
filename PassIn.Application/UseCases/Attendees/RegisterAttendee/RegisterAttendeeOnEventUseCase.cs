using Microsoft.EntityFrameworkCore;
using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using System.Net.Mail;

namespace PassIn.Application.UseCases.Attendees.RegisterAttendee
{
    public class RegisterAttendeeOnEventUseCase
    {
        private readonly PassInDbContext _dbContext;

        public RegisterAttendeeOnEventUseCase()
        {
            _dbContext = new PassInDbContext();
        }

        public ResponseRegisteredJson Execute(Guid eventId, RequestRegisterEventJson request)
        {
            Validate(eventId, request);

            var entity = new Infrastructure.Entities.Attendee
            {
                Email = request.Email,
                Name = request.Name,
                Event_Id = eventId,
                Created_At = DateTime.UtcNow,
            };

            _dbContext.Attendees.Add(entity);
            _dbContext.SaveChanges();

            return new ResponseRegisteredJson
            {
                Id = entity.Id
            };
        }

        private void Validate(Guid eventId, RequestRegisterEventJson request)
        {
            var eventEntity = _dbContext.Events.Find(eventId);

            if (eventEntity is null)
            {
                throw new NotFoundExcpetion("Evento não encontrado.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ErroOnValidationException("O Nome é inválido.");
            }

            if (!EmailIsValid(request.Email))
            {
                throw new ErroOnValidationException("O Email é inválido.");
            }

            var attendeeAlreadyRegistered = _dbContext.Attendees.Any(at => at.Email == request.Email && at.Event_Id == eventId);

            if (attendeeAlreadyRegistered)
            {
                throw new ConflictException("O participante já se cadastrou neste evento.");
            }

            int attendeesForEvent = _dbContext.Attendees.Count(at => at.Event_Id == eventId);

            if (attendeesForEvent == eventEntity.Maximum_Attendees)
            {
                throw new ErroOnValidationException("O número de participantes para este evento já foi atingido.");
            }
        }

        private bool EmailIsValid(string email)
        {
            try
            {
                new MailAddress(email);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
