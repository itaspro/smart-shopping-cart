<script setup>
import Counter from "../components/Counter.vue";
import Cart from "../components/Cart.vue";
import stocks from "../data.json"

import { reactive, ref, onMounted, nextTick } from "vue";
  const state = reactive({
    isLoading: false,
    products: [],
    labels: [],
  });

  onMounted(async () => {
    state.isLoading = true;
    state.labels = await loadLabels();
    state.isLoading = false;
  });

  let loadLabels = async () => {
    let resp = await fetch("v4/labels.txt");
    return (await resp.text()).split("\n");
  };

  let onDetected =  (data ) => {
    state.products = data
      .filter((p) => p.confidence > 0.5)
      .map((p) => {
        return {...stocks[p.sku], label: state.labels[p.sku], imageData: p.imageData}
      })
  }

</script>

<template>
  <div class="container">
    <Cart :products="state.products" class="side" />
    <Counter :onDetected="onDetected" :threshold=0.5 class="content"/>
  </div>
</template>

<style scoped>
  .container {
    height: 100%;
    display: flex;
  }

  .side {
    flex: auto;
  }

  .content {
  }
  
</style>
