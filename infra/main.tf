terraform {
  required_version = ">= 1.8.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
  }

  backend "azurerm" {
    resource_group_name  = "ribbitreels-rg"
    storage_account_name = "ribbitreelstfstate"
    container_name       = "tfstate"
    key                  = "infra.terraform.tfstate"
  }
}

provider "azurerm" {
  features {}

  client_id                        = var.client_id
  client_secret                    = var.client_secret
  tenant_id                        = var.tenant_id
  subscription_id                  = var.subscription_id
  resource_provider_registrations = ["Microsoft.App", "Microsoft.ContainerRegistry"]
}

resource "azurerm_resource_group" "main" {
  name     = "ribbitreels-rg"
  location = "East US 2"
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
  revision_mode                = "Single"

  template {
    container {
      name   = "ribbitreels-api"
      image  = "ghcr.io/${var.github_owner}/ribbitreels-api:latest"
      cpu    = 0.5
      memory = "1Gi"

      env {
        name  = "WEBSITES_PORT"
        value = "8080"
      }
    }
  }

  ingress {
    external_enabled = true
    target_port      = 8080

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }

  secret {
    name  = "ghcr-token"
    value = var.github_token
  }

  registry {
    server               = "ghcr.io"
    username             = var.github_owner
    password_secret_name = "ghcr-token"
  }
}
