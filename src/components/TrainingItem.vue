<script setup>
  import {  ref,reactive, onMounted, computed, watch  } from "vue";
  const pic = ref(null)
  const props = defineProps(['item'])
  onMounted(() =>{
  })
</script>

<template>
  <div class="_row">
      <!-- <canvas ref="pic" id="canvas" ></canvas> -->
      <img :src="props.item.image"/>
      <h1>{{ props.item.label }}</h1>
  </div>
</template>

<style scoped>

</style>