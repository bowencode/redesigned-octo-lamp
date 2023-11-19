resource "random_pet" "test_name" {
  prefix = "app-test"
}

resource "azurerm_resource_group" "app-test-rg" {
  name     = "${random_pet.test_name.id}-rg"
  location = var.azure_region
}

resource "azurerm_storage_account" "storage-account" {
  name                     = var.storage_account_name
  resource_group_name      = azurerm_resource_group.app-test-rg.name
  location                 = azurerm_resource_group.app-test-rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_storage_container" "blob-container" {
  name                  = var.blob_container_name
  storage_account_name  = azurerm_storage_account.storage-account.name
  container_access_type = "private"
}

resource "azurerm_storage_queue" "company-queue" {
  name                 = "company-form"
  storage_account_name = azurerm_storage_account.storage-account.name
}

resource "azurerm_storage_queue" "individual-queue" {
  name                 = "individual-form"
  storage_account_name = azurerm_storage_account.storage-account.name
}

resource "azurerm_application_insights" "app-test-ai" {
  name                = "${random_pet.test_name.id}-ai"
  location            = azurerm_resource_group.app-test-rg.location
  resource_group_name = azurerm_resource_group.app-test-rg.name
  application_type    = "web"
}

module "sql-server" {
  source              = "./sqlazure"
  resource_group_name = azurerm_resource_group.app-test-rg.name
  location            = azurerm_resource_group.app-test-rg.location
  db_name             = "Customers"
  local_ip_address    = var.local_ip_address
}

resource "random_pet" "kv_name" {
}

locals {
  kv_name        = "${random_pet.kv_name.id}-kv"
  key_vault_url  = "https://${local.kv_name}.vault.azure.net/"
}

module "function-app" {
  source                             = "./functions"
  resource_group_name                = azurerm_resource_group.app-test-rg.name
  location                           = azurerm_resource_group.app-test-rg.location
  storage_account_name               = azurerm_storage_account.storage-account.name
  storage_account_primary_access_key = azurerm_storage_account.storage-account.primary_access_key
  app_insights_connection_string     = azurerm_application_insights.app-test-ai.connection_string
  connection_string                  = module.sql-server.sql_connection_string
}

module "api-app" {
  source                         = "./api"
  resource_group_name            = azurerm_resource_group.app-test-rg.name
  location                       = azurerm_resource_group.app-test-rg.location
  app_insights_connection_string = azurerm_application_insights.app-test-ai.connection_string
  key_vault_url                  = local.key_vault_url
}

module "kv" {
  source                                 = "./kv"
  resource_group_name                    = azurerm_resource_group.app-test-rg.name
  location                               = azurerm_resource_group.app-test-rg.location
  kv_name                                = local.kv_name
  connection_string_secret_name          = "SqlConnectionString"
  connection_string_secret_value         = module.sql-server.sql_connection_string
  storage_connection_string_secret_name  = "FormsStorage"
  storage_connection_string_secret_value = azurerm_storage_account.storage-account.primary_connection_string
  api_app_managed_identity               = module.api-app.api_app_managed_identity
}
