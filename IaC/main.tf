terraform {

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=3.0.0"

    }
  }

}

//Step 1, Set All Variables
provider "azurerm" {
  features {}

  subscription_id =  var.subscriptionid

}

//Step 2, Provision Tag Schema
locals {
  tags = {
    environment = "prod"
    department = "IT"
    source = "terraform"
    contactemail = "${var.contactemail}"
  }
}

resource "azurerm_resource_group" "perftestgroup" {
  name     = "${var.projectnamingconvention}-rg"
  location = var.location

  tags = local.tags
}

resource "azurerm_storage_account" "storageQuotaApp" {
  name                     = "${var.projectnamingconvention}quotaappsto"
  resource_group_name      = azurerm_resource_group.perftestgroup.name
  location                 = azurerm_resource_group.perftestgroup.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = local.tags
}

resource "azurerm_storage_account" "storageTokenizerApp" {
  name                     = "${var.projectnamingconvention}tokenizersto"
  resource_group_name      = azurerm_resource_group.perftestgroup.name
  location                 = azurerm_resource_group.perftestgroup.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = local.tags
}

//create log analytics workspace
resource "azurerm_log_analytics_workspace" "law" {
  name                = "${var.projectnamingconvention}-law"
  location            = azurerm_resource_group.perftestgroup.location
  resource_group_name = azurerm_resource_group.perftestgroup.name
  sku                 = "PerGB2018"
  retention_in_days   = 30

  tags = local.tags
}

//create workspace-based app insights instance 
resource "azurerm_application_insights" "appinsights" {
  name                = "${var.projectnamingconvention}-appinsights"
  location            = azurerm_resource_group.perftestgroup.location
  resource_group_name = azurerm_resource_group.perftestgroup.name
  application_type    = "web"
  retention_in_days   = 30
  
  workspace_id = azurerm_log_analytics_workspace.law.id

  tags = local.tags
}

resource "azurerm_service_plan" "functionserviceplan" {
  name                = "${var.projectnamingconvention}-asp-win-1"
  resource_group_name = azurerm_resource_group.perftestgroup.name
  location            = azurerm_resource_group.perftestgroup.location
  
  maximum_elastic_worker_count = 50
  
  os_type             = "Windows"
  sku_name            = "EP2"

  tags = local.tags
}

resource "azurerm_windows_function_app" "Quotafunction" {
  name                = "${var.projectnamingconvention}-quota-func-1"
  location            = azurerm_resource_group.perftestgroup.location
  resource_group_name = azurerm_resource_group.perftestgroup.name
  service_plan_id     = azurerm_service_plan.functionserviceplan.id

  storage_account_name       = azurerm_storage_account.storageQuotaApp.name
  storage_account_access_key = azurerm_storage_account.storageQuotaApp.primary_access_key
  
  //add app insights
  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.appinsights.instrumentation_key
  }

  identity {
    type = "SystemAssigned"
  }

  site_config {
    application_stack {
      dotnet_version = "v6.0"
    }
    elastic_instance_minimum = 5
  }
  
  tags = local.tags
}

resource "azurerm_service_plan" "pyfunctionserviceplan" {
  name                = "${var.projectnamingconvention}-asp-linux-1"
  resource_group_name = azurerm_resource_group.perftestgroup.name
  location            = azurerm_resource_group.perftestgroup.location
  
  maximum_elastic_worker_count = 50
  
  os_type             = "Linux"
  sku_name            = "EP2"
  
  tags = local.tags
}

resource "azurerm_linux_function_app" "tokenizerfunction" {
  name                = "${var.projectnamingconvention}-tokenizer-func-1"
  location            = azurerm_resource_group.perftestgroup.location
  resource_group_name = azurerm_resource_group.perftestgroup.name
  service_plan_id     = azurerm_service_plan.pyfunctionserviceplan.id

  storage_account_name       = azurerm_storage_account.storageTokenizerApp.name
  storage_account_access_key = azurerm_storage_account.storageTokenizerApp.primary_access_key

  identity {
    type = "SystemAssigned"
  }

  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.appinsights.instrumentation_key
  }

  site_config {
    application_stack {
      python_version = "3.10"
    }
    elastic_instance_minimum = 5
  }
  
  tags = local.tags
}

