using Microsoft.Xrm.Sdk;

namespace CrmExtensions.Tests
{
    public static class EntityExtensions
    {
        /// <summary>
        /// Makes a tracking entity from an existing entity that may contain a number of attributes.
        /// The tracking entity will monitor which attributes are modified.
        /// </summary>
        /// <param name="entity">Original CRM entity</param>
        /// <returns>Tracking version of the original entity</returns>
        public static TrackingEntity ToTrackingEntity(this Entity entity)
        {
            return new TrackingEntity(entity);
        }
    }
}