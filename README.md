# Open AI Cost Gateway Pattern

Real-Time Capabilities: <br/>
   Track Spending & Tokens By Product (Cost Chargeback) for every Request, incliding streaming <br/>
   Rate Limit By Product based on spending Limits (default) or Tokens (429 Rate Limiting Response when Spending/Token limit has been reached ) <br/>


## Architecture


![AI Cost Gateway](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/c0992f84-5d3b-4799-9d87-b3e0e82fcb21)



## Open AI Service, Real-Time Cost Tracking And Rate Limiting Per HTTP Request (by Product)


![Picture1](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/1a27d263-f69e-41c0-9f30-7fb9e5d23cf7)


<br/>

## Addtional Capabilities - Any Service, Rate Limiting based on Budget (by Product) and Event Hub Logging

Additional Capabilities: <br/>
   Rate Limiting based on Budget Alerts <br/>   
   Logging via Event Hubs to Data Lake Hub <br/>


![Picture2](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/8e335ce5-f484-4b39-85f7-6b4accae5d4a)


<br/>



## High Level Architecture of all Features in the repo

<br/>
Open AI Transactional Cost Tracking and Rate limiting <br/>
Budget Alert Rate Limiting <br/>
Event Hub Logging <br/>

<br/>


![AI Gateway](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/cc3d5d63-0df0-43b9-923a-7a1a32da487d)


## Streaming Capabilities
Streaming responses do not include Token Information, that must be calculated <br/>
Prompt Tokens are calcuated using Additional Python Function API wrapper that uses TikToken : <br/>

https://github.com/awkwardindustries/dossier/tree/main/samples/open-ai/tokenizer/azure-function-python-v2


## Methods

1) Create
2) Update
3) Budget Alert Endpoint
4) GetAll
5) GetById

   
<br/>


## AOAI Swagger

Repo: <br/>
   [azure-rest-api-specs/specification/cognitiveservices/data-plane/AzureOpenAI/](https://github.com/Azure/azure-rest-api-specs/tree/main/specification/cognitiveservices/data-plane/AzureOpenAI)

JSON Repo:
https://github.com/Azure/azure-rest-api-specs/blob/main/specification/cognitiveservices/data-plane/AzureOpenAI/inference/stable/2023-05-15/inference.json

JSON File URI:
https://raw.githubusercontent.com/Azure/azure-rest-api-specs/main/specification/cognitiveservices/data-plane/AzureOpenAI/inference/stable/2023-05-15/inference.json

<br/>


## Budget Alerts

Latency: </br>
Cost and usage data is typically available within 8-24 hours and budgets are evaluated against these costs every 24 hours.

<br/>
Documentation: <br/>
https://learn.microsoft.com/en-us/azure/cost-management-billing/costs/tutorial-acm-create-budgets


## FAQ

Cost API: </br>
Attempted this but Proved to be Overly Complicated. Cost and usage data is typically available within 8-24 hours. 
would have to create a polling mechanism to call Cost API for each resource to be monitored
</br>

Streaming Responses: </br>
when "Stream" : true added to JSON payload, No Token information is provided by Open AI Service.  </br>
Prompt Tokens are calculated using a Python Function (PyTokenizer) that wraps a BPE Tokenizer library TikToken </br>
Completion Tokens are calculated by counting the SSE responses and subtracting 2 </br>


Granularity of Cost Tracking: </br>
Solution uses APIM Product Subscription Keys but can also be used against individual ID's, header value, etc
