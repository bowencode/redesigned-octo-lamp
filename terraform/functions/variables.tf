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

variable "storage_account_name" {
  description = "Storage Account Name"
  type        = string
  nullable    = false
}

variable "storage_account_primary_access_key" {
  description = "Storage Account Primary Access Key"
  type        = string
  nullable    = false
}

variable "app_insights_connection_string" {
  description = "Application Insights Connection String"
  type        = string
  nullable    = false  
}

variable "connection_string" {
  description = "SQL Connection String"
  type        = string
  nullable    = false
}