resource "azurerm_redis_cache" "RedisCache" {
  name                = "${var.projectnamingconvention}rediscache"
  location            = azurerm_resource_group.perftestgroup.location
  resource_group_name = azurerm_resource_group.perftestgroup.name
  capacity            = 1
  family              = "P"
  sku_name            = "Premium"
  enable_non_ssl_port = false
  minimum_tls_version = "1.2"

  redis_configuration {
  }
  

  tags = local.tags
}

resource "azurerm_storage_account" "storageSolution" {
  name                     = "${var.projectnamingconvention}gatewaytablesto"
  resource_group_name      = azurerm_resource_group.perftestgroup.name
  location                 = azurerm_resource_group.perftestgroup.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  
  tags = local.tags
}

resource "azurerm_storage_table" "kvTable" {
  name                 = "AIKeyValueStore"
  storage_account_name = azurerm_storage_account.storageSolution.name
  depends_on = [ azurerm_storage_account.storageSolution ]
  
}

resource "azurerm_eventhub_namespace" "main" {
  name                = "${var.projectnamingconvention}ehns"
  location            = azurerm_resource_group.perftestgroup.location
  resource_group_name = azurerm_resource_group.perftestgroup.name
  sku                 = "Standard"
  capacity            = 5
  
  tags = local.tags
}

resource "azurerm_eventhub" "main" {
  name                = "${var.projectnamingconvention}eh"
  namespace_name      = azurerm_eventhub_namespace.main.name
  resource_group_name = azurerm_resource_group.perftestgroup.name
  partition_count     = 2
  message_retention   = 1

  depends_on = [ azurerm_eventhub_namespace.main ]
}

resource "azurerm_api_management" "apim" {
  name                = "${var.projectnamingconvention}apim"
  location            = azurerm_resource_group.perftestgroup.location
  resource_group_name = azurerm_resource_group.perftestgroup.name
  publisher_name      = "${var.companyname}"
  publisher_email     = "${var.contactemail}"
  sku_name            = "Premium_1"

  tags = local.tags
}


/*
// Cosmeos DB Capability for Multi-region HADR

resource "azurerm_cosmosdb_account" "CosmosDB" {
  name                = "${var.projectnamingconvention}cosmosdb"
  location            = azurerm_resource_group.perftestgroup.location
  resource_group_name = azurerm_resource_group.perftestgroup.name
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"
  consistency_policy {
    consistency_level       = "Session"
    max_interval_in_seconds = 5
    max_staleness_prefix    = 100
  }
  enable_automatic_failover = false
  geo_location {
    location          = azurerm_resource_group.perftestgroup.location
    failover_priority = 0
  }

  enable_multiple_write_locations = false
  
  tags = local.tags
}

resource "azurerm_cosmosdb_sql_database" "main" {
  name                = "${var.projectnamingconvention}cosmossqldb"
  resource_group_name = azurerm_resource_group.perftestgroup.name
  account_name        = azurerm_cosmosdb_account.CosmosDB.name
  depends_on = [ azurerm_cosmosdb_account.CosmosDB ]
  
  tags = local.tags
}

resource "azurerm_cosmosdb_sql_container" "main" {
  name                = "${var.projectnamingconvention}container"
  resource_group_name = azurerm_resource_group.perftestgroup.name
  account_name        = azurerm_cosmosdb_account.CosmosDB.name
  database_name       = azurerm_cosmosdb_sql_database.main.name
  partition_key_path  = "/id"
  throughput          = 4000
  depends_on = [ azurerm_cosmosdb_account.CosmosDB , azurerm_cosmosdb_sql_database.main ]
  
  tags = local.tags
}

*/
