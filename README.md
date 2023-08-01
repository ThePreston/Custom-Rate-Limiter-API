# AI Gateway 

Provides Additional Rate Limiting Capabilities based on anything one might need, this specific example uses Spending Limits
additionally, enable Chargebacks
<br/>
APIM Policy Fragment Capabilities:
   Log to Event Hub
   Call Function App and determine if Subscriptionb needs to be rate limited
   Receive Budget Alert to Rate Limit Specific Subscriptions
   

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


## High Level Architecture


![AI Gateway](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/fb141731-225f-44a9-91e1-74a74b79fa58)


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

