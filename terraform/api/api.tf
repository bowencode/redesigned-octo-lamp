resource "null_resource" "api_app_build" {
  triggers = {
    app_md5 = "${filemd5("${path.module}/../../src/Demo.Processing/Demo.Processing.Api/bin/Debug/net7.0/Demo.Processing.Api.dll")}"
  }

  provisioner "local-exec" {
    command     = "dotnet publish -c Debug Demo.Processing.Api.csproj"
    working_dir = "${path.module}/../../src/Demo.Processing/Demo.Processing.Api"
  }
}

data "archive_file" "file_api_app" {
  depends_on  = [null_resource.api_app_build]
  type        = "zip"
  source_dir  = "${path.module}/../../src/Demo.Processing/Demo.Processing.Api/bin/Debug/net7.0/publish/"
  output_path = "api-app.zip"
}

resource "random_pet" "azurerm_service_plan_name" {
  prefix = "api-service-plan"
}

resource "random_pet" "azurerm_api_app_name" {
  prefix = "api-app"
}

resource "azurerm_service_plan" "app_service_plan" {
  name                = random_pet.azurerm_service_plan_name.id
  location            = var.location
  resource_group_name = var.resource_group_name
  os_type             = "Windows"
  sku_name            = "D1"
}

resource "azurerm_windows_web_app" "api_app" {
  name                = random_pet.azurerm_api_app_name.id
  resource_group_name = var.resource_group_name
  location            = var.location
  service_plan_id     = azurerm_service_plan.app_service_plan.id
  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE"                        = "1",
    "APPLICATIONINSIGHTS_CONNECTION_STRING"           = var.app_insights_connection_string,
    "APPINSIGHTS_INSTRUMENTATIONKEY"                  = var.app_insights_instrumentation_key,
    "APPINSIGHTS_PROFILERFEATURE_VERSION"             = "1.0.0",
    "APPINSIGHTS_SNAPSHOTFEATURE_VERSION"             = "1.0.0",
    "ApplicationInsightsAgent_EXTENSION_VERSION"      = "~2",
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE"                 = "false",
    "KeyVaultUrl"                                     = var.key_vault_url,
    "AzureAd:Authority"                               = var.azuread_authority,
    "AzureAd:Audience"                                = var.azuread_audience,
    "DiagnosticServices_EXTENSION_VERSION"            = "~3",
    "InstrumentationEngine_EXTENSION_VERSION"         = "disabled",
    "SnapshotDebugger_EXTENSION_VERSION"              = "disabled",
    "XDT_MicrosoftApplicationInsights_BaseExtensions" = "disabled",
    "XDT_MicrosoftApplicationInsights_Mode"           = "recommended",
    "XDT_MicrosoftApplicationInsights_PreemptSdk"     = "1",
    "XDT_MicrosoftApplicationInsights_Java"           = "1",
    "XDT_MicrosoftApplicationInsights_NodeJS"         = "1",
  }
  site_config {
    always_on = false
  }
  identity {
    type = "SystemAssigned"
  }
}

locals {
  depends_on         = [azurerm_windows_web_app.api_app]
  deploy_app_command = "az webapp deployment source config-zip --resource-group ${var.resource_group_name} --name ${azurerm_windows_web_app.api_app.name} --src ${data.archive_file.file_api_app.output_path}"
}

resource "null_resource" "api_app_publish" {
  depends_on = [local.deploy_app_command, data.archive_file.file_api_app]
  triggers = {
    input_zip            = data.archive_file.file_api_app.output_md5
    publish_code_command = local.deploy_app_command
  }
  provisioner "local-exec" {
    command = local.deploy_app_command
  }
}
