using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace CrmPowerTools.Tests
{
    public class TrackingEntityTests
    {
        [Fact]
        public void TestGetModifiedAttributesOnNewEntityNoAttributesSetReturnsEmptyCollection()
        {
            TrackingEntity entity = new TrackingEntity();
            
            var modifiesAttributes = entity.GetModifiedAttributes();
            
            Assert.Empty(modifiesAttributes);
        }
        
        [Fact]
        public void TestGetModifiedAttributesSetOneAttributeReturnsOneElement()
        {
            TrackingEntity entity = new TrackingEntity();
            string attribute = "money";

            entity[attribute] = 2.0m;
            var modifiedAttributes = entity.GetModifiedAttributes();

            Assert.Single(modifiedAttributes);
            Assert.Equal(attribute, modifiedAttributes.First());
        }

        [Fact]
        public void TestGetModifiedAttributesSetMultipleAttributeReturnsAllAttributes()
        {
            TrackingEntity entity = new TrackingEntity();
            var attributes = Enumerable
                .Range(0, 10)
                .Select(c => Tuple.Create("Attribute" + c, "Value" + c))
                .ToList();

            foreach (var attribute in attributes)
            {
                entity[attribute.Item1] = attribute.Item2;
            }

            var modifiedAttributes = entity.GetModifiedAttributes();

            Assert.Equal(attributes.Select(c => c.Item1).ToList(), modifiedAttributes);
        }

        [Fact]
        public void TestSetAttributeIndexerOneTimeReturnsValue()
        {
            TrackingEntity entity = new TrackingEntity();
            string attributeName = "attribute";
            decimal attributeValue = 2.5m;

            entity[attributeName] = attributeValue;

            decimal returnedValue = (decimal) entity[attributeName];

            Assert.Equal(attributeValue, returnedValue);
        }

        [Fact]
        public void TestSetAttributeIndexerMultipleTimesReturnsLastValue()
        {
            TrackingEntity entity = new TrackingEntity();
            string attributeName = "attribute";
            var decimalValues = Enumerable
                .Range(0, 10)
                .Select(c => new decimal(c))
                .ToList();

            foreach (var decimalValue in decimalValues)
            {
                entity[attributeName] = decimalValue;
            }

            decimal returnedValue = (decimal)entity[attributeName];

            Assert.Equal(decimalValues.Last(), returnedValue);
        }

        [Fact]
        public void TestGetAttributeIndexerNoAttributeValueThrowsException()
        {
            TrackingEntity entity = new TrackingEntity();

            Assert.Throws<KeyNotFoundException>(() => entity["someattribute"]);
        }

        [Fact]
        public void TestToFinalEntityOneModifiedAttributeFromExistingReturnsEntityWithOneAttribute()
        {
            Entity originalEntity = new Entity();
            originalEntity["attribute1"] = 2.0m;
            originalEntity["attribute2"] = "some string value";
            originalEntity["attribute3"] = true;
            TrackingEntity entity = new TrackingEntity(originalEntity);
            entity["attribute1"] = 3.0m;

            Entity finalEntity = entity.ToFinalEntity();

            Assert.Single(finalEntity.Attributes);
            Assert.Equal("attribute1", finalEntity.Attributes.First().Key);
            Assert.Equal(3.0m, finalEntity.Attributes.First().Value);
        }

        [Fact]
        public void TestToFinalEntityNoModifiedAttributesFromExistingReturnsEntityWithEmptyAttributeCollection()
        {
            Entity originalEntity = new Entity();
            originalEntity["attribute1"] = 5.0m;
            originalEntity["attribute2"] = "some string value";
            originalEntity["attribute3"] = false;
            TrackingEntity entity = new TrackingEntity(originalEntity);

            Entity finalEntity = entity.ToFinalEntity();

            Assert.Empty(finalEntity.Attributes);
        }

        [Fact]
        public void TestToFinalEntityMultipleModifiedAttributesFromExistingReturnsEntityWithSameAttributeCollection()
        {
            Entity originalEntity = new Entity();
            originalEntity["attribute1"] = 5.0m;
            originalEntity["attribute2"] = "some string value";
            originalEntity["attribute3"] = false;
            TrackingEntity entity = new TrackingEntity(originalEntity);

            var attributeUpdateValues = new List<KeyValuePair<string, object>>();
            attributeUpdateValues.Add(new KeyValuePair<string, object>("attribute1", 10.0m));
            attributeUpdateValues.Add(new KeyValuePair<string, object>("attribute3", false));

            foreach (var attributeUpdateValue in attributeUpdateValues)
            {
                entity[attributeUpdateValue.Key] = attributeUpdateValue.Value;
            }

            Entity finalEntity = entity.ToFinalEntity();

            Assert.Equal(attributeUpdateValues, finalEntity.Attributes);
        }

    }
}