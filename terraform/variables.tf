variable "azure_region" {
  description = "Azure Region"
  type        = string
  default     = "eastus"
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

variable "api_app_registration_name" {
  description = "API App Registration Name"
  type        = string
  default     = "TfDemoApiApp"
}

variable "test_app_registration_name" {
  description = "Test App Registration Name"
  type        = string
  default     = "IntegrationTestApp"
}