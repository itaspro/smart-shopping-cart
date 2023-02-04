<script setup>
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <router-link to="/">POS</router-link>
  <router-link to="/Admin">Admin</router-link>
  <RouterView />
</template>
