<script setup>
  import { onMounted , reactive} from 'vue';
  import TrainingItem from '../components/TrainingItem.vue';

  const state = reactive({traningData : []})
  onMounted(() => {
    state.traningData = Object.keys(localStorage)
      .filter(k => k.startsWith("image:"))
      .map(k=> JSON.parse(localStorage.getItem(k)))
  })
</script>
<template>
  <div class="admin">
    <TrainingItem :item="item" v-for="item in state.traningData" :key="item.label">
    </TrainingItem>
  </div>
</template>

<style scoped>
</style>
