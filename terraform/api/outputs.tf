output "api_app_managed_identity" {
  value = azurerm_windows_web_app.api_app.identity[0].principal_id
}

output "api_app_url" {
  value = "https://${azurerm_windows_web_app.api_app.default_hostname}"
}
