using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;

namespace PassIn.Application.UseCases.CheckIns

{
    public class DoAttendeeCheckIn
    {
        private readonly PassInDbContext _dbContext;

        public DoAttendeeCheckIn()
        {
            _dbContext = new PassInDbContext();
        }

        public ResponseRegisteredJson Execute(Guid attendeeId)
        {
            Validade(attendeeId);

            var attendeeCheckIn = new Infrastructure.Entities.CheckIn
            {
                Attendee_Id = attendeeId,
                Created_At = DateTime.UtcNow,
            };

            _dbContext.CheckIns.Add(attendeeCheckIn);
            _dbContext.SaveChanges();

            return new ResponseRegisteredJson
            {
                Id = attendeeCheckIn.Id,                
            };
        }

        public void Validade(Guid attendeeId)
        {
            var existAttendee = _dbContext.Attendees.Any(attendee => attendee.Id == attendeeId);
            if (!existAttendee)
            {
                throw new NotFoundExcpetion("Participante não inscrito.");
            }

            var existCheckIn = _dbContext.CheckIns.Any(checkIn =>  checkIn.Attendee_Id == attendeeId);
            if (existCheckIn)
            {
                throw new ConflictException("Participante não pode fazer checkin duas vezes.");
            }
        }
    }
}
