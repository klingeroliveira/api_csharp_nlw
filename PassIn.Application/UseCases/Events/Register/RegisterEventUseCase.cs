using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;

namespace PassIn.Application.UseCases.Events.Register
{
    public class RegisterEventUseCase
    {
        private readonly PassInDbContext _dbContext;

        public RegisterEventUseCase()
        {
            _dbContext = new PassInDbContext();
        }

        public ResponseRegisteredJson Execute(RequestEventJson request)
        {           
            Validate(request);

            var entity = new Infrastructure.Entities.Event
            {
                Title = request.Title,
                Details = request.Details,
                Slug = request.Title.ToLower().Replace(" ", "-"),
                Maximum_Attendees = request.MaximumAttendees
            };

            _dbContext.Events.Add(entity);
            _dbContext.SaveChanges();

            return new ResponseRegisteredJson
            {
                Id = entity.Id
            };
        }

        private void Validate(RequestEventJson request)
        {
            if (request.MaximumAttendees < 1)
            {
                throw new ErroOnValidationException("O máximo de participantes é inválido.");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw new ErroOnValidationException("O Título é inválido.");
            }

            if (string.IsNullOrWhiteSpace(request.Details))
            {
                throw new ErroOnValidationException("O Detalhe é inválido.");
            }

            var slug = request.Title.ToLower().Replace(" ", "-");

            var eventRegister = _dbContext.Events.Any(ev => ev.Slug == slug);

            if (eventRegister)
            {
                throw new ErroOnValidationException("Já foi resgistrado um evento com este nome.");
            }
        }
    }
}
