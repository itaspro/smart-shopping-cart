<script setup>
  import item from "./ProductItem.vue"
  import {computed} from "vue";
  const props = defineProps(['products'])
  const emits = defineEmits("productUpdated")
  const total = computed(() => {
    let t = {
      subtotal: 0,
      gst: 0,
      pst: 0,
      sum: 0
    }
    if (props.products) {
      let subtotal = props.products.reduce((acc, c) => acc+ parseFloat(c.price) *c.count, 0)
      t = {
        subtotal: subtotal.toFixed(2),
        gst: (subtotal * 0.05).toFixed(2),
        pst: (subtotal * 0.07).toFixed(2),
        total: (subtotal * 1.12).toFixed(2)
      }
    }
    return t
  })

  const notifyProductUpdated = (product) => {
    emits("productUpdated", product)
  }

</script>

<template>
  <v-container>
    <v-row>
      <v-col cols="3">
      <section class="_grid cart-totals">
        <div class="_column subtotal" id="subtotalCtr">
          <div class="cart-totals-key">Subtotal</div>
          <div class="cart-totals-value">${{ total.subtotal }}</div>
        </div>
        <div class="_column taxed" id="GST">
          <div class="cart-totals-key">GST (5%)</div>
          <div class="cart-totals-value">${{ total.gst }}</div>
        </div>
        <div class="_column taxes" id="PST">
          <div class="cart-totals-key">Taxes (7%)</div>
          <div class="cart-totals-value">${{ total.pst }}</div>
        </div>
        <div class="_column total" id="totalCtr">
          <div class="cart-totals-key">Total</div>
          <div class="cart-totals-value">${{ total.total }}</div>
        </div>
        <div class="_column checkout">
          <button class="_btn checkout-btn entypo-forward">Pay</button>
        </div>
      </section>


      </v-col >
      <v-col cols="9">

    <section class="shopping-cart">
      <ol class="ui-list shopping-cart--list" id="shopping-cart--list">
        <item class="_grid shopping-cart--list-item" @onProductUpdated="notifyProductUpdated" v-for= "p in products" :product=p :key="p.id">
        </item>
      </ol>

  </section>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped>
._btn {
  display: inline-block;
  background-color: #bdc3c7;
  border: none;
  padding: .5em .75em;
  text-align: center;
  font-weight: 300;
}
._btn:hover,
.cart-totals:hover ._btn {
  background-color: #3498db;
  color: #ecf0f1;
}

/**
 * @section: shopping-cart;
 */
.shopping-cart {
  max-width: 60rem;
  margin: 10px;
  /* display: flex;
  flex-direction: column; */
}
/**
 * @extends: _grid;
 */

/**
 * @section: cart-totals;
 * @extends: _grid;
 */
.cart-totals {
  /* border-top: 1px solid #bdc3c7; */
}
.cart-totals ._column {
  width: 19.984013%;
  padding: .5rem;
  line-height: 1.2;
}
.cart-totals ._column:first-of-type {
  padding-left: 0;
}
.cart-totals-key {
  font-size: 1.125em;
  color: #bdc3c7;
}
.cart-totals ._column:hover .cart-totals-value,
.cart-totals ._column:hover .cart-totals-key {
  color: #333;
}
.cart-totals-value {
  font-size: 1.5em;
}
._column.checkout {
  text-align: right;
  padding: 0;
  margin-top: 1.5em;
  vertical-align: middle;
}
.checkout-btn {
  margin-right: .5em;
  padding: 10px;
  width: 150px;
}
._btn.checkout-btn:hover {
  background-color: #2980b9;
}

/**
 * Animations
 */
.product-remove,
.cart-totals * {
  transition: all .2s ease;
}
.closing {
  transition: all .5s ease;
  transform: translate3d(0, -100%, 0);
  opacity: 0;
}


.shopping-cart--list {
  /* flex-grow: 1; */
  min-height: 80%;
}
.shopping-cart--list-item {
  border: 1px solid #bdc3c7;
  /* margin-bottom: 3rem; */
  /* overflow: hidden;
  display: grid; */
  /* grid-template-columns: 100px 1fr 200px; */
}

.shopping-cart--list-item:hover,
.shopping-cart--list-item:hover * {
  border-color: #3498db;
}
.shopping-cart--list-item > ._column {
    height: 100%; /* make vertical lines match */
}

.shopping-cart--list-item:hover .product-total-price {
  background-color: #3498db;
  color: #ecf0f1;
}
</style>