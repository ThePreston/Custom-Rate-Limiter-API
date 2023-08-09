# AI Gateway 

Capabilities: <br/>
   Track Spending By Product (Cost Chargeback)  <br/>
   Rate Limiting based on spend  <br/>
   Rate Limiting based on Budget Alerts <br/>   
   Logging via Event Hubs to Data Lake Hub <br/>


## Open AI Service, Realtime Cost Tracking And Rate Limiting Per HTTP Request (by Product)


![Picture1](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/1a27d263-f69e-41c0-9f30-7fb9e5d23cf7)


<br/>


## Any Service, Rate Limiting based on Budget (by Product) 


![Picture2](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/8e335ce5-f484-4b39-85f7-6b4accae5d4a)


<br/>



## High Level Architecture


![AI Gateway](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/cc3d5d63-0df0-43b9-923a-7a1a32da487d)



## Methods
<br/>

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
