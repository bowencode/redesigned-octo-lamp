variable "resource_group_name" {
  description = "Resource Group Name"
  type        = string
  nullable    = false
}

variable "location" {
  description = "Azure Region"
  type        = string
  nullable    = false
}

variable "app_insights_instrumentation_key" {
  description = "Application Insights Instrumentation Key"
  type        = string
  nullable    = false  
}

variable "app_insights_connection_string" {
  description = "Application Insights Connection String"
  type        = string
  nullable    = false  
}

variable "key_vault_url" {
  description = "Key Vault URL"
  type        = string
  nullable    = false
}

variable "azuread_authority" {
  description = "Azure AD Authority"
  type        = string
  nullable    = false
}

variable "azuread_audience" {
  description = "Azure AD Audience"
  type        = string
  nullable    = false  
}