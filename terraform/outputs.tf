output "SqlConnectionString" {
  value     = module.sql-server.sql_connection_string
  sensitive = true
}

output "StorageConnectionString" {
  value     = azurerm_storage_account.storage-account.primary_connection_string
  sensitive = true
}

output "ApiAppUrl" {
  value = module.api-app.api_app_url  
}

output "TestAppClientId" {
  value = data.azuread_application.test-app-registration.client_id  
}

output "TestAppClientSecret" {
  value     = azuread_application_password.test-app-auth-secret.value
  sensitive = true  
}

output "ApiAppAuthority" {
  value = local.api_app_authority  
}

output "ApiAppAudience" {
  value = local.api_app_audience  
}

output "ApiAppScope" {
  value = "${local.api_app_audience}/.default" 
}
