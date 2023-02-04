<script setup>
  import {  ref,reactive, onMounted, computed, watch  } from "vue";
  const pic = ref(null)
  const props = defineProps(['item'])
  onMounted(() =>{
    let ctx = pic.value.getContext('2d')
    ctx.putImageData(props.item.imageData,0,0)
  })
</script>

<template>
  <div class="_row">
      <canvas ref="pic" id="canvas" ></canvas>
      <h4 >{{item.label}}</h4>
  </div>
</template>

<style scoped>

</style>