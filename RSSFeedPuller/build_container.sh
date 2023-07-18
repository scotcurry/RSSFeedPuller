# This builds the image and stores it in the Azure Container Registry
az acr build --image datadogcurryware/rssfeedpuller:v3.717.1237 \
  --registry curryware.azurecr.io \
  --file Dockerfile .


# This starts the container using the image from above.
az containerapp up \
  --name curryware-rssfeedpuller \
  --resource-group Curryware_Azure_Resources \
  --ingress external \
  --target-port 80 \
  --env-vars "DD_API_KEY=$DD_API_KEY" "DD_TRACE_ENABLED=true" "DD_SITE='datadoghq.com'" \
  --image curryware.azurecr.io/datadogcurryware/rssfeedpuller:v3.717.237