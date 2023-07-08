# Open AI API Gateway - Custom Rate Limiter

Provides Additional Rate Limiting Capabilities based on anything one might need, this specific example uses SPEND

## Methods

1) Create
2) Update
3) GetAll
4) GetById

## JSON object KVQuota (KeyValue representation for Amount by Subscription)
{
"SubscriptionKey":"",
"Amount" : ""
}

## JSON object QuotaEntry (used for decrementing Transactions)
{
"SubscriptionKey":"",
"Model" : "",
"PrompTokens":"",
"CompletionTokens" : "",
"TotalTokens":""
}

## High Level Architecture

![APIPIC](https://github.com/ThePreston/Custom-Rate-Limiter-API/assets/84995595/832ce32e-1b4c-45f5-b4e7-ab0964f3de68)
