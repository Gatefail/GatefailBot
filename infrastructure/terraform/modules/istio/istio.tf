# ISTIO
resource "null_resource" "install_istio" {
  triggers = {
    cluster_ep = var.master_endpoint
  }
  
  provisioner "local-exec" {
    command = <<EOT
      curl -L https://istio.io/downloadIstio | sh -
      export ISTIODIR=$(find . -maxdepth 1 -type d -name 'istio*')
      istioctl manifest apply \
      --set values.grafana.enabled=false \
      --set values.prometheus.enabled=false \
      --set values.kiali.enabled=true \
      --set values.tracing.enabled=false \
      -c $KUBECONFIG
    EOT

    environment = {
      KUBECONFIG = var.kube_config_path
    }
  }
}