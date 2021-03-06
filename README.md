# CRM SDK Extensions
=============
Some things that, over time has emerged in my solutions, and can be handy for CRM 2011/2013/Online development.


## Use TrackingEntity To Optimize Updating Existing Entities When Using Late Binding
The TrackingEntity class is used to track which attributes are modified and create a final entity that can be used to update an existing entity through the OrganizationService.
Only attributes that have changed will be sent and updated server side.

```csharp
IOrganizationService service = ...
Entity retrievedEntity = service.Retrieve(..)

// Entity may contain n attributes
TrackingEntity entity = new retrievedEntity.ToTrackingEntity();
entity["money"] = 250.0m;

// This will only send money to the CRM server
service.Update(entity.ToFinalEntity());

```
