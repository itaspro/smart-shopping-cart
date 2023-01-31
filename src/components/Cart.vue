<script setup>
  import item from "./ProductItem.vue"
  const props = defineProps(['products'])
</script>

<template>
  <div>
    <h1>Shopping cart</h1>
    <h2 class="sub-heading">Free shipping from $100!</h2>

    <item v-for= "p in products" :product=p :key="p[3]"/>
  </div>

</template>
