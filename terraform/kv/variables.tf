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