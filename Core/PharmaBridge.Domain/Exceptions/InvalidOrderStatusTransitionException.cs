using PharmaBridge.Shared.EnumHelper.UserAccessEnums;

namespace PharmaBridge.Domain.Exceptions
{
    
    // Thrown when a pharmacy attempts an illegal order status transition
    // Example: jumping from Pending directly to Delivered
    // Extends BadRequestCustomeException => maps to HTTP 400 via global handler
    
    public class InvalidOrderStatusTransitionException : BadRequestCustomeException
    {
        public OrderStatus From { get; }
        public OrderStatus To { get; }

        public InvalidOrderStatusTransitionException(OrderStatus from, OrderStatus to)
            : base($"Invalid status transition: cannot move from '{from}' to '{to}' " +
                   $"Follow the pipeline: Pending → Prepared → OutForDelivery → Delivered")
        {
            From = from;
            To = to;
        }
    }
}