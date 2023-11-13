resource "null_resource" "function_app_build" {
  triggers = {
    app_md5 = "${filemd5("${path.module}/../../src/Demo.Processing/Demo.Processing.Functions/bin/Debug/net7.0/Demo.Processing.Functions.exe")}"
  }

  provisioner "local-exec" {
    command     = "dotnet build -c Debug /p:DeployOnBuild=true /p:DeployTarget=Package /p:CreatePackageOnPublish=true"
    working_dir = "${path.module}/../../src/Demo.Processing/Demo.Processing.Functions"
  }
}

data "archive_file" "file_function_app" {
    depends_on = [ null_resource.function_app_build ]
  type        = "zip"
  source_dir  = "${path.module}/../../src/Demo.Processing/Demo.Processing.Functions/bin/Debug/net7.0/publish/"
  output_path = "function-app.zip"
}

resource "random_pet" "azurerm_service_plan_name" {
  prefix = "function-service-plan"
}

resource "random_pet" "azurerm_function_app_name" {
  prefix = "function-app"
}

resource "azurerm_service_plan" "app_service_plan" {
  name                = random_pet.azurerm_service_plan_name.id
  location            = var.location
  resource_group_name = var.resource_group_name
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_windows_function_app" "function_app" {
  name                       = random_pet.azurerm_function_app_name.id
  resource_group_name        = var.resource_group_name
  location                   = var.location
  service_plan_id            = azurerm_service_plan.app_service_plan.id
  storage_account_name       = var.storage_account_name
  storage_account_access_key = var.storage_account_primary_access_key
  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE"              = "1",
    "FUNCTIONS_WORKER_RUNTIME"              = "dotnet-isolated",
    "FUNCTIONS_EXTENSION_VERSION"           = "~4",
    "APPLICATIONINSIGHTS_CONNECTION_STRING" = var.app_insights_connection_string,
  }
  site_config {
  }
}

locals {
  deploy_app_command = "az webapp deployment source config-zip --resource-group ${var.resource_group_name} --name ${azurerm_windows_function_app.function_app.name} --src ${data.archive_file.file_function_app.output_path}"
}

resource "null_resource" "function_app_publish" {
  provisioner "local-exec" {
    command = local.deploy_app_command
  }
  depends_on = [local.deploy_app_command]
  triggers = {
    input_json           = filemd5(data.archive_file.file_function_app.output_path)
    publish_code_command = local.deploy_app_command
  }
}
