﻿using Microsoft.EntityFrameworkCore;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;

namespace PassIn.Application.UseCases.Attendees.GetAll
{
    public class GetAttendeesByIdUseCase
    {
        private readonly PassInDbContext _dbContext;

        public GetAttendeesByIdUseCase()
        {
            _dbContext = new PassInDbContext();
        }

        public ResponseAllAttendeesJson Execute(Guid eventId)
        {
            var entity = _dbContext.Events.Include(ev => ev.Attendees).ThenInclude(attendee => attendee.CheckIn).FirstOrDefault(ev => ev.Id == eventId);

            if (entity is null || entity.Equals(""))
            {
                throw new NotFoundExcpetion("Nenhum participante encontrado.");
            }

            return new ResponseAllAttendeesJson
            {
                Attendees = entity.Attendees.Select(attendee => new ResponseAttendeeJson
                {
                    Id = attendee.Id,
                    Name = attendee.Name,
                    Email = attendee.Email,
                    CreatedAt = attendee.Created_At,
                    CheckedInAt = attendee.CheckIn?.Created_At
                }).ToList()
            };
        }
    }
}
