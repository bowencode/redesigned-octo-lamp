terraform {
  required_providers {
    azapi = {
      source  = "azure/azapi"
      version = "=1.10.0"
    }
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.80.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~>3.5"
    }
  }
}

provider "azapi" {
  default_location = "eastus"
  default_tags = {
    team = "My Dev Testing"
  }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
  }
}