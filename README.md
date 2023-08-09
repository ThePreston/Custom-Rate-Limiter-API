# Open AI Cost Gateway Pattern

Capabilities: <br/>
   Track Spending By Product (Cost Chargeback)  <br/>
   Rate Limi By Product based on spending Limits  <br/>



## Open AI Service, Real-Time Cost Tracking And Rate Limiting Per HTTP Request (by Product)


![Picture1](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/1a27d263-f69e-41c0-9f30-7fb9e5d23cf7)


<br/>


## Architecture


![AI Cost Gateway](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/257a293d-83ec-4082-b0d5-2fe584ab1c79)



## Addtional Capabilities - Any Service, Rate Limiting based on Budget (by Product) and Event Hub Logging

Additional Capabilities: <br/>
   Rate Limiting based on Budget Alerts <br/>   
   Logging via Event Hubs to Data Lake Hub <br/>


![Picture2](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/8e335ce5-f484-4b39-85f7-6b4accae5d4a)


<br/>



## High Level Architecture


![AI Gateway](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/cc3d5d63-0df0-43b9-923a-7a1a32da487d)


## Streaming Capbilities
Prompt Tokens are calcuated using Additional Python Function API wrapper that uses TikToken



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
