# This builds the image and stores it in the Azure Container Registry
az acr build --image datadogcurryware/rssfeedpuller:v3.724.994 \
  --registry curryware.azurecr.io \
  --file Dockerfile .


# This starts the container using the image from above.
az containerapp up \
  --name curryware-rssfeedpuller \
  --resource-group Curryware_Azure_Resources \
  --ingress external \
  --target-port 80 \
  --env-vars "DD_API_KEY=$DD_API_KEY" "DD_TRACE_ENABLED=true" "DD_SITE=datadoghq.com" "LOG_PATH=/var/log" \
    "DD_LOGS_ENABLED=true" "DD_APM_ENABLED=true" "CORECLR_ENABLE_PROFILING=1" \
    "CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}" \
    "CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so" \
    "DD_DOTNET_TRACER_HOME=/opt/datadog" "DD_LOGS_INJECTION=true" "DD_RUNTIME_METRICS_ENABLED=true" \
    "DD_ENV=prod" "DD_VERION=3.724.994" "DD_SERVICE=rssfeedpuller" "DD_RUNTIME_SECURITY_CONFIG_ENABLED=true" \
  --image curryware.azurecr.io/datadogcurryware/rssfeedpuller:v3.724.994
