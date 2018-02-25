# Controllable API
This document describes an idea that makes it possible to define a API-key that can be limited in a way so each key can only call a limited amount of API methods.
Likewise, due to the GDPR, I wanted to make it possible to limit the amount of fields returned to each of the API keys for a specific type of object. That way one can document what kind of data is returned to each of the keys.

## Data model
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
* TypePath
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

## Implementation

Define a attribute for the API method declarations
* Check the if there exists a Authorized header against the API_Key table + the API_Method table

Define a attribute to define that the decorated type should be limited for fields at serialization

Define ContractRezolver that removes non-allowed fields for a API key
* Controller attribute set by defining a IControllerConfiguration attribute decorated on the controller
  * ControllerSettings.Formatters.JsonFormatter

API for generating JSON schema for given type - with all fields

Implement a DelegatingHandler to plugin into the pipeline and log API calls

Handle personal sensitive issues in the HTTP request log on the server?