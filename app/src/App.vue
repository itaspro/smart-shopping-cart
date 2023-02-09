<script setup>
  import { RouterLink, RouterView } from 'vue-router'
  import {reactive} from 'vue'
  const state = reactive({drawer: false})
</script>

<template>
    <v-app >

    <v-navigation-drawer v-model="state.drawer">
        <router-link to="/">POS</router-link>
        <router-link to="/Admin">Admin</router-link>
    </v-navigation-drawer>

    <v-app-bar>
      <v-app-bar-nav-icon @click="state.drawer = !state.drawer"></v-app-bar-nav-icon>

      <v-toolbar-title>Smart Checkout</v-toolbar-title>
    </v-app-bar>

    <v-main>
        <RouterView />
    </v-main>
  </v-app> 
</template>

<style scoped>
  .v-main {
    display: flex;
    flex-direction: columns;
  }
</style>