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

resource "azurerm_container_app_environment" "env" {
  name                = "ribbitreels-env"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
}

resource "azurerm_container_app" "api" {
  name                         = "ribbitreels-api"
  container_app_environment_id = azurerm_container_app_environment.env.id
  resource_group_name          = azurerm_resource_group.main.name
  location                     = azurerm_resource_group.main.location
  revision_mode                = "Single"

  template {
    containers {
      name   = "ribbitreels-api"
      image  = "ghcr.io/${var.github_owner}/ribbitreels-api:latest"
      resources {
        cpu    = 0.5
        memory = "1.0Gi"
      }
      env {
        name  = "WEBSITES_PORT"
        value = "8080"
      }
    }

    ingress {
      external_enabled = true
      target_port      = 8080
      transport        = "auto"
    }
  }

  registry_credential {
    server   = "ghcr.io"
    username = var.github_owner
    secret   = var.github_token
  }
}
