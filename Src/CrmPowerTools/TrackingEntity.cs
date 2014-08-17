using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace CrmPowerTools
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
            this.Id = existing.Id;
            this.LogicalName = existing.LogicalName;
            this.EntityState = existing.EntityState;
            this.Attributes = existing.Attributes;
            this.ExtensionData = existing.ExtensionData;

            foreach (var relatedEntity in existing.RelatedEntities)
            {
                this.RelatedEntities.Add(relatedEntity);
            }

            foreach (var formattedValue in existing.FormattedValues)
            {
                this.FormattedValues.Add(formattedValue);
            }
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

        internal Entity ShallowCopyAttributesExcluded(Entity entity)
        {
            Entity copy = new Entity();

            copy.Id = entity.Id;
            copy.LogicalName = entity.LogicalName;
            copy.EntityState = entity.EntityState;
            copy.ExtensionData = entity.ExtensionData;

            foreach (var relatedEntity in entity.RelatedEntities)
            {
                copy.RelatedEntities.Add(relatedEntity);
            }

            foreach (var formattedValue in entity.FormattedValues)
            {
                copy.FormattedValues.Add(formattedValue);
            }

            return copy;
        }

        internal Entity ShallowCopy(Entity entity)
        {
            Entity initialCopy = ShallowCopyAttributesExcluded(entity);
            initialCopy.Attributes = entity.Attributes;

            return initialCopy;
        }

    }
}