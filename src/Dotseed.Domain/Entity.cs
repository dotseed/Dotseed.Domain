using System;
using System.Collections.Generic;

using MediatR;

namespace Dotseed.Domain
{
    public abstract class Entity
    {
        int? _requestedHashCode;

        Guid _id;     
        
        public DateTime CreatedAt { get; private set; }
        
        public DateTime UpdatedAt { get; private set; }

        public virtual Guid Id 
        {
            get
            {
                return _id;
            }
            
            protected set
            {
                _id = value;
            }
        }

        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public bool IsTransient()
        {
            var defaultGuid = default(Guid);
            var result = this.Id == defaultGuid;
            return result;    
        }
        
        public void SetCreatedAt(DateTime createdAt)
        {
            CreatedAt = createdAt;
        }
        
        public void SetUpdateAt(DateTime updatedAt)
        {
            UpdatedAt = updatedAt;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            Entity item = (Entity)obj;

            if (item.IsTransient() || IsTransient())
                return false;
            
            return item.Id == Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = Id.GetHashCode() ^ 31;

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();

        }
        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return Object.Equals(right, null);
            
            return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}