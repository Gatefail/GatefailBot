resource "random_password" "username" {
  length = 16
  special = false
}

resource "random_password" "password" {
  length = 64
  special = true
  override_special = "_%@"
}

resource "google_container_cluster" "primary" {
  name     = var.cluster_name
  location = var.location

  # We can't create a cluster with no node pool defined, but we want to only use
  # separately managed node pools. So we create the smallest possible default
  # node pool and immediately delete it.
  remove_default_node_pool = true
  initial_node_count       = 1
  resource_labels          = var.cluster_labels
  master_auth {
    username = length(var.master_username) == 0 ? random_password.username.result : var.master_username
    password = length(var.master_password) == 0 ? random_password.password.result : var.master_password

    client_certificate_config {
      issue_client_certificate = true
    }
  }

  node_config {
    preemptible  = true
    machine_type = "g1-small"
    disk_size_gb = 10

    metadata = {
      disable-legacy-endpoints = "true"
    }

    oauth_scopes = [
      "https://www.googleapis.com/auth/logging.write",
      "https://www.googleapis.com/auth/monitoring",
    ]
  }
}

resource "google_container_node_pool" "primary_preemptible_nodes" {
  name       = length(var.node_pool_name) == 0 ? "${var.cluster_name}-primary-nodepool" : var.node_pool_name
  location   = var.location
  cluster    = google_container_cluster.primary.name
  node_count = 2

  node_config {
    preemptible  = var.preemptible_nodes
    machine_type = var.machine_type
    disk_size_gb = var.disk_size
    labels       = var.node_pool_labels

    metadata = {
      disable-legacy-endpoints = "true"
    }

    oauth_scopes = [
      "https://www.googleapis.com/auth/logging.write",
      "https://www.googleapis.com/auth/monitoring",
    ]
  }
}

data "template_file" "kubeconfig" {
  template = file("${path.module}/kubeconfig-template.yaml")

  vars = {
    cluster_name    = google_container_cluster.primary.name
    user_name       = google_container_cluster.primary.master_auth[0].username
    user_password   = google_container_cluster.primary.master_auth[0].password
    endpoint        = google_container_cluster.primary.endpoint
    cluster_ca      = google_container_cluster.primary.master_auth[0].cluster_ca_certificate
    client_cert     = google_container_cluster.primary.master_auth[0].client_certificate
    client_cert_key = google_container_cluster.primary.master_auth[0].client_key
  }
}

resource "local_file" "kubeconfig" {
  depends_on = [google_container_node_pool.primary_preemptible_nodes]
  content  = data.template_file.kubeconfig.rendered
  filename = "${path.root}/credentials/kubeconfig"
}

output "kubeconfig_file" {
  value = local_file.kubeconfig.filename
}

output "master_endpoint" {
  value = google_container_cluster.primary.endpoint
}
