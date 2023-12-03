resource "random_pet" "test_name" {
  prefix = "demo"
}

resource "azurerm_resource_group" "demo-rg" {
  name     = "${random_pet.test_name.id}-rg"
  location = "westus"
}

resource "azurerm_storage_account" "storage-account" {
  name                     = "mytfintrodemosa"
  resource_group_name      = azurerm_resource_group.demo-rg.name
  location                 = azurerm_resource_group.demo-rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}
