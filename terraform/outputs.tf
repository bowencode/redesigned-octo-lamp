output "sql_connection_string" {
  value     = module.sql-server.sql_connection_string
  sensitive = true
}

output "storage_connection_string" {
  value     = azurerm_storage_account.storage-account.primary_connection_string
  sensitive = true
}

output "api_app_url" {
  value = module.api-app.api_app_url  
}
