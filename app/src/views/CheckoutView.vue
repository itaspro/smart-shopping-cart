<script setup>
import Counter from "../components/Counter.vue";
import Cart from "../components/Cart.vue";
import stocks from "../data.json"
import  { BlobServiceClient } from "@azure/storage-blob";
import { reactive, ref, onMounted, nextTick } from "vue";
  const state = reactive({
    isLoading: false,
    products: [],
    labels: [],
  });

  const containerName = "azureml";
  onMounted(async () => {
    state.isLoading = true;
    state.labels = await loadLabels();
    state.isLoading = false;

    //const { text } = await (await fetch("/api/ImageUpload")).json();
  });

  let loadLabels = async () => {
    let resp = await fetch("https://machinelearnin1260987423.blob.core.windows.net/models/labels.txt");
    return (await resp.text()).split("\n");
  };

  let onDetected =  (data ) => {
    state.products = data
      .filter((p) => p.confidence > 0.5)
      .map((p) => {
        return {...stocks[p.sku], label: state.labels[p.sku], imageData: p.imageData, count: 1}
      })
  }

  let updateProduct =  (product ) => {
    const id = product.id
    let idx = state.products.findIndex(p => p.id == id)
    state.products[idx] = {...product}
  }

  let onProductItemAdded = async (p) => {
    console.log("onProductItemAdded")
    let product = {...stocks[p.sku], label: p.label, imageData: p.imageData, count: 1}
    state.products = [
      ...state.products,
      product
    ]

    if (p.saveForTrainging) {
        console.log("...")
        let resp  = await fetch("/api/imageUpload", { method: "POST"})
        let {sasToken, container,blobServiceUrl } = await resp.json()
        const blobServiceClient = new BlobServiceClient(`${blobServiceUrl}?${sasToken}`);
        const containerClient = blobServiceClient.getContainerClient(container);
        let data = {
          label: p.label, 
          width: p.width, 
          height: p.height, 
          image: p.image,
          augment: p.augment
        }
        const content = JSON.stringify(data);
        const blobName = `${data.label}__` + new Date().getTime();
        const blockBlobClient = containerClient.getBlockBlobClient(blobName);
        const uploadBlobResponse = await blockBlobClient.upload(content, content.length);
        console.log(`Upload block blob ${blobName} successfully`, uploadBlobResponse.requestId);
    }
  }
</script>

<template>
  <v-container fluid>
    <v-row class="container">
      <v-col cols="8">
        <Cart :products="state.products" @productUpdated="updateProduct" class="side" />
      </v-col>
      <v-col cols="4">
        <Counter :labels="state.labels" @onDetected="onDetected" @onProductItemAdded="onProductItemAdded" :threshold=0.5 class="content"/>
      </v-col>

    </v-row>

  </v-container>
  
</template>

<style scoped>
  .container {
    height: 100%;
  }

</style>
