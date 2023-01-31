<script setup>
  const props = defineProps(['products'])
</script>

<template>
  <div>
    <h1>Shopping cart</h1>
    <h2 class="sub-heading">Free shipping from $100!</h2>

    <p id="products" v-for="p in products" :key="p[2]">{{p[3] }}</p>
  </div>

</template>
