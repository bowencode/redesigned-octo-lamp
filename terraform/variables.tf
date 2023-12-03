variable "azure_region" {
  description = "Azure Region"
  type        = string
  default     = "westus"
}

variable "storage_account_name" {
  description = "Storage Account Name"
  type        = string
  default     = "tfdemotestsa"
}

variable "blob_container_name" {
  description = "Blob Storage Container Name"
  type        = string
  default     = "received-forms"
}

variable "local_ip_address" {
  description = "IP Address to allow test execution"
  type        = string
  default     = null
}
