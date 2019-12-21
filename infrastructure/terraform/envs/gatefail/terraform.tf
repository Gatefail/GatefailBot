module "cluster" {
  source                = "../../modules/cluster"
  
  location              = var.gcp_location

  cluster_name          = "${var.gcp_project_name}-k8s-cluster"
  cluster_labels        = { environment = var.gcp_project_name }

  node_pool_name        = "${var.gcp_project_name}-primary-nodepool"
  node_pool_labels      = { environment = var.gcp_project_name }
  node_count            = 2
}

module "istio" {
  source                = "../../modules/istio"

  kube_config_path      = module.cluster.kubeconfig_file
  master_endpoint       = module.cluster.master_endpoint
}

