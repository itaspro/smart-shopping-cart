<script setup>
import Counter from "../components/Counter.vue";
import Cart from "../components/Cart.vue";

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

  let onDetected = async (data ) => {
    const zip = (arr, ...arrs) => {
      return arr.map((val, i) => arrs.reduce((a, arr) => [...a, arr[i]], [val]));
    };

    state.products = zip(...data)
      .filter((d) => d[1] > 0.5)
      .map((p) => [...p, state.labels[p[2]]]);
  }
</script>

<template>
  <div class="container">
    <Cart :products="state.products" class="side" />
    <Counter :onDetected="onDetected" class="content"/>
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
    background-color: aquamarine;
  }
  
</style>
