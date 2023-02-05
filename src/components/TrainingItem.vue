<script setup>
  import {  ref,reactive, onMounted, computed, watch  } from "vue";
  const pic = ref(null)
  const props = defineProps(['item'])
  onMounted(() =>{
  })
</script>

<template>

  <v-row>

      <h2>{{ props.item.label }}</h2>
  </v-row>
  <v-row>
      <!-- <canvas ref="pic" id="canvas" ></canvas> -->
      <img class="training-images" v-for="item in props.item.items" :src="item.image" :key="item.label"/>
  </v-row>
</template>

<style scoped>
  img.training-images {
    max-width: 120px;
    max-height: 120px;
    margin: 10px;
  }
</style>