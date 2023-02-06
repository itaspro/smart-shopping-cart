<script setup>
  import { onMounted , reactive} from 'vue';
  import TrainingItem from '../components/TrainingItem.vue';

  const state = reactive({traningData : []})
  onMounted(() => {
    const groups = Object.keys(localStorage)
      .filter(k => k.startsWith("image:"))
      .map(k=> JSON.parse(localStorage.getItem(k)))
      .reduce((g, p) => {
        let a = g[p.label] ||[]
        a.push(p)
        g[p.label] = a
        return g
      },{})

    state.traningData = 
      Object.keys(groups)
      .map(label => ({
          label, 
          items: groups[label]
      }))
  })
</script>
<template>
  <v-container fluid>
    <TrainingItem :item="item" v-for="item in state.traningData" :key="item.label">
    </TrainingItem>
  </v-container>
</template>

<style scoped>
</style>
