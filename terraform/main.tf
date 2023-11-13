resource "azurerm_resource_group" "app-test-rg" {
  name = "${var.resource_group_name}"
  location = "${var.azure_region}"
}

resource "azurerm_storage_account" "storage-account" {
  name                     = "${var.storage_account_name}"
  resource_group_name      = azurerm_resource_group.app-test-rg.name
  location                 = azurerm_resource_group.app-test-rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_storage_container" "blob-container" {
  name                  = "${var.blob_container_name}"
  storage_account_name  = azurerm_storage_account.storage-account.name
  container_access_type = "private"
}