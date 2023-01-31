<script setup>
  import item from "./ProductItem.vue"
  const props = defineProps(['products'])
</script>

<template>
  <div>
    <h1>Cart</h1>
    <item v-for= "p in products" :product=p :key="p[3]"/>
  </div>

</template>
