<script setup>
  import item from "./ProductItem.vue"
  const props = defineProps(['products'])
</script>

<template>
  <div>
    <h1>Shopping cart</h1>
    <h2 class="sub-heading">Free shipping from $100!</h2>

    <section class="shopping-cart">
      <ol class="ui-list shopping-cart--list" id="shopping-cart--list">

        <item class="_grid shopping-cart--list-item" v-for= "p in products" :product=p :key="p.id">
  
        </item>

    </ol>

    <footer class="_grid cart-totals">
      <div class="_column subtotal" id="subtotalCtr">
        <div class="cart-totals-key">Subtotal</div>
        <div class="cart-totals-value">$0.00</div>
      </div>
      <div class="_column shipping" id="shippingCtr">
        <div class="cart-totals-key">Shipping</div>
        <div class="cart-totals-value">$0.00</div>
      </div>
      <div class="_column taxes" id="taxesCtr">
        <div class="cart-totals-key">Taxes (6%)</div>
        <div class="cart-totals-value">$0.00</div>
      </div>
      <div class="_column total" id="totalCtr">
        <div class="cart-totals-key">Total</div>
        <div class="cart-totals-value">$0.00</div>
      </div>
      <div class="_column checkout">
        <button class="_btn checkout-btn entypo-forward">Checkout</button>
      </div>
    </footer>

  </section>
  </div>

</template>
