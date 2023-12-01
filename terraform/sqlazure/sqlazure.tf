resource "random_pet" "azurerm_mssql_server_name" {
  prefix = "sql"
}

resource "random_password" "admin_password" {
  count       = var.admin_password == null ? 1 : 0
  length      = 20
  special     = false
  min_numeric = 1
  min_upper   = 1
  min_lower   = 1
}

locals {
  admin_password = try(random_password.admin_password[0].result, var.admin_password)
}

resource "azurerm_mssql_server" "server" {
  name                         = random_pet.azurerm_mssql_server_name.id
  resource_group_name          = var.resource_group_name
  location                     = var.location
  administrator_login          = var.admin_username
  administrator_login_password = local.admin_password
  version                      = "12.0"
}

resource "azurerm_mssql_database" "db" {
  name      = var.db_name
  server_id = azurerm_mssql_server.server.id
  sku_name  = "Basic"
}

module "ip_address" {
    source = "../myip"
}

resource "azurerm_mssql_firewall_rule" "firewall-local-ip" {
    name                        = "${azurerm_mssql_server.server.name}-firewall"
    server_id                   = azurerm_mssql_server.server.id
    start_ip_address            = var.local_ip_address == null ? module.ip_address.ip : var.local_ip_address
    end_ip_address              = var.local_ip_address == null ? module.ip_address.ip : var.local_ip_address
}

resource "azurerm_mssql_firewall_rule" "firewall-azure" {
    name                        = "${azurerm_mssql_server.server.name}-firewall-azure"
    server_id                   = azurerm_mssql_server.server.id
    start_ip_address            = "0.0.0.0"
    end_ip_address              = "0.0.0.0"
}

resource "null_resource" "db_setup" {
    depends_on = [azurerm_mssql_database.db]

    triggers = {
      scripts = file("${path.module}/dbSetup.sql")
    }

    provisioner "local-exec" {
        command = "sqlcmd -d ${var.db_name} -U ${var.admin_username} -P ${local.admin_password} -S ${azurerm_mssql_server.server.name}.database.windows.net -i ${path.module}/dbSetup.sql"
    }
}