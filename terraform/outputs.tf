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

output "test_app_client_id" {
  value = data.azuread_application.test-app-registration.client_id  
}

output "test_app_client_secret" {
  value     = azuread_application_password.test-app-auth-secret.value
  sensitive = true  
}
