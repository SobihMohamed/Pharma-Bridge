using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Events
{
    public class PrescriptionRequestCreatedEvent : INotification
    {
        public int RequestId { get; }

        public PrescriptionRequestCreatedEvent(int requestId)
        {
            RequestId = requestId;
        }
    }
}
