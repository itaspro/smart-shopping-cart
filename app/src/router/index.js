import { createRouter, createWebHistory } from 'vue-router'
import CheckoutView from '../views/CheckoutView.vue'
import AdminView from '../views/AdminView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'checkout',
      component: CheckoutView
    },
    {
      path: '/admin',
      name: 'Admin',
      component: AdminView
    },

  ]
})

export default router
