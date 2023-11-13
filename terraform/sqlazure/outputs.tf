
output "sql_server_name" {
  value = azurerm_mssql_server.server.name
}

output "admin_username" {
  value = var.admin_username
}

output "admin_password" {
  sensitive = true
  value     = local.admin_password
}

output "sql_connection_string" {
    value = "Server=tcp:${azurerm_mssql_server.server.name}.database.windows.net,1433;Initial Catalog=${var.db_name};Persist Security Info=False;User ID=${var.admin_username};Password=${local.admin_password};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}