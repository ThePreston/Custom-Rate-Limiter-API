# AI Gateway 

Capabilities: <br/>
   Logging via Event Hubs to Data Lake Hub <br/>
   Track Spending (Cost Chargeback)  <br/>
   Rate Limiting based on spend  <br/>
   Rate Limiting based on Budget Alerts 
   <br/>

## Open AI Service, Realtime Cost Tracking And Rate Limiting (calculated by tokens) Per HTTP Request (by Product)


![Picture1](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/159c2362-4ef6-48d2-87dd-ba51ee3cccfd)

<br/>


## Any Service, Rate Limiting based on Budget (by Product) 


![Picture2](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/1b2a987b-0baf-4e05-9d13-8a05779b7017)


<br/>



## High Level Architecture


![AI Gateway](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/fb141731-225f-44a9-91e1-74a74b79fa58)




## Methods
<br/>

1) Create
2) Update
3) Budget Alert Endpoint
4) GetAll
5) GetById

   
## JSON object KVQuota 
(KeyValue representation for Amount by Subscription)

{ <br/>
&nbsp;&nbsp;&nbsp;&nbsp;"SubscriptionKey":"", <br/>
&nbsp;&nbsp;&nbsp;&nbsp;"Amount" : "" <br/>
}

    
## JSON object QuotaEntry
(used for decrementing Transactions)

<br/>
{<br/>
&nbsp;&nbsp;&nbsp;&nbsp;"SubscriptionKey":"",<br/>
&nbsp;&nbsp;&nbsp;&nbsp;"Model" : "",<br/>
&nbsp;&nbsp;&nbsp;&nbsp;"PrompTokens":"",<br/>
&nbsp;&nbsp;&nbsp;&nbsp;"CompletionTokens" : "",<br/>
&nbsp;&nbsp;&nbsp;&nbsp;"TotalTokens":""<br/>
}<br/>
<br/>
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
