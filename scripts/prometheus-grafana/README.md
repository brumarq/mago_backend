# README: Adding Prometheus Configuration to Prometheus Deployment

This README provides guidance on how to update the Prometheus configuration in a deployment, particularly in an OpenShift environment. The steps involve creating a ConfigMap from a Prometheus configuration file and then updating the Prometheus deployment to use this ConfigMap.

## Prerequisites

Before proceeding, ensure that you have:

1. **Access to the OpenShift:** You should have `oc` command-line tool installed and configured to interact with your cluster.

2. **Prometheus Deployment:** Ensure that Prometheus is already deployed in your cluster.

3. **Prometheus Configuration File:** Have the `prometheus.yml` file ready.

## Step-by-Step Guide

### Step 1: Create a ConfigMap from the Prometheus Configuration File

First, you need to create a ConfigMap in your cluster that contains the `prometheus.yml` configuration file. Use the following command:

```sh
oc create configmap prometheus-config --from-file=prometheus.yml
```

This command creates a new ConfigMap named `prometheus-config` and includes `prometheus.yml` as its data.

### Step 2: Update the Prometheus Deployment

Next, update your Prometheus deployment to use the newly created ConfigMap. The following command mounts the ConfigMap as a volume in the Prometheus deployment.

```sh
oc set volume deployment/prometheus --add --overwrite --name=prometheus-config --configmap-name=prometheus-config --mount-path=/etc/prometheus/
```

### Step 3: Restart the Prometheus Deployment

After updating the deployment, you need to restart it to apply the new configuration:

```sh
oc rollout restart deployment/prometheus
```

This command restarts the Prometheus deployment, ensuring that it picks up the new configuration from the ConfigMap.