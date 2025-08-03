provider "azurerm" {
  features {}

  client_id       = var.client_id
  client_secret   = var.client_secret
  tenant_id       = var.tenant_id
  subscription_id = var.subscription_id
}

resource "azurerm_resource_group" "main" {
  name     = "ribbitreels-rg"
  location = "East US"
}

resource "azurerm_service_plan" "app_service_plan" {
  name                = "ribbitreels-plan"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  os_type             = "Linux"
  sku_name            = "B1"
}

resource "azurerm_linux_web_app" "api_app" {
  name                = "ribbitreels-app"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  service_plan_id     = azurerm_service_plan.app_service_plan.id

  site_config {
    application_stack {
      docker_image_name   = "ghcr.io/${var.github_owner}/ribbitreels-api:latest"
      docker_registry_url = "https://ghcr.io"
    }
  }

  app_settings = {
    WEBSITES_PORT                  = "8080" # Match Dockerfile's EXPOSE
    DOCKER_REGISTRY_SERVER_URL    = "https://ghcr.io"
    DOCKER_REGISTRY_SERVER_USERNAME = var.github_owner
    DOCKER_REGISTRY_SERVER_PASSWORD = var.github_token
  }

  identity {
    type = "SystemAssigned"
  }
}
