# Open AI API Gateway - Custom Rate Limiter

Provides Additional Rate Limiting Capabilities based on anything one might need, this specific example uses SPEND
<br/>
## Methods
<br/>

1) Create
2) Update
3) GetAll
4) GetById

   
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




![APIPIC](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/832ce32e-1b4c-45f5-b4e7-ab0964f3de68)



## AOAI Swagger

https://github.com/Azure/azure-rest-api-specs/tree/main/specification/cognitiveservices/data-plane/AzureOpenAI

https://raw.githubusercontent.com/Azure/azure-rest-api-specs/main/specification/cognitiveservices/data-plane/AzureOpenAI/inference/stable/2023-05-15/inference.json

