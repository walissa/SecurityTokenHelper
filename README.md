# SecurityTokenHelper
[![Build status](https://dev.azure.com/mytestings/Integrations/_apis/build/status/SecurityTokenHelper)](https://dev.azure.com/mytestings/Integrations/_build/latest?definitionId=30)

SecurityTokenHelper is a WCF endpoint behavior extension that helps authenticating calls in BizTalk, it also contains TokenHelper class that use to get token with without using WCF.

## TokenHelper Functions

 - string GetBasicAuthEncoded(string username, string password): encodes username:password string in Base64 format to be used in basic auth header.
 - GetToken
 ```
 string GetToken(HttpMethodEnum method, string url, HeaderCollection headers, string contentType, string body, string tokenPath)
 string GetToken(HttpMethodEnum method, string url, HeaderCollection headers, string contentType, string body, string tokenPath, Guid tokenId, int tokenExpiresIn, bool cachedToken)
 ```
### Parameters
| Parameter| Type| Description |
|--|--|--|
| method | HttpMethodEnum (Required) | The http method used when calling the token service endpoint.<br/>[POST (Default),GET]
| url | string (Required) | The token service endpoint url.
| headers | HeaderCollection (Optional) | Headers to be used to make successful call.
| contentType | string (Optional) | The content-type for the body is being sent in the call, Required if the body parameter is not empty |
| body | string (Optional) | The body of the call.
| tokenPath | string (Required)| The XPath or JPath for the token value e.g. for JSON: $..access_token, For Xml: /Token/Access_Token.|
|cachedToken| bool (Optional)| determines if the token to be cached, or retrieved from Token Cache|
| tokenId | guid (Optional) | Token identifier, must be determined if cachedToken is set to true.|
| expriesIn | int (Optional) | the period in seconds for the token before it expires, required if cachedToken is set to true, and must have a value greater than 0|

## SecurityTokenHelperBehavior
The WCF Behavior extension to be used with WCF bindings to retrieve the token from a token service endpoint before performing the main call.

### Properties

|Property | Type| Description |
|--|--|--|
| HttpMethod | HttpMethodEnum (Required) | The http method used when calling the token service endpoint.<br/>[POST (Default),GET]
| AuthEndpoint | string (Required) | The token service endpoint url.
| Headers| HeaderCollection (Optional) | Headers to be used to make successful call.
| ContentType| string (Optional) | The content-type for the body is being sent in the call, Required if Body property is not empty |
| Body | string (Optional) | The body of the call.
| TokenPath | string (Required)| The XPath or JPath for the token value e.g. for JSON: $..access_token, For Xml: /Token/Access_Token.|
|CacheToken| bool (Optional)| determines if the token to be cached, or retrieved from Token Cache|
| TokenId| guid (Optional) | Token identifier, must be determined if CacheToken is set to true.|
| TokenExpiresIn| int (Optional) | the period in seconds for the token before it expires, required if CacheToken is set to true, and must have a value greater than 0|
|TokenUsage| TokenUsageEnum(Required) | Specifies where to use the retrieved token in the main call, [Header (Default),QueryParameter]|
|TokenKey| string (Required)| the name of the header or the query string parameter that holds the retrieved token e.g. Authorization|
|TokenPrefix| string(Optional)| a prefix to be added before the token e.g. Bearer |
|TokenSuffix| string (Optional) | a suffix to be added to the token |


## SecurityTokenHelperElement
This is the WCF behavior extension element, that will load all the configuration from the config file, and it presents the behavior extension in BizTalk.

### Properties

|Property | Type| Description |
|--|--|--|
| HttpMethod | HttpMethodEnum (Required) | The http method used when calling the token service endpoint.<br/>[POST (Default),GET]
| AuthEndpoint | string (Required) | The token service endpoint url.
| Headers| string(Optional) | Headers to be used to make successful call, this is multiline property and each header is represented in a single line.
| ContentType| string (Optional) | The content-type for the body is being sent in the call, Required if Body property is not empty |
| Body | string (Optional) | The body of the call.
| TokenPath | string (Required)| The XPath or JPath for the token value e.g. for JSON: $..access_token, For Xml: /Token/Access_Token.|
|CacheToken| bool (Optional)| determines if the token to be cached, or retrieved from Token Cache|
| TokenId| guid (Optional) | Token identifier, must be determined if CacheToken is set to true.|
| TokenExpiresIn| int (Optional) | the period in seconds for the token before it expires, required if CacheToken is set to true, and must have a value greater than 0|
|TokenUsage| TokenUsageEnum(Required) | Specifies where to use the retrieved token in the main call, [Header (Default),QueryParameter]|
|TokenKey| string (Required)| the name of the header or the query string parameter that holds the retrieved token e.g. Authorization|
|TokenPrefix| string(Optional)| a prefix to be added before the token e.g. Bearer |
|TokenSuffix| string (Optional) | a suffix to be added to the token |
## Remarks


## Installation
Download and install the latest release.
This extension is designed to be used with BizTalk, but it can also be used with other applications.
There are two ways to add this extension to the configuration, it can be added to the global machine.config file, or it can be added to the host instance only.

###  Adding the extension to machine.config
the extension must be added to both .Net framework versions (32 bit and 64 bit).


`%windir%\Microsoft.Net\Framework\<.NET Version>\config\machine.config`

`%windir%\Microsoft.Net\Framework64\<.NET Version>\config\machine.config`


locate behaviorExtensions section and add the following line
```
<add name="SecurityTokenHelper" type="BizTalkComponents.WCFExtensions.SecurityTokenHelper.SecurityTokenHelperElement, BizTalkComponents.WCFExtensions.SecurityTokenHelper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f8600b34c8012b7b" />
```

### Adding the extension to BizTalk Host Instance
In BizTalk Administrator Console, go to Platform Settings --> Adapters  and select the WCF adapter you would like to add the extension to, then select the send handler you want to make the extension available with, right click on it and then select properties, Click Properties button in Adapter Handler Properties, then select WCF extensions, copy the code below to a new Xml file then click on Import.
```
<configuration>
  <system.serviceModel>
    <extensions>
      <behaviorExtensions>
          <add name="SecurityTokenHelper" type="BizTalkComponents.WCFExtensions.SecurityTokenHelper.SecurityTokenHelperElement, BizTalkComponents.WCFExtensions.SecurityTokenHelper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f8600b34c8012b7b" />
      </behaviorExtensions>
    </extensions>
  </system.serviceModel>
</configuration>
```

