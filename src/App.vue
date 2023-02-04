<script setup>
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  
  
  <v-app>
      <v-layout>
        <v-navigation-drawer>
          <v-list>
            <v-list-item title="Menu">
              <router-link to="/">POS</router-link>
              <router-link to="/Admin">Admin</router-link>
            </v-list-item>
          </v-list>
        </v-navigation-drawer>
  
        <v-app-bar title="Smart Checkout"></v-app-bar>
  
        <v-main >
          <RouterView />
        </v-main>
      </v-layout>
  </v-app>
</template>

<style scoped>
  .v-main {
    display: flex;
    flex-direction: columns;
  }
</style>