# Controllable API
This document describes an idea that makes it possible to define a API-key that can be limited in a way so each key can only call a limited amount of API methods.
Likewise, due to the GDPR, I wanted to make it possible to limit the amount of fields returned to each of the API keys for a specific type of object. That way one can document what kind of data is returned to each of the keys.

Original brainstorm can be found [here](./dumps/MPE.Api.pdf)

The implementation is executed through the MPE.Web.Api project that contains a integration to the DarkSky weather API.

## Implementation
This section describes a bit about the implementation. This is not production ready at all and is not intended to be, but rather be a project to proof a concept.

### Configuration of API access
The basis for this implementation is the Key. It's pretty straightforward, just create a new key in the API_Key table only defining a name. It autogenerates a GUID as the API-key. It is also possible here to define the key as a Admin - that enabling all fields without any restrictions.

When this is done you can now insert records in the API_Method table, defining which methods the key have access to. These value defined in the field "Method" should be equal the value of the attributed defined on the API method in the controller.

When this is done you have to find define what the fields the key should have returned. This is only done for the classes decorated with the RestrictSerialization attribute - all other classes returns all fields.

There are two ways to define this configuration. By declaring only a TypeAlias - that enabling the key to get the entire object returned. Or you can define both a TypeAlias and a FieldName - that only returning the fields defined in the table for that type.

### Decorating through attributes
This small setup is using attributes to controll the functionality. 

#### RestrictMethod
This is intended to decorate API methods in the controller. When it is executed it will do a lookup in the database to check if the API key defined in Authorization header is allowed to call the method. The method name is defined in the attribute initialization.

#### RestrictSerialization
This defines that a specific class is under the limitation during return of the serialized data. You should define a type alias that is used for mapping in the database so it is possible to define that a key have access to a field or the entire type.

#### RestrictSerializationController
To enable the limited serialization you have to decorate the controller with this attribute. This takes the controller configuration and sets the contract resolver to use the ApiLimitedFieldContractResolver implementation that only serializes what have been configured in the database.

### Data model
The following are the different models with fields that is the foundation of the API setup.

Common for all the tables would be the following fields:
* [Table name]ID
* CreatedOn
* Deleted

__Api_Key__  
Table containing a key in the form of a GUID, along with the name of the user 
* Name
* Key

__Api_KeyMethod__  
Relation to the key to define which method the specific key have access to.
* KeyID
* Method

__Api_KeyField__  
Relation to the key to define which field is returned for a specific type of object. This can be free-text or enum values.
* KeyID
* TypeAlias
* FieldName

__Api_Log__  
A table to capture all the valid requests and their responses including codes and content returned.
* KeyID
* RequestContent
* ReqeustQuery
* RequestTimeStamp
* ResponseStatus
* ResponseContent
* ResponseTimestamp


### ApiControllers
One of the things that can be problematic are if you limit a clients access to an object without them knowing what the entire object contains. If they know that it contains extra fields that could be important for them, but they didnt have access to it, then they could make a direct inquery for access.

To do this the project contains a small API Controller that contains two methods. One for returning the names of the types marked with the RestrictedSerialization attribute, and another to generate a specific type by its type alias defined in the same attribute.

That way the user can get an idea of the total structure of the object and see if they need to get access to more fields.

## Whats missing
Implement a DelegatingHandler to plugin into the pipeline and log API calls

Handle personal sensitive issues in the HTTP request log on the server?