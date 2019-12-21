variable "kube_config_path" {
    type        = string
    description = "Path to kubeconfig file"
    default     = "/.kube/kubeconfig"
}

variable "master_endpoint" {
    type        = string
    description = "IP / Hostname of the k8s master"
}



