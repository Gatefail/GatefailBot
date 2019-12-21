provider "google" {
  credentials   = file("./credentials/serviceaccount.json")
  project       = var.gcp_project_name
  region        = var.gcp_location
}
