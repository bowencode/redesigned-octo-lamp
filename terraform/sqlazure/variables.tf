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

variable "db_name" {
  type        = string
  description = "The name of the SQL Database."
  nullable    = false
}

variable "admin_username" {
  type        = string
  description = "The administrator username of the SQL logical server."
  default     = "dmtadmin"
}

variable "admin_password" {
  type        = string
  description = "The administrator password of the SQL logical server."
  sensitive   = true
  default     = null
}

variable "local_ip_address" {
  description = "IP Address to allow test execution"
  type        = string
  default     = null
}