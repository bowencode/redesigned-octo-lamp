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

variable "kv_name" {
  description = "Key Vault Name"
  type        = string
  nullable    = false
}

variable "connection_string_secret_name" {
  description = "Connection String Secret Name"
  type        = string
  nullable    = false
}

variable "connection_string_secret_value" {
  description = "Connection String Secret Value"
  type        = string
  nullable    = false
}

variable "storage_connection_string_secret_name" {
  description = "Storage Connection String Secret Name"
  type        = string
  nullable    = false
}

variable "storage_connection_string_secret_value" {
  description = "Storage Connection String Secret Value"
  type        = string
  nullable    = false
}

variable "api_app_managed_identity" {
  description = "API App Managed Identity"
  type        = string
  nullable    = false  
}