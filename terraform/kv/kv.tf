data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "key_vault" {
  name                        = var.kv_name
  location                    = var.location
  resource_group_name         = var.resource_group_name
  enabled_for_disk_encryption = true
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false

  sku_name = "standard"

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = [
      "Get",
      "List",
      "Set",
      "Delete",
      "Backup",
      "Restore",
      "Purge",
    ]
  }

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = var.api_app_managed_identity

    secret_permissions = [
      "Get",
      "List",
    ]
  }
}

resource "azurerm_key_vault_secret" "connection_string_secret" {
  name         = var.connection_string_secret_name
  value        = var.connection_string_secret_value
  key_vault_id = azurerm_key_vault.key_vault.id
}

resource "azurerm_key_vault_secret" "storage_connection_string_secret" {
  name         = var.storage_connection_string_secret_name
  value        = var.storage_connection_string_secret_value
  key_vault_id = azurerm_key_vault.key_vault.id
}
