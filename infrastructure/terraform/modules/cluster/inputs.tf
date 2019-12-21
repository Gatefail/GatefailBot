variable "cluster_name" {
    type        = string
    description = "Name of the created cluster"
}

variable "location" {
    type        = string
    description = "Which zone the cluster will be created in, e.g. europe-west1-b"
}

variable "master_username" {
    type        = string
    description = "Username for basic auth. Not recommended"
    default     = ""
}

variable "master_password" {
    type        = string
    description = "Password for basic auth. Not recommended"
    default     = ""
}

variable "node_pool_name" {
    type        = string
    description = "Name of the created node pool"
    default     = "" 
}

variable "node_count" {
    type        = number
    description = "Number of nodes in the created pool"
    default     = 1
}

variable "preemptible_nodes" {
    type        = bool
    description = "Whether preemptible nodes should be used"
    default     = true  
}

variable "machine_type" {
    type        = string
    default     = "g1-small"  
}

variable "disk_size" {
    type        = number
    description = "Disc size in gigabytes"
    default     = 10
}

variable "cluster_labels" {
    type        = map(string)
    description = "Map of labels to be placed on the created cluster"
    default     = {}
}

variable "node_pool_labels" {
    type        = map(string)
    description = "Map of labels to be placed on the created node pool"
    default     = {}
}






