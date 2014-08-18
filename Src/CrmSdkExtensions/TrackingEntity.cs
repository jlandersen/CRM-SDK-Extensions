using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace CrmExtensions
{
    public class TrackingEntity : Entity
    {
        private ISet<string> modifiedAttributes = new HashSet<string>();

        public TrackingEntity() : base()
        {
            
        }

        public TrackingEntity(string entityName) : base(entityName)
        {

        }

        public TrackingEntity(Entity existing)
        {
            ShallowCopyAttributesExcluded(existing, this);
        }

        /// <summary>
        /// Provides an indexer for the attribute values. If any attribute is set or updated it will be included in the Attributes collection 
        /// of the resulting entity from invoking the <see cref="ToFinalEntity"/> method.
        /// </summary>
        /// <param name="attributeName">The logical name of the attribute.</param>
        /// <returns>The attribute specified by the attributeName parameter.</returns>
        public new object this[string attributeName]
        {
            get
            {
                return this.Attributes[attributeName];    
            }
            set
            {
                if (!modifiedAttributes.Contains(attributeName))
                {
                    modifiedAttributes.Add(attributeName);
                }

                this.Attributes[attributeName] = value;
            }   
        }

        /// <summary>
        /// Provides a snapshot of all the attributes that has been modified.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetModifiedAttributes()
        {
            return modifiedAttributes.ToList();
        }

        /// <summary>
        /// Returns a new entity with only the attributes that have been set or updated.
        /// </summary>
        /// <returns></returns>
        public Entity ToFinalEntity()
        {
            Entity finalEntity = ShallowCopyAttributesExcluded(this);
            foreach (var modifiedAttribute in modifiedAttributes)
            {
                finalEntity[modifiedAttribute] = this[modifiedAttribute];
            }

            return finalEntity;
        }

        internal Entity ShallowCopyAttributesExcluded(Entity entity, Entity destination = null)
        {
            Entity copy = destination ?? new Entity();

            copy.Id = entity.Id;
            copy.LogicalName = entity.LogicalName;
            copy.EntityState = entity.EntityState;
            copy.ExtensionData = entity.ExtensionData;
            copy.RelatedEntities.AddRange(entity.RelatedEntities);
            copy.FormattedValues.AddRange(entity.FormattedValues);

            return copy;
        }

        internal Entity ShallowCopy(Entity entity, Entity destination = null)
        {
            Entity initialCopy = ShallowCopyAttributesExcluded(entity, destination);
            initialCopy.Attributes = entity.Attributes;

            return initialCopy;
        }

    }
}